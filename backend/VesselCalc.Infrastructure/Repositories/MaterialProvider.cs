using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VesselCalc.Application.Interfaces;
using VesselCalc.Domain.Entities;
using VesselCalc.Infrastructure.Data;

namespace VesselCalc.Infrastructure.Repositories;

public class MaterialProvider : IMaterialProvider
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MaterialProvider> _logger;

    public MaterialProvider(ApplicationDbContext dbContext, ILogger<MaterialProvider> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Material>> GetAllMaterialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var dbMaterials = await _dbContext.Materials
                .Include(m => m.TemperatureProperties)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (dbMaterials.Count > 0)
            {
                return dbMaterials;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Database unreachable when querying materials ({Reason}); falling back to seeded CSV.", ex.GetType().Name);
        }

        return await LoadSeededCsvMaterialsAsync(cancellationToken);
    }

    public async Task<Material?> GetMaterialByIdAsync(string materialId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(materialId)) return null;

        try
        {
            var material = await _dbContext.Materials
                .Include(m => m.TemperatureProperties)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m => m.MaterialId.ToUpper() == materialId.ToUpper(),
                    cancellationToken);

            if (material is not null)
            {
                return material;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Database unreachable when querying material ({Reason}); falling back to seeded CSV.", ex.GetType().Name);
        }

        var csvMaterials = await LoadSeededCsvMaterialsAsync(cancellationToken);
        return csvMaterials.FirstOrDefault(m => string.Equals(m.MaterialId, materialId, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<IReadOnlyList<Material>> LoadSeededCsvMaterialsAsync(CancellationToken cancellationToken)
    {
        var seedDir = Path.Combine(AppContext.BaseDirectory, "Data", "Seed");
        var results = new List<Material>();

        if (!Directory.Exists(seedDir))
        {
            return results;
        }

        foreach (var file in Directory.GetFiles(seedDir, "*.csv"))
        {
            var parsed = await DatabaseSeeder.ParseMaterialsFromCsvAsync(file, cancellationToken);
            results.AddRange(parsed);
        }

        return results;
    }
}

