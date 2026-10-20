using UnitsNet;

namespace VesselCalc.Domain;

public interface IShellThicknessCalculator
{
    Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency);
}
