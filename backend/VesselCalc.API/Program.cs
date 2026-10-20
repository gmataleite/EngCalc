using Microsoft.EntityFrameworkCore;
using VesselCalc.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// === INÍCIO DO ESCOPO DO SEEDER ===
using (var scope = app.Services.CreateScope())
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
// === FIM DO ESCOPO ===

app.Run();