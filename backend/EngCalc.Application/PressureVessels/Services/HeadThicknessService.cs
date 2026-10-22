using System.Globalization;
using UnitsNet;
using EngCalc.Application.Common.DTOs;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.PressureVessels.DTOs;
using EngCalc.Application.PressureVessels.Interfaces;
using EngCalc.Domain.PressureVessels;

namespace EngCalc.Application.PressureVessels.Services;

public class HeadThicknessService : IHeadThicknessService
{
    private readonly IMaterialProvider _materialProvider;

    private static readonly IReadOnlyList<SelectOptionDto> HeadTypeOptions =
    [
        new("ellipsoidal", "Ellipsoidal"),
        new("torispherical", "Torispherical"),
        new("hemispherical", "Hemispherical")
    ];

    private static readonly IReadOnlyList<SelectOptionDto> JointTypeOptions =
    [
        new("type_1", "Butt Joints attained by Double Welding per UW-35"),
        new("type_2", "Single-Welded Butt Joint with Backing Strip"),
        new("type_3", "Single-Welded Butt Joint without Backing Strip"),
        new("seamless", "Seamless Head (UW-11)")
    ];

    private static readonly IReadOnlyList<SelectOptionDto> RadiographicOptions =
    [
        new("full", "Full"),
        new("spot", "Spot"),
        new("none", "None")
    ];

    public HeadThicknessService(IMaterialProvider materialProvider)
    {
        _materialProvider = materialProvider;
    }

    public HeadThicknessOptionsDto GetOptions()
    {
        return new HeadThicknessOptionsDto(HeadTypeOptions, JointTypeOptions, RadiographicOptions);
    }

    public async Task<HeadThicknessResponseDto> CalculateAsync(
        HeadThicknessRequestDto request,
        CancellationToken cancellationToken = default)
    {
        bool isImperial = string.Equals(request.UnitSystem, "imperial", StringComparison.OrdinalIgnoreCase);
        string unitSystem = isImperial ? "imperial" : "metric";
        string lengthUnit = isImperial ? "inch" : "mm";
        string pressureUnit = isImperial ? "psi" : "MPa";
        string temperatureUnit = isImperial ? "°F" : "°C";

        string headType = (request.HeadType ?? "ellipsoidal").ToLowerInvariant();
        if (headType == "conical")
        {
            throw new NotSupportedException("Conical head calculations are planned on the roadmap and not yet available.");
        }

        if (!string.Equals(request.Mode, "min_thickness", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Calculate Maximum Allowable Working Pressure (P) is planned on the roadmap and not yet available.");
        }

        if (request.InternalPressure is null || request.InternalPressure <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InternalPressure), "Internal design pressure must be greater than zero.");
        }

        if (request.CorrosionAllowance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.CorrosionAllowance), "Corrosion allowance cannot be negative.");
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

        // 3. Convert pressure and corrosion allowance to Domain UnitsNet types
        Pressure designPressure = isImperial
            ? Pressure.FromPoundsForcePerSquareInch(request.InternalPressure.Value)
            : Pressure.FromMegapascals(request.InternalPressure.Value);

        Length corrosionAllowance = isImperial
            ? Length.FromInches(request.CorrosionAllowance)
            : Length.FromMillimeters(request.CorrosionAllowance);

        // 4. Execute Domain Calculation by Head Type
        Length totalMinThickness;
        string activeLogic;
        string referenceLatex;
        string headLabel;

        switch (headType)
        {
            case "ellipsoidal":
            {
                if (request.InsideDiameter is null || request.InsideDiameter <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.InsideDiameter), "Inside diameter of head skirt (D) must be greater than zero.");
                }

                Length insideDiameter = isImperial
                    ? Length.FromInches(request.InsideDiameter.Value)
                    : Length.FromMillimeters(request.InsideDiameter.Value);

                Length? headHeight = null;
                if (request.HeadHeight is > 0)
                {
                    headHeight = isImperial
                        ? Length.FromInches(request.HeadHeight.Value)
                        : Length.FromMillimeters(request.HeadHeight.Value);
                }

                var head = new EllipsoidalHead(
                    designPressure: designPressure,
                    internalDiameter: insideDiameter,
                    allowableStress: allowableStress,
                    allowableCorrosion: corrosionAllowance,
                    jointEfficiency: jointEfficiency,
                    fluidColumnPressure: Pressure.Zero,
                    headHeight: headHeight);

                totalMinThickness = head.CalculateMinimumRequiredThickness();
                activeLogic = headHeight.HasValue
                    ? "Ellipsoidal Heads — Custom Ratio (Appendix 1-4(c))"
                    : "Ellipsoidal Heads (UG-32(c))";
                referenceLatex = headHeight.HasValue
                    ? @"t = \frac{P \cdot D \cdot K}{2SE - 0.2P} + CA"
                    : @"t = \frac{P \cdot D}{2SE - 0.2P} + CA";
                headLabel = "Ellipsoidal";
                break;
            }

            case "torispherical":
            {
                if (request.CrownRadius is null || request.CrownRadius <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.CrownRadius), "Inside crown radius (L) must be greater than zero.");
                }

                if (request.KnuckleRadius is null || request.KnuckleRadius <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.KnuckleRadius), "Inside knuckle radius (r) must be greater than zero.");
                }

                Length crownRadius = isImperial
                    ? Length.FromInches(request.CrownRadius.Value)
                    : Length.FromMillimeters(request.CrownRadius.Value);

                Length knuckleRadius = isImperial
                    ? Length.FromInches(request.KnuckleRadius.Value)
                    : Length.FromMillimeters(request.KnuckleRadius.Value);

                try
                {
                    var head = new TorisphericalHead(
                        designPressure: designPressure,
                        crownRadius: crownRadius,
                        knuckleRadius: knuckleRadius,
                        allowableStress: allowableStress,
                        allowableCorrosion: corrosionAllowance,
                        jointEfficiency: jointEfficiency,
                        fluidColumnPressure: Pressure.Zero);

                    totalMinThickness = head.CalculateMinimumRequiredThickness();
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(request.KnuckleRadius),
                        "Knuckle radius (r) must be between 6% and 50% of the crown radius (L) per ASME UG-32(d) / Appendix 1-4(d).");
                }

                activeLogic = "Torispherical Heads (UG-32(d) / Appendix 1-4(d))";
                referenceLatex = @"t = \frac{P \cdot L \cdot M}{2SE - 0.2P} + CA";
                headLabel = "Torispherical";
                break;
            }

            case "hemispherical":
            {
                if (request.InsideRadius is null || request.InsideRadius <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.InsideRadius), "Inside spherical radius (L) must be greater than zero.");
                }

                Length insideRadius = isImperial
                    ? Length.FromInches(request.InsideRadius.Value)
                    : Length.FromMillimeters(request.InsideRadius.Value);

                var head = new HemisphericalHead(
                    designPressure: designPressure,
                    internalRadius: insideRadius,
                    allowableStress: allowableStress,
                    allowableCorrosion: corrosionAllowance,
                    jointEfficiency: jointEfficiency,
                    fluidColumnPressure: Pressure.Zero);

                totalMinThickness = head.CalculateMinimumRequiredThickness();
                activeLogic = "Hemispherical Heads (UG-32(e) / Appendix 1-3)";
                referenceLatex = @"t = \frac{P \cdot L}{2SE - 0.2P} + CA";
                headLabel = "Hemispherical";
                break;
            }

            default:
                throw new NotSupportedException($"Unsupported head type '{request.HeadType}'.");
        }

        Length requiredWithoutCa = totalMinThickness - corrosionAllowance;

        double totalVal = isImperial ? totalMinThickness.Inches : totalMinThickness.Millimeters;
        double reqVal = isImperial ? requiredWithoutCa.Inches : requiredWithoutCa.Millimeters;
        double caVal = isImperial ? corrosionAllowance.Inches : corrosionAllowance.Millimeters;
        double stressVal = isImperial ? allowableStress.PoundsForcePerSquareInch : allowableStress.Megapascals;

        string breakdown = string.Create(
            CultureInfo.InvariantCulture,
            $"Breakdown ({headLabel}): Structural thickness required {reqVal:F4} {lengthUnit} ({activeLogic}). Adding CA ({caVal:F4} {lengthUnit}) yields {totalVal:F4} {lengthUnit}.");

        return new HeadThicknessResponseDto(
            UnitSystem: unitSystem,
            HeadType: headType,
            activeLogic,
            LengthUnit: lengthUnit,
            PressureUnit: pressureUnit,
            TemperatureUnit: temperatureUnit,
            JointEfficiency: jointEfficiency,
            JointEfficiencyPercent: Math.Round(jointEfficiency * 100.0, 2),
            AllowableStress: Math.Round(stressVal, 4),
            IsManualAllowableStress: isManualOverride,
            RequiredThicknessWithoutCa: Math.Round(reqVal, 4),
            CorrosionAllowance: Math.Round(caVal, 4),
            TotalMinimumThickness: Math.Round(totalVal, 4),
            ReferenceLatex: referenceLatex,
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
            ("seamless", "none") => (0.85, "Per ASME UW-11(a)(5)(b), E = 0.85 is applied for seamless heads when connecting welds receive no radiography."),

            _ => (1.00, null)
        };
    }
}

