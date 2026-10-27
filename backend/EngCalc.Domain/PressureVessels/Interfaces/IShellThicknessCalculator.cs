using UnitsNet;

namespace EngCalc.Domain.PressureVessels;

public interface IShellThicknessCalculator
{
    Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency);
    Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency);
}
