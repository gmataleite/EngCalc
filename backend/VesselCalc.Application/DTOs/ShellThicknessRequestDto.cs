namespace VesselCalc.Application.DTOs;

public record ShellThicknessRequestDto(
    string UnitSystem,
    string ShellType,
    string Mode,
    string MaterialId,
    double DesignTemperature,
    double InsideRadius,
    double? InternalPressure,
    double? Thickness,
    string JointTypeId,
    string RtId,
    double CorrosionAllowance,
    double? AllowableStressOverride
);

