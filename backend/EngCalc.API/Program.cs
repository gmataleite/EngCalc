using Microsoft.EntityFrameworkCore;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.Materials.Services;
using EngCalc.Application.PressureVessels.Interfaces;
using EngCalc.Application.PressureVessels.Services;
using EngCalc.Infrastructure.Data;
using EngCalc.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMaterialProvider, MaterialProvider>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IShellThicknessService, ShellThicknessService>();
builder.Services.AddScoped<IHeadThicknessService, HeadThicknessService>();

var app = builder.Build();

// === DATABASE MIGRATION & SEEDER SCOPE ===
using (var scope = app.Services.CreateScope())
{
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
        logger.LogWarning(ex, "Database migration/seeding skipped because PostgreSQL is unavailable. Seeded CSV fallback will be used.");
    }
}

app.UseCors("FrontendPolicy");
app.MapControllers();

app.Run();

