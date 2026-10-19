namespace VesselCalc.Domain;

using System;
using VesselCalc.Domain.Common;
using UnitsNet;

public class HemisphericalHead
{
    private readonly Pressure _designPressure;
    private readonly Length _internalRadius;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;
    
    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;

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

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, UG-32(e)
    {
        Length thickness = new HemisphericalHeadThinPlateCalculator().CalculateThickness(EffectivePressure, _internalRadius, _allowableStress, _jointEfficiency);

        if (IsThickShellByPressureLimit() || IsThickShellByThickness(thickness))
        {
            thickness = new HemisphericalHeadThickPlateCalculator().CalculateThickness(EffectivePressure, _internalRadius, _allowableStress, _jointEfficiency);
        }

        return thickness + _allowableCorrosion;
    }

    private bool IsThickShellByPressureLimit() 
    {
        double limitFactor = 0.665;
        return EffectivePressure > limitFactor * _allowableStress * _jointEfficiency;
    }

    private bool IsThickShellByThickness(Length thickness) 
    {
        double limitFactor = 0.356;
        return thickness > limitFactor * _internalRadius;
    }
}