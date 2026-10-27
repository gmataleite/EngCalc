namespace EngCalc.Application.PressureVessels.DTOs;

public record HeadThicknessResponseDto(
    string UnitSystem,
    string HeadType,
    string ActiveLogic,
    string LengthUnit,
    string PressureUnit,
    string TemperatureUnit,
    double JointEfficiency,
    double JointEfficiencyPercent,
    double AllowableStress,
    bool IsManualAllowableStress,
    double RequiredThicknessWithoutCa,
    double CorrosionAllowance,
    double TotalMinimumThickness,
    string ReferenceLatex,
    string Breakdown,
    IReadOnlyList<string> Warnings,
    string Mode = "min_thickness",
    double? CorrodedThickness = null,
    double? MaximumAllowableWorkingPressure = null
);
