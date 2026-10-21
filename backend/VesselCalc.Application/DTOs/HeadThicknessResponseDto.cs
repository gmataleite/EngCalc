namespace VesselCalc.Application.DTOs;

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
    IReadOnlyList<string> Warnings
);

