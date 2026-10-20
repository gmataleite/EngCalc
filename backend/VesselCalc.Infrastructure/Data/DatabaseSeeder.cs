using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VesselCalc.Domain.Entities;

namespace VesselCalc.Infrastructure.Data;

public static class DatabaseSeeder
{
    private static readonly decimal[] Temperatures = { 100, 200, 300, 400, 500, 600, 700, 800 };

    private const int FirstStressColumn = 6;                                // S100
    private const int FirstYieldColumn = FirstStressColumn + 8;             // Y100
    private const int MinColumns = FirstStressColumn + 8;                   // 14 (Y é opcional)

    public static async Task SeedMaterialsAsync(ApplicationDbContext context, string csvFilePath)
    {
        if (await context.Materials.AnyAsync())
            return;

        if (!File.Exists(csvFilePath))
            throw new FileNotFoundException("CSV de materiais não encontrado.", csvFilePath);

        var lines = await File.ReadAllLinesAsync(csvFilePath);
        var materials = new List<Material>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 1; i < lines.Length; i++) // pula o cabeçalho
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var c = lines[i].Split(';').Select(x => x.Trim()).ToArray();
            if (c.Length < MinColumns)
                throw new FormatException($"Linha {i + 1}: esperadas pelo menos {MinColumns} colunas, encontradas {c.Length}.");

            var materialId = c[0].ToUpperInvariant();
            if (!ids.Add(materialId))
                throw new FormatException($"Linha {i + 1}: MaterialId duplicado '{materialId}'.");

            var material = new Material(
                materialId: materialId,
                specification: c[1],
                grade: NullIfEmpty(c[2]),
                productForm: NullIfEmpty(c[3]),
                minTensile: ParseDecimal(c[4], i),
                minYield: ParseDecimal(c[5], i));

            for (int t = 0; t < Temperatures.Length; t++)
            {
                var allowable = ParseNullableDecimal(GetCell(c, FirstStressColumn + t), i);
                var yield = ParseNullableDecimal(GetCell(c, FirstYieldColumn + t), i);

                // se ambos são nulos, não há o que gravar para essa temperatura
                if (allowable is null && yield is null) continue;

                material.AddTemperaturePoint(Temperatures[t], allowable, yield);
            }

            materials.Add(material);
        }

        await context.Materials.AddRangeAsync(materials);
        await context.SaveChangesAsync();
    }

    // colunas Y podem nem existir na linha
    private static string GetCell(string[] cols, int index) =>
        index < cols.Length ? cols[index] : string.Empty;

    // vazio ou "None" => null
    private static string? NullIfEmpty(string s) =>
        string.IsNullOrWhiteSpace(s) || s.Equals("None", StringComparison.OrdinalIgnoreCase) ? null : s;

    private static decimal ParseDecimal(string s, int line)
    {
        if (!decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            throw new FormatException($"Linha {line + 1}: valor numérico inválido '{s}'.");
        return value;
    }

    // vazio, "NP" e "---" => null
    private static decimal? ParseNullableDecimal(string s, int line)
    {
        if (string.IsNullOrWhiteSpace(s) ||
            s.Equals("NP", StringComparison.OrdinalIgnoreCase) ||
            s == "---")
            return null;

        return ParseDecimal(s, line);
    }
}