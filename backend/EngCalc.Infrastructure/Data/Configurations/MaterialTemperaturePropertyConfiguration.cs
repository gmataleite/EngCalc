using EngCalc.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngCalc.Infrastructure.Data.Configurations;

public class MaterialTemperaturePropertyConfiguration : IEntityTypeConfiguration<MaterialTemperatureProperty>
{
    public void Configure(EntityTypeBuilder<MaterialTemperatureProperty> builder)
    {
        builder.ToTable("MaterialsTemperatureProperties");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MaterialId).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Temperature).HasColumnType("decimal(18,2)");
        builder.Property(p => p.AllowableStress).HasColumnType("decimal(18,2)");
        builder.Property(p => p.YieldStress).HasColumnType("decimal(18,2)");

        builder.HasIndex(p => new { p.MaterialId, p.Temperature }).IsUnique();
    }
}

