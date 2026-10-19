using VesselCalc.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VesselCalc.Infrastructure.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");
        builder.HasKey(m => m.MaterialId);

        builder.Property(m => m.MaterialId).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Specification).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Grade).HasMaxLength(50);
        builder.Property(m => m.ProductForm).HasMaxLength(50);
        builder.Property(m => m.MinTensileStrength).HasColumnType("decimal(18,2)");
        builder.Property(m => m.MinYieldStrength).HasColumnType("decimal(18,2)");
        builder.Property(m => m.Notes).HasMaxLength(255);

        builder.HasMany(m => m.TemperatureProperties)
            .WithOne()
            .HasForeignKey(p => p.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.TemperatureProperties)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}