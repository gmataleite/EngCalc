namespace VesselCalc.Domain;

using UnitsNet;
using UnitsNet.Units;

internal class HemisphericalHeadThinPlateCalculator : HemisphericalHeadBase
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, L, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);

        double thickness =  P * L / (2.0 * S * E - 0.2 * P);

        return Length.FromMillimeters(thickness);
    }
}