using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using VesselCalc.Application.Interfaces;
using VesselCalc.Application.Services;
using VesselCalc.Infrastructure.Data;
using VesselCalc.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 120,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    allowedOrigins = ["http://localhost:5173"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .WithMethods("GET", "POST", "OPTIONS")
                  .WithHeaders("Content-Type", "Accept");
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMaterialProvider, MaterialProvider>();
builder.Services.AddScoped<IShellThicknessService, ShellThicknessService>();

var app = builder.Build();

// === DATABASE MIGRATION & SEEDER SCOPE ===
var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var seedDir = Path.Combine(AppContext.BaseDirectory, "Data", "Seed");
        if (Directory.Exists(seedDir))
        {
            foreach (var file in Directory.GetFiles(seedDir, "*.csv"))
                await DatabaseSeeder.SeedMaterialsAsync(context, file);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning("Database migration/seeding skipped ({Reason}). Seeded CSV fallback will be used.", ex.GetType().Name);
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("FrontendPolicy");
app.UseRateLimiter();
app.MapControllers();

app.Run();
