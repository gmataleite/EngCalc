using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Domain.Entities;
using EngCalc.Infrastructure.Data;

namespace EngCalc.Infrastructure.Repositories;

public class MaterialProvider : IMaterialProvider
{
    private const string AllMaterialsCacheKey = "EngCalc:Materials:All";
    private static readonly TimeSpan DbCacheDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan FallbackCacheDuration = TimeSpan.FromMinutes(5);

    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MaterialProvider> _logger;

    public MaterialProvider(
        ApplicationDbContext dbContext,
        IMemoryCache cache,
        ILogger<MaterialProvider> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Material>> GetAllMaterialsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(AllMaterialsCacheKey, out IReadOnlyList<Material>? cached) && cached is not null)
        {
            return cached;
        }

        if (HasConfiguredConnectionString())
        {
            try
            {
                var dbMaterials = await _dbContext.Materials
                    .Include(m => m.TemperatureProperties)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (dbMaterials.Count > 0)
                {
                    _cache.Set(AllMaterialsCacheKey, (IReadOnlyList<Material>)dbMaterials, DbCacheDuration);
                    return dbMaterials;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Database unreachable when querying materials ({Reason}); falling back to seeded CSV.", ex.GetType().Name);
            }
        }

        var csvMaterials = await LoadSeededCsvMaterialsAsync(cancellationToken);
        if (csvMaterials.Count > 0)
        {
            _cache.Set(AllMaterialsCacheKey, csvMaterials, FallbackCacheDuration);
        }

        return csvMaterials;
    }

    public async Task<Material?> GetMaterialByIdAsync(string materialId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(materialId) || materialId.Length > 50)
        {
            return null;
        }

        string normalizedId = materialId.Trim().ToUpperInvariant();

        if (_cache.TryGetValue(AllMaterialsCacheKey, out IReadOnlyList<Material>? cached) && cached is not null)
        {
            return cached.FirstOrDefault(m => string.Equals(m.MaterialId, normalizedId, StringComparison.OrdinalIgnoreCase));
        }

        if (HasConfiguredConnectionString())
        {
            try
            {
                var material = await _dbContext.Materials
                    .Include(m => m.TemperatureProperties)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        m => m.MaterialId == normalizedId,
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
        }

        var allMaterials = await GetAllMaterialsAsync(cancellationToken);
        return allMaterials.FirstOrDefault(m => string.Equals(m.MaterialId, normalizedId, StringComparison.OrdinalIgnoreCase));
    }

    private bool HasConfiguredConnectionString()
    {
        try
        {
            var connStr = _dbContext.Database.GetConnectionString();
            return !string.IsNullOrWhiteSpace(connStr);
        }
        catch
        {
            return false;
        }
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
