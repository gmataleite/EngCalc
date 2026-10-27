using System.Globalization;
using UnitsNet;
using EngCalc.Application.Common.DTOs;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.PressureVessels.DTOs;
using EngCalc.Application.PressureVessels.Interfaces;
using EngCalc.Domain.PressureVessels;

namespace EngCalc.Application.PressureVessels.Services;

public class ShellThicknessService : IShellThicknessService
{
    private readonly IMaterialProvider _materialProvider;

    private static readonly IReadOnlyList<SelectOptionDto> JointTypeOptions =
    [
        new("type_1", "Butt Joints attained by Double Welding per UW-35"),
        new("type_2", "Single-Welded Butt Joint with Backing Strip"),
        new("type_3", "Single-Welded Butt Joint without Backing Strip"),
        new("seamless", "Seamless Shell (No Longitudinal Weld Joint, UW-11)")
    ];

    private static readonly IReadOnlyList<SelectOptionDto> RadiographicOptions =
    [
        new("full", "Full"),
        new("spot", "Spot"),
        new("none", "None")
    ];

    public ShellThicknessService(IMaterialProvider materialProvider)
    {
        _materialProvider = materialProvider;
    }

    public ShellThicknessOptionsDto GetOptions()
    {
        return new ShellThicknessOptionsDto(JointTypeOptions, RadiographicOptions);
    }

    public async Task<ShellThicknessResponseDto> CalculateAsync(
        ShellThicknessRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        bool isImperial = string.Equals(request.UnitSystem, "imperial", StringComparison.OrdinalIgnoreCase);
        string unitSystem = isImperial ? "imperial" : "metric";
        string lengthUnit = isImperial ? "in" : "mm";
        string pressureUnit = isImperial ? "psi" : "MPa";
        string temperatureUnit = isImperial ? "°F" : "°C";

        if (!string.Equals(request.ShellType, "cylindrical", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Spherical shell calculations are planned on the roadmap and not yet available.");
        }

        string mode = (request.Mode ?? "min_thickness").Trim().ToLowerInvariant();
        bool isMawpMode = mode == "mawp";
        if (mode != "min_thickness" && !isMawpMode)
        {
            throw new NotSupportedException($"Unsupported calculation mode '{request.Mode}'.");
        }

        if (!double.IsFinite(request.InsideRadius) || request.InsideRadius <= 0 || request.InsideRadius > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InsideRadius), "Inside radius must be a finite value greater than zero.");
        }

        if (!double.IsFinite(request.CorrosionAllowance) || request.CorrosionAllowance < 0 || request.CorrosionAllowance > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(request.CorrosionAllowance), "Corrosion allowance must be a finite non-negative value.");
        }

        if (!double.IsFinite(request.DesignTemperature) || request.DesignTemperature < -460 || request.DesignTemperature > 5_000)
        {
            throw new ArgumentOutOfRangeException(nameof(request.DesignTemperature), "Design temperature is outside the valid physical range.");
        }

        if (request.AllowableStressOverride is not null && (!double.IsFinite(request.AllowableStressOverride.Value) || request.AllowableStressOverride.Value > 1_000_000))
        {
            throw new ArgumentOutOfRangeException(nameof(request.AllowableStressOverride), "Allowable stress override must be a finite value within a valid physical range.");
        }

        if (isMawpMode)
        {
            if (request.Thickness is null || !double.IsFinite(request.Thickness.Value) || request.Thickness <= 0 || request.Thickness > 100_000)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Thickness), "Nominal shell thickness must be a finite value greater than zero.");
            }

            if (request.Thickness.Value <= request.CorrosionAllowance)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Thickness), "Nominal shell thickness (t) must be strictly greater than the corrosion allowance (CA).");
            }
        }
        else
        {
            if (request.InternalPressure is null || !double.IsFinite(request.InternalPressure.Value) || request.InternalPressure <= 0 || request.InternalPressure > 100_000)
            {
                throw new ArgumentOutOfRangeException(nameof(request.InternalPressure), "Internal pressure must be a finite value greater than zero.");
            }
        }

        var warnings = new List<string>();

        // 1. Resolve Joint Efficiency (E) per ASME VIII Div. 1 Table UW-12
        var (jointEfficiency, jointWarning) = ResolveJointEfficiency(request.JointTypeId, request.RtId);
        if (jointWarning is not null)
        {
            warnings.Add(jointWarning);
        }

        // 2. Resolve Allowable Stress (S)
        Pressure allowableStress;
        bool isManualOverride = request.AllowableStressOverride is > 0;

        if (isManualOverride)
        {
            allowableStress = isImperial
                ? Pressure.FromPoundsForcePerSquareInch(request.AllowableStressOverride!.Value)
                : Pressure.FromMegapascals(request.AllowableStressOverride!.Value);

            warnings.Add("Manual allowable stress override is active. Ensure value complies with ASME Section II Part D Table 1A.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.MaterialId) || request.MaterialId.Length > 50)
            {
                throw new ArgumentException("A valid MaterialId is required when manual allowable stress override is not active.", nameof(request.MaterialId));
            }

            var material = await _materialProvider.GetMaterialByIdAsync(request.MaterialId, cancellationToken)
                ?? throw new KeyNotFoundException($"Material '{request.MaterialId}' was not found in the database.");

            double temperatureCelsius = isImperial
                ? Temperature.FromDegreesFahrenheit(request.DesignTemperature).DegreesCelsius
                : request.DesignTemperature;

            decimal? interpolatedStressMpa = material.GetAllowableStress((decimal)temperatureCelsius);
            if (interpolatedStressMpa is null)
            {
                throw new InvalidOperationException(
                    $"Design temperature ({request.DesignTemperature.ToString("F2", CultureInfo.InvariantCulture)} {temperatureUnit}) exceeds the allowable temperature range for material {material.MaterialId} (max {material.MaxTemperatureLimit} °C).");
            }

            allowableStress = Pressure.FromMegapascals((double)interpolatedStressMpa.Value);
        }

        // 3. Convert geometric inputs to Domain UnitsNet types
        Length insideRadius = isImperial
            ? Length.FromInches(request.InsideRadius)
            : Length.FromMillimeters(request.InsideRadius);

        Length internalDiameter = insideRadius * 2.0;

        Length corrosionAllowance = isImperial
            ? Length.FromInches(request.CorrosionAllowance)
            : Length.FromMillimeters(request.CorrosionAllowance);

        double stressVal = isImperial ? allowableStress.PoundsForcePerSquareInch : allowableStress.Megapascals;
        double caVal = isImperial ? corrosionAllowance.Inches : corrosionAllowance.Millimeters;

        // 4. Execute Domain Calculation by Mode
        if (isMawpMode)
        {
            Length nominalThickness = isImperial
                ? Length.FromInches(request.Thickness!.Value)
                : Length.FromMillimeters(request.Thickness!.Value);

            var shell = new CylindricalShell(
                nominalThickness: nominalThickness,
                internalDiameter: internalDiameter,
                allowableStress: allowableStress,
                allowableCorrosion: corrosionAllowance,
                jointEfficiency: jointEfficiency,
                fluidColumnPressure: Pressure.Zero);

            var (circMawp, longMawp, isCircThick, isLongThick) = shell.CalculateDetailedMAWP();
            Pressure governingMawp = shell.CalculateMaximumAllowableWorkingPressure();

            if (isCircThick || isLongThick)
            {
                warnings.Add("Thick-wall shell threshold exceeded (P > 0.385·S·E or t > 0.5·R). Mandatory Appendix 1-2 (Lamé) equations were applied.");
            }

            double circMawpVal = isImperial ? circMawp.PoundsForcePerSquareInch : circMawp.Megapascals;
            double longMawpVal = isImperial ? longMawp.PoundsForcePerSquareInch : longMawp.Megapascals;
            double governingMawpVal = isImperial ? governingMawp.PoundsForcePerSquareInch : governingMawp.Megapascals;

            Length corrodedThickness = nominalThickness - corrosionAllowance;
            double corrodedVal = isImperial ? corrodedThickness.Inches : corrodedThickness.Millimeters;
            double nomVal = isImperial ? nominalThickness.Inches : nominalThickness.Millimeters;

            string circFormulaLabel = isCircThick ? "Thick Wall (App. 1-2)" : "Thin Wall (UG-27)";
            string longFormulaLabel = isLongThick ? "Thick Wall (App. 1-2)" : "Thin Wall (UG-27)";

            string breakdown = string.Create(
                CultureInfo.InvariantCulture,
                $"Breakdown (Cylindrical MAWP): Corroded thickness {corrodedVal:F4} {lengthUnit} (Nominal {nomVal:F4} - CA {caVal:F4}). Circ. Stress allows {circMawpVal:F4} {pressureUnit} ({circFormulaLabel}). Long. Stress allows {longMawpVal:F4} {pressureUnit} ({longFormulaLabel}). Min of these yields the governing MAWP.");

            return new ShellThicknessResponseDto(
                UnitSystem: unitSystem,
                LengthUnit: lengthUnit,
                PressureUnit: pressureUnit,
                TemperatureUnit: temperatureUnit,
                JointEfficiency: jointEfficiency,
                JointEfficiencyPercent: Math.Round(jointEfficiency * 100.0, 2),
                AllowableStress: Math.Round(stressVal, 4),
                IsManualAllowableStress: isManualOverride,
                CircumferentialThickness: 0,
                LongitudinalThickness: 0,
                GoverningRequiredThickness: Math.Round(corrodedVal, 4),
                CorrosionAllowance: Math.Round(caVal, 4),
                TotalMinimumThickness: Math.Round(nomVal, 4),
                CircumferentialFormulaType: circFormulaLabel,
                LongitudinalFormulaType: longFormulaLabel,
                Breakdown: breakdown,
                Warnings: warnings,
                Mode: "mawp",
                CircumferentialMawp: Math.Round(circMawpVal, 4),
                LongitudinalMawp: Math.Round(longMawpVal, 4),
                GoverningMawp: Math.Round(governingMawpVal, 4),
                CorrodedThickness: Math.Round(corrodedVal, 4));
        }
        else
        {
            Pressure designPressure = isImperial
                ? Pressure.FromPoundsForcePerSquareInch(request.InternalPressure!.Value)
                : Pressure.FromMegapascals(request.InternalPressure!.Value);

            var shell = new CylindricalShell(
                designPressure: designPressure,
                internalDiameter: internalDiameter,
                allowableStress: allowableStress,
                allowableCorrosion: corrosionAllowance,
                jointEfficiency: jointEfficiency,
                fluidColumnPressure: Pressure.Zero);

            var (circThickness, longThickness, isCircThick, isLongThick) = shell.CalculateDetailedThicknesses();
            Length totalMinThickness = shell.CalculateMinimumRequiredThickness();

            if (isCircThick || isLongThick)
            {
                warnings.Add("Thick-wall shell threshold exceeded (P > 0.385·S·E or t > 0.5·R). Mandatory Appendix 1-2 (Lamé) equations were applied.");
            }

            double circVal = isImperial ? circThickness.Inches : circThickness.Millimeters;
            double longVal = isImperial ? longThickness.Inches : longThickness.Millimeters;
            double governingVal = Math.Max(circVal, longVal);
            double totalVal = isImperial ? totalMinThickness.Inches : totalMinThickness.Millimeters;

            string circFormulaLabel = isCircThick ? "Thick Wall (App. 1-2)" : "Thin Wall (UG-27)";
            string longFormulaLabel = isLongThick ? "Thick Wall (App. 1-2)" : "Thin Wall (UG-27)";

            string breakdown = string.Create(
                CultureInfo.InvariantCulture,
                $"Breakdown (Cylindrical): Circ. Stress required {circVal:F4} {lengthUnit} ({circFormulaLabel}). Long. Stress required {longVal:F4} {lengthUnit} ({longFormulaLabel}). Max of these + CA yields the final result.");

            return new ShellThicknessResponseDto(
                UnitSystem: unitSystem,
                LengthUnit: lengthUnit,
                PressureUnit: pressureUnit,
                TemperatureUnit: temperatureUnit,
                JointEfficiency: jointEfficiency,
                JointEfficiencyPercent: Math.Round(jointEfficiency * 100.0, 2),
                AllowableStress: Math.Round(stressVal, 4),
                IsManualAllowableStress: isManualOverride,
                CircumferentialThickness: Math.Round(circVal, 4),
                LongitudinalThickness: Math.Round(longVal, 4),
                GoverningRequiredThickness: Math.Round(governingVal, 4),
                CorrosionAllowance: Math.Round(caVal, 4),
                TotalMinimumThickness: Math.Round(totalVal, 4),
                CircumferentialFormulaType: circFormulaLabel,
                LongitudinalFormulaType: longFormulaLabel,
                Breakdown: breakdown,
                Warnings: warnings,
                Mode: "min_thickness");
        }
    }

    private static (double Efficiency, string? Warning) ResolveJointEfficiency(string jointTypeId, string rtId)
    {
        string jt = (jointTypeId ?? "type_1").ToLowerInvariant();
        string rt = (rtId ?? "full").ToLowerInvariant();

        return (jt, rt) switch
        {
            ("type_1", "full") => (1.00, null),
            ("type_1", "spot") => (0.85, null),
            ("type_1", "none") => (0.70, null),

            ("type_2", "full") => (0.90, null),
            ("type_2", "spot") => (0.80, null),
            ("type_2", "none") => (0.65, null),

            ("type_3", "none") => (0.60, null),
            ("type_3", _) => (0.60, "Per ASME Table UW-12, Joint Type 3 does not credit Full or Spot radiography; E = 0.60 was used."),

            ("seamless", "full") => (1.00, null),
            ("seamless", "spot") => (1.00, null),
            ("seamless", "none") => (0.85, "Per ASME UW-11(a)(5)(b), E = 0.85 is applied for seamless shells when connecting welds receive no radiography."),

            _ => (1.00, null)
        };
    }
}
