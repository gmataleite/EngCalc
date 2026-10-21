using VesselCalc.Application.DTOs;

namespace VesselCalc.Application.Interfaces;

public interface IShellThicknessService
{
    Task<IReadOnlyList<MaterialOptionDto>> GetMaterialsAsync(CancellationToken cancellationToken = default);
    ShellThicknessOptionsDto GetOptions();
    Task<ShellThicknessResponseDto> CalculateAsync(ShellThicknessRequestDto request, CancellationToken cancellationToken = default);
}

