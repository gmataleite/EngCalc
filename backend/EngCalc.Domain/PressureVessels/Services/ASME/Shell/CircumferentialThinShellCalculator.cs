namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class CircumferentialThinShellCalculator : ShellCalculatorBase // ASME VIII Div. 1, UG-27(c-1)
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, R, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double denominator = (S * E) - (0.6 * P);
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) exceeds the allowable stress limit for thin-wall cylindrical shells (UG-27).");
        }

        double thickness = P * R / denominator;
        
        return Length.FromMillimeters(thickness);
    }
}
