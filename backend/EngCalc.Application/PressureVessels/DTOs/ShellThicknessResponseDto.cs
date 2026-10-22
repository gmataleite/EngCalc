namespace EngCalc.Application.PressureVessels.DTOs;

public record ShellThicknessResponseDto(
    string UnitSystem,
    string LengthUnit,
    string PressureUnit,
    string TemperatureUnit,
    double JointEfficiency,
    double JointEfficiencyPercent,
    double AllowableStress,
    bool IsManualAllowableStress,
    double CircumferentialThickness,
    double LongitudinalThickness,
    double GoverningRequiredThickness,
    double CorrosionAllowance,
    double TotalMinimumThickness,
    string CircumferentialFormulaType,
    string LongitudinalFormulaType,
    string Breakdown,
    IReadOnlyList<string> Warnings
);

