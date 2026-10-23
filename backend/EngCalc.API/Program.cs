using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
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

// 2. Injeção de Dependências (Services & Providers)
ConfigureDependencies(builder.Services);

// 3. Configurações da API (Controllers, JSON, OpenAPI, CORS)
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
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

void ConfigureDependencies(IServiceCollection services)
{
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
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddOpenApi();

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    if (allowedOrigins is null || allowedOrigins.Length == 0)
    {
        allowedOrigins = ["http://localhost:5173"];
    }

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendPolicy", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

async Task ConfigurePipelineAsync(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    await ExecuteDatabaseMigrationAndSeedAsync(app);

    app.UseHttpsRedirection();
    app.UseCors("FrontendPolicy");
    app.MapControllers();
}

async Task ExecuteDatabaseMigrationAndSeedAsync(WebApplication app)
{
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

public partial class Program { }



