namespace EngCalc.Application.StorageTanks.DTOs;

public record TankVolumeRequestDto(
        double InternalDiameter, 
        double InternalBodyHeight, 
        string BottomType, 
        double? BottomHeight, 
        double? BottomAngle, 
        string HeadType, 
        double? HeadHeight, 
        double? HeadAngle,
        double? ProductHeight,
        double? Orientation
);