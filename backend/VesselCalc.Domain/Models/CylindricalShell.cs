namespace VesselCalc.Domain;

using VesselCalc.Domain.Constants;
using VesselCalc.Domain.Common;

using UnitsNet;
using UnitsNet.Units;

public class CylindricalShell
{
    private readonly Pressure _designPressure;
    private readonly Length _internalDiameter;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;

    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;
    private Length InternalRadius => _internalDiameter / 2;

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

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, UG-27
    {
        Length circumferentialThickness = CalculateThicknessForStressType(StressType.Circumferential);
        Length longitudinalThickness = CalculateThicknessForStressType(StressType.Longitudinal);

        double maxRequiredThicknessMm = Math.Max(circumferentialThickness.Millimeters, longitudinalThickness.Millimeters);

        if (maxRequiredThicknessMm > InternalRadius.Millimeters / 2.0)
        {
            circumferentialThickness = new CircumferentialThickShellCalculator().CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);
            longitudinalThickness = new LongitudinalThickShellCalculator().CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);

            maxRequiredThicknessMm = Math.Max(circumferentialThickness.Millimeters, longitudinalThickness.Millimeters);
        }

        return Length.FromMillimeters(maxRequiredThicknessMm) + _allowableCorrosion;
    }

    private Length CalculateThicknessForStressType(StressType type)
    {
        IShellThicknessCalculator calculator = type switch
        {
            StressType.Circumferential => IsThickShellByPressureLimit(StressType.Circumferential) 
                ? new CircumferentialThickShellCalculator() 
                : new CircumferentialThinShellCalculator(),
                
            StressType.Longitudinal => IsThickShellByPressureLimit(StressType.Longitudinal) 
                ? new LongitudinalThickShellCalculator() 
                : new LongitudinalThinShellCalculator(),
                
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        return calculator.CalculateThickness(EffectivePressure, InternalRadius, _allowableStress, _jointEfficiency);
    }
        
    private bool IsThickShellByPressureLimit(StressType type) 
        {
            double limitFactor = type == StressType.Circumferential ? 0.385 : 1.25;
            return EffectivePressure > limitFactor * _allowableStress * _jointEfficiency;
        }

}
