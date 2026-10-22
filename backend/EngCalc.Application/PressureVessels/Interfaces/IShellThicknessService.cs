using EngCalc.Application.PressureVessels.DTOs;

namespace EngCalc.Application.PressureVessels.Interfaces;

public interface IShellThicknessService
{
    ShellThicknessOptionsDto GetOptions();
    Task<ShellThicknessResponseDto> CalculateAsync(ShellThicknessRequestDto request, CancellationToken cancellationToken = default);
}

