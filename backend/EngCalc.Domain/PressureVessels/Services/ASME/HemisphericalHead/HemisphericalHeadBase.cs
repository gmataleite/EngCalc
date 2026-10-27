namespace EngCalc.Domain.PressureVessels;

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

    protected (double t, double L, double S, double E) GetMawpConstants(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        return (
            corrodedThickness.Millimeters,
            internalRadius.Millimeters,
            allowableStress.Megapascals,
            jointEfficiency
        );
    }

    public abstract Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency);
    public abstract Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency);
}
