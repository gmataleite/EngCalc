using VesselCalc.Application.DTOs;

namespace VesselCalc.Application.Interfaces;

public interface IHeadThicknessService
{
    HeadThicknessOptionsDto GetOptions();
    Task<HeadThicknessResponseDto> CalculateAsync(HeadThicknessRequestDto request, CancellationToken cancellationToken = default);
}

