using System.Globalization;
using UnitsNet;
using VesselCalc.Application.DTOs;
using VesselCalc.Application.Interfaces;
using VesselCalc.Domain;
using VesselCalc.Domain.Entities;

namespace VesselCalc.Application.Services;

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

    public async Task<IReadOnlyList<MaterialOptionDto>> GetMaterialsAsync(CancellationToken cancellationToken = default)
    {
        var materials = await _materialProvider.GetAllMaterialsAsync(cancellationToken);

        return materials
            .OrderBy(m => m.MaterialId)
            .Select(FormatMaterialOption)
            .ToList();
    }

    public ShellThicknessOptionsDto GetOptions()
    {
        return new ShellThicknessOptionsDto(JointTypeOptions, RadiographicOptions);
    }

    public async Task<ShellThicknessResponseDto> CalculateAsync(
        ShellThicknessRequestDto request,
        CancellationToken cancellationToken = default)
    {
        bool isImperial = string.Equals(request.UnitSystem, "imperial", StringComparison.OrdinalIgnoreCase);
        string unitSystem = isImperial ? "imperial" : "metric";
        string lengthUnit = isImperial ? "in" : "mm";
        string pressureUnit = isImperial ? "psi" : "MPa";
        string temperatureUnit = isImperial ? "°F" : "°C";

        if (!string.Equals(request.ShellType, "cylindrical", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Spherical shell calculations are planned on the roadmap and not yet available.");
        }

        if (!string.Equals(request.Mode, "min_thickness", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Solve for Internal Design Pressure (MAWP) is planned on the roadmap and not yet available.");
        }

        if (request.InternalPressure is null || !double.IsFinite(request.InternalPressure.Value) || request.InternalPressure <= 0 || request.InternalPressure > 100_000)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InternalPressure), "Internal pressure must be a finite value greater than zero.");
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

        // 3. Convert geometric and pressure inputs to Domain UnitsNet types
        Pressure designPressure = isImperial
            ? Pressure.FromPoundsForcePerSquareInch(request.InternalPressure.Value)
            : Pressure.FromMegapascals(request.InternalPressure.Value);

        Length insideRadius = isImperial
            ? Length.FromInches(request.InsideRadius)
            : Length.FromMillimeters(request.InsideRadius);

        Length internalDiameter = insideRadius * 2.0;

        Length corrosionAllowance = isImperial
            ? Length.FromInches(request.CorrosionAllowance)
            : Length.FromMillimeters(request.CorrosionAllowance);

        // 4. Execute Domain Calculation
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

        // 5. Convert output values to requested unit system
        double circVal = isImperial ? circThickness.Inches : circThickness.Millimeters;
        double longVal = isImperial ? longThickness.Inches : longThickness.Millimeters;
        double governingVal = Math.Max(circVal, longVal);
        double caVal = isImperial ? corrosionAllowance.Inches : corrosionAllowance.Millimeters;
        double totalVal = isImperial ? totalMinThickness.Inches : totalMinThickness.Millimeters;
        double stressVal = isImperial ? allowableStress.PoundsForcePerSquareInch : allowableStress.Megapascals;

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
            Warnings: warnings);
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

    private static MaterialOptionDto FormatMaterialOption(Material m)
    {
        var gradePart = !string.IsNullOrWhiteSpace(m.Grade) ? $" Gr. {m.Grade}" : string.Empty;
        var formPart = !string.IsNullOrWhiteSpace(m.ProductForm) ? $" — Carbon/Alloy Steel {m.ProductForm}" : string.Empty;
        var label = $"ASTM/ASME {m.Specification}{gradePart}{formPart}";

        return new MaterialOptionDto(
            Id: m.MaterialId,
            Label: label,
            Specification: m.Specification,
            Grade: m.Grade,
            ProductForm: m.ProductForm,
            MaxTemperatureLimit: m.MaxTemperatureLimit);
    }
}

