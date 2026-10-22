using EngCalc.Domain.Entities;

namespace EngCalc.Application.Materials.Interfaces;

public interface IMaterialProvider
{
    Task<IReadOnlyList<Material>> GetAllMaterialsAsync(CancellationToken cancellationToken = default);
    Task<Material?> GetMaterialByIdAsync(string materialId, CancellationToken cancellationToken = default);
}

