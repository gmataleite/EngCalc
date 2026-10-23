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
}
