namespace VesselCalc.Domain;

using System;
using VesselCalc.Domain.Common;
using UnitsNet;

public class EllipsoidalHead
{
    private readonly Pressure _designPressure;
    private readonly Length _internalDiameter;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;
    private readonly double _factorK;
    private readonly Length _headHeight;
    
    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;

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

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, Appendix 1
    {
        double P = EffectivePressure.Megapascals;
        double D = _internalDiameter.Millimeters;
        double S = _allowableStress.Megapascals;
        double E = _jointEfficiency;
        double K = _factorK;

        double thickness = P * D * K / (2 * S * E - 0.2 * P);
        
        return Length.FromMillimeters(thickness) + _allowableCorrosion;
    }

    private double CalculateFactorK(Length headHeight, Length internalDiameter) 
    {
        double D = internalDiameter.Millimeters;
        double H = headHeight.Millimeters;
        
        return (2.0 + Math.Pow(D / (2.0 * H), 2)) / 6;
    }
}