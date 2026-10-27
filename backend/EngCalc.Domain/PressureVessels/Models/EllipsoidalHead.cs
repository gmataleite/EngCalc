namespace EngCalc.Domain.PressureVessels;

using System;
using EngCalc.Domain.Common;
using UnitsNet;

public class EllipsoidalHead
{
    private readonly Pressure _designPressure;
    private readonly Length _nominalThickness;
    private readonly Length _internalDiameter;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;
    private readonly double _factorK;
    private readonly Length _headHeight;
    
    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;
    private Length CorrodedThickness => _nominalThickness - _allowableCorrosion;

    public EllipsoidalHead(Pressure designPressure, Length internalDiameter, Pressure allowableStress, Length allowableCorrosion, double jointEfficiency, Pressure fluidColumnPressure, Length? headHeight = null, double? factorK = null)
    {
        Guard.AgainstNegativeOrZero(designPressure, nameof(designPressure));
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (headHeight.HasValue) 
            Guard.AgainstNegativeOrZero(headHeight, nameof(headHeight));

        if (factorK.HasValue)
            Guard.AgainstNegativeOrZero(factorK.Value, nameof(factorK));

        _designPressure = designPressure;
        _internalDiameter = internalDiameter;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;

        _headHeight = headHeight ?? (internalDiameter / 4.0);
        
        if (factorK.HasValue && headHeight.HasValue)
            factorK = null; 

        _factorK = factorK ?? CalculateFactorK(_headHeight, _internalDiameter);
    }

    public EllipsoidalHead(Length nominalThickness, Length internalDiameter, Pressure allowableStress, Length allowableCorrosion, double jointEfficiency, Pressure fluidColumnPressure, Length? headHeight = null, double? factorK = null)
    {
        Guard.AgainstNegativeOrZero(nominalThickness, nameof(nominalThickness));
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (nominalThickness <= allowableCorrosion)
            throw new ArgumentOutOfRangeException(nameof(nominalThickness), "A espessura nominal (t) deve ser estritamente maior que a sobreespessura de corrosão (CA).");

        if (headHeight.HasValue)
            Guard.AgainstNegativeOrZero(headHeight, nameof(headHeight));

        if (factorK.HasValue)
            Guard.AgainstNegativeOrZero(factorK.Value, nameof(factorK));

        _nominalThickness = nominalThickness;
        _internalDiameter = internalDiameter;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;

        _headHeight = headHeight ?? (internalDiameter / 4.0);

        if (factorK.HasValue && headHeight.HasValue)
            factorK = null;

        _factorK = factorK ?? CalculateFactorK(_headHeight, _internalDiameter);
    }

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, Appendix 1
    {
        double P = EffectivePressure.Megapascals;
        double D = _internalDiameter.Millimeters;
        double S = _allowableStress.Megapascals;
        double E = _jointEfficiency;
        double K = _factorK;

        double denominator = 2.0 * S * E - 0.2 * P;
        if (denominator <= 0 || P >= S * E)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) exceeds the allowable membrane stress limit (S·E) for ellipsoidal heads.");
        }

        double thickness = P * D * K / denominator;
        
        return Length.FromMillimeters(thickness) + _allowableCorrosion;
    }

    public Pressure CalculateMaximumAllowableWorkingPressure() // ASME VIII Div. 1, UG-32(c) / Appendix 1-4(c)
    {
        Length corrodedThickness = CorrodedThickness;
        if (corrodedThickness <= Length.Zero)
        {
            throw new InvalidOperationException(
                "Corroded thickness (nominal thickness minus corrosion allowance) must be strictly greater than zero.");
        }

        double t = corrodedThickness.Millimeters;
        double D = _internalDiameter.Millimeters;
        double S = _allowableStress.Megapascals;
        double E = _jointEfficiency;
        double K = _factorK;

        double denominator = D * K + 0.2 * t;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Invalid geometry for ellipsoidal head MAWP calculation.");
        }

        double effectivePressureMpa = (2.0 * S * E * t) / denominator;
        Pressure mawp = Pressure.FromMegapascals(effectivePressureMpa) - _fluidColumnPressure;

        if (mawp <= Pressure.Zero)
        {
            throw new InvalidOperationException(
                "Calculated Maximum Allowable Working Pressure (MAWP) cannot sustain the static fluid column pressure.");
        }

        return mawp;
    }

    public Pressure CalculateMAWP() => CalculateMaximumAllowableWorkingPressure();

    private double CalculateFactorK(Length headHeight, Length internalDiameter) 
    {
        double D = internalDiameter.Millimeters;
        double H = headHeight.Millimeters;
        
        return (2.0 + Math.Pow(D / (2.0 * H), 2)) / 6;
    }
}
