using EngCalc.Application.StorageTanks.DTOs;

namespace EngCalc.Application.StorageTanks.Interfaces;

public interface ITankVolumeService
{
    TankOptionsDto GetOptions();
    Task<TankVolumeResponseDto> CalculateVolumeAsync(TankVolumeRequestDto request);
}

