using EngCalc.Application.PressureVessels.DTOs;

namespace EngCalc.Application.PressureVessels.Interfaces;

public interface IHeadThicknessService
{
    HeadThicknessOptionsDto GetOptions();
    Task<HeadThicknessResponseDto> CalculateAsync(HeadThicknessRequestDto request, CancellationToken cancellationToken = default);
}

