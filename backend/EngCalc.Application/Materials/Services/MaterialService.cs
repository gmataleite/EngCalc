using EngCalc.Application.Materials.DTOs;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Domain.Entities;

namespace EngCalc.Application.Materials.Services;

public class MaterialService : IMaterialService
{
    private readonly IMaterialProvider _materialProvider;

    public MaterialService(IMaterialProvider materialProvider)
    {
        _materialProvider = materialProvider;
    }

    public async Task<IReadOnlyList<MaterialOptionDto>> GetMaterialsAsync(CancellationToken cancellationToken = default)
    {
        var materials = await _materialProvider.GetAllMaterialsAsync(cancellationToken);

        return materials
            .OrderBy(m => m.MaterialId)
            .Select(FormatMaterialOption)
            .ToList();
    }

    private static MaterialOptionDto FormatMaterialOption(Material m)
    {
        var gradePart = !string.IsNullOrWhiteSpace(m.Grade) ? $" Gr. {m.Grade}" : string.Empty;
        var formPart = !string.IsNullOrWhiteSpace(m.ProductForm) ? $" — Carbon/Alloy Steel {m.ProductForm}" : string.Empty;
        var label = $"ASTM/ASME {m.Specification}{gradePart}{formPart}";

        return new MaterialOptionDto(
            Id: m.MaterialId,
            Label: label,
            Specification: m.Specification,
            Grade: m.Grade,
            ProductForm: m.ProductForm,
            MaxTemperatureLimit: m.MaxTemperatureLimit);
    }
}

