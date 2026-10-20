namespace VesselCalc.Domain;

using UnitsNet;
using UnitsNet.Units;

internal class HemisphericalHeadThickPlateCalculator : HemisphericalHeadBase
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, L, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double Y = 2.0 * (S * E + P) /  (2.0 * S * E - P);
        double thickness = L * (Math.Pow(Y, 1.0 / 3.0) - 1.0);

        return Length.FromMillimeters(thickness);
    }
}