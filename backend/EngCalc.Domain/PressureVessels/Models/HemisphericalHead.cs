namespace EngCalc.Domain.PressureVessels;

using System;
using EngCalc.Domain.Common;
using UnitsNet;

public class HemisphericalHead
{
    private readonly Pressure _designPressure;
    private readonly Length _nominalThickness;
    private readonly Length _internalRadius;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;
    
    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;
    private Length CorrodedThickness => _nominalThickness - _allowableCorrosion;

    public HemisphericalHead(Pressure designPressure, Length internalRadius, Pressure allowableStress, Length allowableCorrosion, double jointEfficiency, Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(designPressure, nameof(designPressure));
        Guard.AgainstNegativeOrZero(internalRadius, nameof(internalRadius));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        _designPressure = designPressure;
        _internalRadius = internalRadius;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;
    }

    public HemisphericalHead(Length nominalThickness, Length internalRadius, Pressure allowableStress, Length allowableCorrosion, double jointEfficiency, Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(nominalThickness, nameof(nominalThickness));
        Guard.AgainstNegativeOrZero(internalRadius, nameof(internalRadius));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (nominalThickness <= allowableCorrosion)
            throw new ArgumentOutOfRangeException(nameof(nominalThickness), "A espessura nominal (t) deve ser estritamente maior que a sobreespessura de corrosão (CA).");

        _nominalThickness = nominalThickness;
        _internalRadius = internalRadius;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;
    }

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, UG-32(e)
    {
        var (totalThickness, _) = CalculateDetailedThickness();
        return totalThickness;
    }

    public (Length TotalMinimumThickness, bool IsThickPlate) CalculateDetailedThickness()
    {
        bool isThick = IsThickShellByPressureLimit(EffectivePressure);
        Length thickness = isThick
            ? new HemisphericalHeadThickPlateCalculator().CalculateThickness(EffectivePressure, _internalRadius, _allowableStress, _jointEfficiency)
            : new HemisphericalHeadThinPlateCalculator().CalculateThickness(EffectivePressure, _internalRadius, _allowableStress, _jointEfficiency);

        if (!isThick && IsThickShellByThickness(thickness))
        {
            isThick = true;
            thickness = new HemisphericalHeadThickPlateCalculator().CalculateThickness(EffectivePressure, _internalRadius, _allowableStress, _jointEfficiency);
        }

        return (thickness + _allowableCorrosion, isThick);
    }

    public Pressure CalculateMaximumAllowableWorkingPressure() // ASME VIII Div. 1, UG-32(e) / Appendix 1-3
    {
        var (mawp, _) = CalculateDetailedMAWP();
        return mawp;
    }

    public Pressure CalculateMAWP() => CalculateMaximumAllowableWorkingPressure();

    public (Pressure Mawp, bool IsThickPlate) CalculateDetailedMAWP()
    {
        Length corrodedThickness = CorrodedThickness;
        if (corrodedThickness <= Length.Zero)
        {
            throw new InvalidOperationException(
                "Corroded thickness (nominal thickness minus corrosion allowance) must be strictly greater than zero.");
        }

        bool isThick = IsThickShellByThickness(corrodedThickness);
        Pressure effectivePressure = isThick
            ? new HemisphericalHeadThickPlateCalculator().CalculateMAWP(corrodedThickness, _internalRadius, _allowableStress, _jointEfficiency)
            : new HemisphericalHeadThinPlateCalculator().CalculateMAWP(corrodedThickness, _internalRadius, _allowableStress, _jointEfficiency);

        if (!isThick && IsThickShellByPressureLimit(effectivePressure))
        {
            isThick = true;
            effectivePressure = new HemisphericalHeadThickPlateCalculator().CalculateMAWP(corrodedThickness, _internalRadius, _allowableStress, _jointEfficiency);
        }

        Pressure mawp = effectivePressure - _fluidColumnPressure;
        if (mawp <= Pressure.Zero)
        {
            throw new InvalidOperationException(
                "Calculated Maximum Allowable Working Pressure (MAWP) cannot sustain the static fluid column pressure.");
        }

        return (mawp, isThick);
    }

    private bool IsThickShellByPressureLimit(Pressure pressure) 
    {
        double limitFactor = 0.665;
        return pressure > limitFactor * _allowableStress * _jointEfficiency;
    }

    private bool IsThickShellByThickness(Length thickness) 
    {
        double limitFactor = 0.356;
        return thickness > limitFactor * _internalRadius;
    }
}
