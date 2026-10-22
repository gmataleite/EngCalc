using Microsoft.EntityFrameworkCore;
using EngCalc.Domain.Entities;

namespace EngCalc.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialTemperatureProperty> MaterialTemperatureProperties => Set<MaterialTemperatureProperty>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

