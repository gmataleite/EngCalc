namespace EngCalc.Domain.PressureVessels;

using System;
using UnitsNet;
using UnitsNet.Units;

internal class LongitudinalThickShellCalculator : ShellCalculatorBase // ASME VIII Div. 1, Appendix 1
{
    public override Length CalculateThickness(Pressure effectivePressure, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (P, R, S, E) = GetConstants(effectivePressure, internalRadius, allowableStress, jointEfficiency);
        double Z = (P / (S * E)) + 1;
        
        double thickness = R * (Math.Sqrt(Z) - 1);
        
        return Length.FromMillimeters(thickness);
    }

    public override Pressure CalculateMAWP(Length corrodedThickness, Length internalRadius, Pressure allowableStress, double jointEfficiency)
    {
        var (t, R, S, E) = GetMawpConstants(corrodedThickness, internalRadius, allowableStress, jointEfficiency);

        double Z = Math.Pow((R + t) / R, 2.0);
        double pressure = S * E * (Z - 1.0);

        return Pressure.FromMegapascals(pressure);
    }
}
