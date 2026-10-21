using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VesselCalc.Domain.Entities;

namespace VesselCalc.Infrastructure.Data;

public static class DatabaseSeeder
{
    private static readonly decimal[] Temperatures =
        { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

    // 0 MaterialId | 1 Specification | 2 Grade | 3 ProductForm
    // 4 MinTensile | 5 MinYield | 6 MaxTemp | 7..16 S100..S1000 | 17..26 Y100..Y1000
    private const int MaxTempColumn = 6;
    private const int FirstStressColumn = 7;
    private static readonly int FirstYieldColumn = FirstStressColumn + Temperatures.Length;
    private static readonly int MinColumns = FirstStressColumn + Temperatures.Length; // Y is optional

    public static async Task<List<Material>> ParseMaterialsFromCsvAsync(string csvFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(csvFilePath))
            throw new FileNotFoundException("Materials CSV file not found.", csvFilePath);

        var lines = await File.ReadAllLinesAsync(csvFilePath, cancellationToken);
        var materials = new List<Material>();
        var idsInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var c = lines[i].Split(';').Select(x => x.Trim()).ToArray();
            if (c.Length < MinColumns)
                throw new FormatException($"Line {i + 1}: expected at least {MinColumns} columns, found {c.Length}.");

            var materialId = c[0].ToUpperInvariant();

            if (!idsInFile.Add(materialId))
                throw new FormatException($"Line {i + 1}: duplicate MaterialId '{materialId}'.");

            var material = new Material(
                materialId: materialId,
                specification: c[1],
                grade: NullIfEmpty(c[2]),
                productForm: NullIfEmpty(c[3]),
                minTensile: ParseDecimal(c[4], i),
                minYield: ParseDecimal(c[5], i),
                maxTemperatureLimit: ParseDecimal(c[MaxTempColumn], i));

            for (int t = 0; t < Temperatures.Length; t++)
            {
                var allowable = ParseNullableDecimal(GetCell(c, FirstStressColumn + t), i);
                var yield = ParseNullableDecimal(GetCell(c, FirstYieldColumn + t), i);
                if (allowable is null && yield is null) continue;

                material.AddTemperaturePoint(Temperatures[t], allowable, yield);
            }

            materials.Add(material);
        }

        return materials;
    }

    public static async Task SeedMaterialsAsync(ApplicationDbContext context, string csvFilePath)
    {
        var parsedMaterials = await ParseMaterialsFromCsvAsync(csvFilePath);

        var existingIds = (await context.Materials
                .Select(m => m.MaterialId)
                .ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newMaterials = parsedMaterials
            .Where(m => !existingIds.Contains(m.MaterialId))
            .ToList();

        if (newMaterials.Count == 0) return;

        context.Materials.AddRange(newMaterials);
        await context.SaveChangesAsync();
    }

    private static string GetCell(string[] cols, int index) =>
        index < cols.Length ? cols[index] : string.Empty;

    private static string? NullIfEmpty(string s) =>
        string.IsNullOrWhiteSpace(s) || s.Equals("None", StringComparison.OrdinalIgnoreCase) ? null : s;

    private static decimal ParseDecimal(string s, int line)
    {
        if (!decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            throw new FormatException($"Line {line + 1}: invalid numeric value '{s}'.");
        return value;
    }

    private static decimal? ParseNullableDecimal(string s, int line)
    {
        if (string.IsNullOrWhiteSpace(s) ||
            s.Equals("NP", StringComparison.OrdinalIgnoreCase) ||
            s == "---")
            return null;

        return ParseDecimal(s, line);
    }
}
