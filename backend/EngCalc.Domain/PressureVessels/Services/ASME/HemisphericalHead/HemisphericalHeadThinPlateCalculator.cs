namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class HemisphericalHeadThinPlateCalculator : HemisphericalHeadBase
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, L, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double denominator = 2.0 * S * E - 0.2 * P;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) exceeds the allowable stress limit for hemispherical heads (UG-32(e)).");
        }

        double thickness = P * L / denominator;

        return Length.FromMillimeters(thickness);
    }

    public override Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (t, L, S, E) = GetMawpConstants(corrodedThickness, internalRadius, allowableStress, jointEfficiency);

        double denominator = L + 0.2 * t;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Invalid geometry for thin-plate hemispherical head MAWP calculation (UG-32(e)).");
        }

        double pressure = (2.0 * S * E * t) / denominator;

        return Pressure.FromMegapascals(pressure);
    }
}
