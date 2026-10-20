namespace VesselCalc.Domain;

using UnitsNet;
using UnitsNet.Units;

internal class CircumferentialThinShellCalculator : ShellCalculatorBase // ASME VIII Div. 1, UG-27(c-1)
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, R, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double thickness = P * R / ((S * E) - (0.6 * P));
        
        return Length.FromMillimeters(thickness);
    }
}