namespace VesselCalc.Domain;

using UnitsNet;
using UnitsNet.Units;

public abstract class HemisphericalHeadBase
{
    protected (double P, double L, double S, double E) GetConstants(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        return (
            effectivePressure.Megapascals,
            internalRadius.Millimeters,
            allowableStress.Megapascals,
            jointEfficiency
        );
    }
    public abstract Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency);

}