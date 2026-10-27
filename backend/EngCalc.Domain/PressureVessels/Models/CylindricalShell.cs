namespace EngCalc.Domain.PressureVessels;

using System;
using EngCalc.Domain.Constants;
using EngCalc.Domain.Common;

using UnitsNet;
using UnitsNet.Units;

public class CylindricalShell
{
    private readonly Pressure _designPressure;
    private readonly Length _nominalThickness;
    private readonly Length _internalDiameter;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;

    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;
    private Length InternalRadius => _internalDiameter / 2;
    private Length CorrodedThickness => _nominalThickness - _allowableCorrosion;

    public CylindricalShell(
        Pressure designPressure, 
        Length internalDiameter, 
        Pressure allowableStress, 
        Length allowableCorrosion, 
        double jointEfficiency, 
        Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(designPressure, nameof(designPressure));
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        _designPressure = designPressure;
        _internalDiameter = internalDiameter;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;
    }
    
    public CylindricalShell(
        Pressure designPressure, 
        Length internalDiameter, 
        Pressure allowableStress, 
        Length allowableCorrosion, 
        double jointEfficiency, 
        Density densityOfFluid, 
        Length heightOfFluidColumn)
        : this(
            designPressure, 
            internalDiameter, 
            allowableStress, 
            allowableCorrosion, 
            jointEfficiency, 
            Pressure.FromPascals(densityOfFluid.KilogramsPerCubicMeter * heightOfFluidColumn.Meters * PhysicalConstants.StandardGravity.MetersPerSecondSquared))
    {
    }

    public CylindricalShell(
        Length nominalThickness,
        Length internalDiameter,
        Pressure allowableStress,
        Length allowableCorrosion,
        double jointEfficiency,
        Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(nominalThickness, nameof(nominalThickness));
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (nominalThickness <= allowableCorrosion)
            throw new ArgumentOutOfRangeException(nameof(nominalThickness), "A espessura nominal (t) deve ser estritamente maior que a sobreespessura de corrosão (CA).");

        _nominalThickness = nominalThickness;
        _internalDiameter = internalDiameter;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;
    }

    public CylindricalShell(
        Length nominalThickness,
        Length internalDiameter,
        Pressure allowableStress,
        Length allowableCorrosion,
        double jointEfficiency,
        Density densityOfFluid,
        Length heightOfFluidColumn)
        : this(
            nominalThickness,
            internalDiameter,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            Pressure.FromPascals(densityOfFluid.KilogramsPerCubicMeter * heightOfFluidColumn.Meters * PhysicalConstants.StandardGravity.MetersPerSecondSquared))
    {
    }

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, UG-27
    {
        var (circumferentialThickness, longitudinalThickness, _, _) = CalculateDetailedThicknesses();

        double maxRequiredThicknessMm = Math.Max(circumferentialThickness.Millimeters, longitudinalThickness.Millimeters);

        return Length.FromMillimeters(maxRequiredThicknessMm) + _allowableCorrosion;
    }

    public (Length CircumferentialThickness, Length LongitudinalThickness, bool IsCircumferentialThick, bool IsLongitudinalThick) CalculateDetailedThicknesses()
    {
        bool isCircThick = IsThickShellByPressureLimit(EffectivePressure, StressType.Circumferential);
        bool isLongThick = IsThickShellByPressureLimit(EffectivePressure, StressType.Longitudinal);

        Length circumferentialThickness = CalculateThicknessForStressType(StressType.Circumferential);
        Length longitudinalThickness = CalculateThicknessForStressType(StressType.Longitudinal);

        double maxRequiredThicknessMm = Math.Max(circumferentialThickness.Millimeters, longitudinalThickness.Millimeters);

        if (maxRequiredThicknessMm > InternalRadius.Millimeters / 2.0)
        {
            isCircThick = true;
            isLongThick = true;
            circumferentialThickness = new CircumferentialThickShellCalculator().CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);
            longitudinalThickness = new LongitudinalThickShellCalculator().CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);
        }

        return (circumferentialThickness, longitudinalThickness, isCircThick, isLongThick);
    }

    public Pressure CalculateMaximumAllowableWorkingPressure() // ASME VIII Div. 1, UG-27 / Appendix 1-2
    {
        var (circumferentialMawp, longitudinalMawp, _, _) = CalculateDetailedMAWP();

        double minMawpMpa = Math.Min(circumferentialMawp.Megapascals, longitudinalMawp.Megapascals);

        return Pressure.FromMegapascals(minMawpMpa);
    }

    public Pressure CalculateMAWP() => CalculateMaximumAllowableWorkingPressure();

    public (Pressure CircumferentialMawp, Pressure LongitudinalMawp, bool IsCircumferentialThick, bool IsLongitudinalThick) CalculateDetailedMAWP()
    {
        Length corrodedThickness = CorrodedThickness;
        if (corrodedThickness <= Length.Zero)
        {
            throw new InvalidOperationException(
                "Corroded thickness (nominal thickness minus corrosion allowance) must be strictly greater than zero.");
        }

        bool isThickByThickness = corrodedThickness.Millimeters > InternalRadius.Millimeters / 2.0;
        bool isCircThick = isThickByThickness;
        bool isLongThick = isThickByThickness;

        Pressure circEffectivePressure = isCircThick
            ? new CircumferentialThickShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency)
            : new CircumferentialThinShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency);

        if (!isCircThick && IsThickShellByPressureLimit(circEffectivePressure, StressType.Circumferential))
        {
            isCircThick = true;
            circEffectivePressure = new CircumferentialThickShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency);
        }

        Pressure longEffectivePressure = isLongThick
            ? new LongitudinalThickShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency)
            : new LongitudinalThinShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency);

        if (!isLongThick && IsThickShellByPressureLimit(longEffectivePressure, StressType.Longitudinal))
        {
            isLongThick = true;
            longEffectivePressure = new LongitudinalThickShellCalculator().CalculateMAWP(corrodedThickness, InternalRadius, _allowableStress, _jointEfficiency);
        }

        Pressure circumferentialMawp = circEffectivePressure - _fluidColumnPressure;
        Pressure longitudinalMawp = longEffectivePressure - _fluidColumnPressure;

        if (circumferentialMawp <= Pressure.Zero || longitudinalMawp <= Pressure.Zero)
        {
            throw new InvalidOperationException(
                "Calculated Maximum Allowable Working Pressure (MAWP) cannot sustain the static fluid column pressure.");
        }

        return (circumferentialMawp, longitudinalMawp, isCircThick, isLongThick);
    }

    private Length CalculateThicknessForStressType(StressType type)
    {
        IShellThicknessCalculator calculator = type switch
        {
            StressType.Circumferential => IsThickShellByPressureLimit(EffectivePressure, StressType.Circumferential) 
                ? new CircumferentialThickShellCalculator() 
                : new CircumferentialThinShellCalculator(),
                
            StressType.Longitudinal => IsThickShellByPressureLimit(EffectivePressure, StressType.Longitudinal) 
                ? new LongitudinalThickShellCalculator() 
                : new LongitudinalThinShellCalculator(),
                
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        return calculator.CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);
    }
        
    private bool IsThickShellByPressureLimit(Pressure pressure, StressType type) 
    {
        double limitFactor = type == StressType.Circumferential ? 0.385 : 1.25;
        return pressure > limitFactor * _allowableStress * _jointEfficiency;
    }
}
