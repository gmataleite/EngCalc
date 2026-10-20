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
    
    // Opcional: Garante que as migrations foram aplicadas no banco
    context.Database.Migrate();

    var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "materials.csv");
    
    if (File.Exists(csvPath))
    {
        await DatabaseSeeder.SeedMaterialsAsync(context, csvPath);
    }
}
// === FIM DO ESCOPO ===

app.Run();