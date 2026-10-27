namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class LongitudinalThinShellCalculator : ShellCalculatorBase // ASME VIII Div. 1, UG-27(c-2)
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, R, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double thickness = P * R / ((2 * S * E) + (0.4 * P));
        
        return Length.FromMillimeters(thickness);
    }

    public override Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (t, R, S, E) = GetMawpConstants(corrodedThickness, internalRadius, allowableStress, jointEfficiency);

        double denominator = R - (0.4 * t);
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Corroded thickness (t) exceeds the geometric limit for thin-wall longitudinal stress calculation (UG-27(c)(2)).");
        }

        double pressure = (2.0 * S * E * t) / denominator;

        return Pressure.FromMegapascals(pressure);
    }
}
