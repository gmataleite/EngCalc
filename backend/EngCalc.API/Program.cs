using System.Net;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using EngCalc.Application.Converters.Interfaces;
using EngCalc.Application.Converters.Services;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.Materials.Services;
using EngCalc.Application.PressureVessels.Interfaces;
using EngCalc.Application.PressureVessels.Services;
using EngCalc.Infrastructure.Data;
using EngCalc.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações de Banco de Dados
ConfigureDatabase(builder);

// 2. Injeção de Dependências (Services, Cache & Providers)
ConfigureDependencies(builder.Services);

// 3. Configurações da API (Controllers, JSON, OpenAPI, CORS, Rate Limiting, ProblemDetails)
ConfigureApi(builder);

var app = builder.Build();

// 4. Pipeline de Requisições HTTP e Inicialização de Dados
await ConfigurePipelineAsync(app);

app.Run();

// ============================================================================
// MÉTODOS DE CONFIGURAÇÃO (Local Functions)
// ============================================================================

void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = ResolveConnectionString(builder.Configuration);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

void ConfigureDependencies(IServiceCollection services)
{
    services.AddMemoryCache();

    // Providers / Repositories
    services.AddScoped<IMaterialProvider, MaterialProvider>();

    // Application Services
    services.AddScoped<IMaterialService, MaterialService>();
    services.AddScoped<IShellThicknessService, ShellThicknessService>();
    services.AddScoped<IHeadThicknessService, HeadThicknessService>();
    services.AddScoped<IConverterService, ConverterService>();
}

void ConfigureApi(WebApplicationBuilder builder)
{
    builder.Services.AddProblemDetails();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: clientIp,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 120,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
        });
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddOpenApi();

    var allowedOrigins = ResolveAllowedOrigins(builder.Configuration, builder.Environment);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendPolicy", policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                      .WithHeaders("Content-Type", "Accept", "Authorization")
                      .WithMethods("GET", "POST", "OPTIONS");
            }
        });
    });
}

async Task ConfigurePipelineAsync(WebApplication app)
{
    app.UseForwardedHeaders();
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    else
    {
        app.UseHsts();
    }

    app.Use(async (context, next) =>
    {
        context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
        context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
        context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    });

    await ExecuteDatabaseMigrationAndSeedAsync(app);

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors("FrontendPolicy");
    app.UseRateLimiter();

    app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
    app.MapControllers();
}

async Task ExecuteDatabaseMigrationAndSeedAsync(WebApplication app)
{
    var autoMigrate = app.Configuration.GetValue("Database:AutoMigrateAndSeed", true);
    if (!autoMigrate)
    {
        return;
    }

    var connectionString = ResolveConnectionString(app.Configuration);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var seedDir = Path.Combine(AppContext.BaseDirectory, "Data", "Seed");
        if (Directory.Exists(seedDir))
        {
            foreach (var file in Directory.GetFiles(seedDir, "*.csv"))
                await DatabaseSeeder.SeedMaterialsAsync(context, file);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database migration/seeding skipped because PostgreSQL is unavailable. Seeded CSV fallback will be used.");
    }
}

static string? ResolveConnectionString(IConfiguration configuration)
{
    var raw = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(raw))
    {
        raw = configuration["DATABASE_URL"];
    }

    if (string.IsNullOrWhiteSpace(raw))
    {
        return null;
    }

    raw = raw.Trim();
    if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(raw);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo.Length > 0 ? WebUtility.UrlDecode(userInfo[0]) : string.Empty;
        var password = userInfo.Length > 1 ? WebUtility.UrlDecode(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.Trim('/');

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = database,
            Username = username,
            Password = password,
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        // Compatibilidade com Supavisor (Supabase Transaction Pooler na porta 6543)
        if (builder.Port == 6543)
        {
            builder.NoResetOnClose = true;
            builder.MaxAutoPrepare = 0;
        }

        return builder.ConnectionString;
    }

    return raw;
}

static string[] ResolveAllowedOrigins(IConfiguration configuration, IHostEnvironment environment)
{
    var fromEnvCsv = configuration["CORS_ALLOWED_ORIGINS"];
    if (!string.IsNullOrWhiteSpace(fromEnvCsv))
    {
        return fromEnvCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(o => o.TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    var fromConfig = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    if (fromConfig is { Length: > 0 })
    {
        return fromConfig
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    return environment.IsDevelopment() ? ["http://localhost:5173"] : [];
}

public partial class Program { }
