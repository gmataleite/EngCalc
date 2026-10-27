namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class CircumferentialThickShellCalculator : ShellCalculatorBase // ASME VIII Div. 1, Appendix 1
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, R, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double denominator = S * E - P;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) cannot equal or exceed the product of allowable stress and joint efficiency (S·E) per ASME Appendix 1-2.");
        }

        double Z = (S * E + P) / denominator;
        double thickness = R * (Math.Sqrt(Z) - 1);

        return Length.FromMillimeters(thickness);
    }

    public override Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (t, R, S, E) = GetMawpConstants(corrodedThickness, internalRadius, allowableStress, jointEfficiency);

        double Z = Math.Pow((R + t) / R, 2.0);
        double pressure = S * E * ((Z - 1.0) / (Z + 1.0));

        return Pressure.FromMegapascals(pressure);
    }
}
