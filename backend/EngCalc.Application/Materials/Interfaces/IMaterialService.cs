using EngCalc.Application.Materials.DTOs;

namespace EngCalc.Application.Materials.Interfaces;

public interface IMaterialService
{
    Task<IReadOnlyList<MaterialOptionDto>> GetMaterialsAsync(CancellationToken cancellationToken = default);
}

