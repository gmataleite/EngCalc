using EngCalc.Application.StorageTanks.DTOs;

namespace EngCalc.Application.StorageTanks.Interfaces;

public interface ITankVolumeService
{
    TankHeadOptionsDto GetOptions();
    Task<TankVolumeResponseDto> CalculateVolumeAsync(TankVolumeRequestDto request);
}

