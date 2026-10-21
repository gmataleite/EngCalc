namespace VesselCalc.Application.DTOs;

public record HeadThicknessRequestDto(
    string UnitSystem,
    string HeadType,
    string Mode,
    string MaterialId,
    double DesignTemperature,
    double? InternalPressure,
    double? InsideDiameter,
    double? HeadHeight,
    double? CrownRadius,
    double? KnuckleRadius,
    double? InsideRadius,
    string JointTypeId,
    string RtId,
    double CorrosionAllowance,
    double? AllowableStressOverride
);

