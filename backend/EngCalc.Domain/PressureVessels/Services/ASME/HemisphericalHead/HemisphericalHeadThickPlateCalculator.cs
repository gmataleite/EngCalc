namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class HemisphericalHeadThickPlateCalculator : HemisphericalHeadBase
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, L, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double denominator = 2.0 * S * E - P;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) cannot equal or exceed 2·S·E for thick hemispherical heads per ASME Appendix 1-3.");
        }

        double Y = 2.0 * (S * E + P) / denominator;
        double thickness = L * (Math.Pow(Y, 1.0 / 3.0) - 1.0);

        return Length.FromMillimeters(thickness);
    }

    public override Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (t, L, S, E) = GetMawpConstants(corrodedThickness, internalRadius, allowableStress, jointEfficiency);

        double Y = Math.Pow((L + t) / L, 3.0);
        double pressure = 2.0 * S * E * ((Y - 1.0) / (Y + 2.0));

        return Pressure.FromMegapascals(pressure);
    }
}
