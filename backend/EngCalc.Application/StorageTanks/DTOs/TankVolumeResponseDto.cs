namespace EngCalc.Application.StorageTanks.DTOs;

public record TankVolumeResponseDto(
        double InternalVolume, 
        double ProductVolume
);