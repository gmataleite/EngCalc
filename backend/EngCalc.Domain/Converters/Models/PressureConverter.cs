namespace EngCalc.Domain.Converters.Models;

using EngCalc.Domain.Converters.Enums;
using EngCalc.Domain.Converters.Interfaces;
using UnitsNet;

public class PressureConverter : IConverter
{
    public double ConvertedValue { get; private set; }

    public PressureConverter(double initialValue, PressureUnits initialUnit, PressureUnits targetUnit)
    {
        if (!double.IsFinite(initialValue))
        {
            throw new ArgumentOutOfRangeException(nameof(initialValue), $"Invalid initial value: {initialValue}");
        }
        if (!Enum.IsDefined(typeof(PressureUnits), initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {initialUnit}", nameof(initialUnit));
        }
        if (!Enum.IsDefined(typeof(PressureUnits), targetUnit))
        {
            throw new ArgumentException($"Invalid target unit: {targetUnit}", nameof(targetUnit));
        }

        ConvertValue(initialValue, initialUnit, targetUnit);
    }

    public double ConvertValue(double initialValue, Enum initialUnit, Enum targetUnit)
    {
        switch (initialUnit, targetUnit)
        {
            case (PressureUnits u1, PressureUnits u2) when u1 == u2:
                ConvertedValue = Math.Round(initialValue, 2);
                break;
            case (PressureUnits.Pascal, PressureUnits.NewtonPerSquareMeter):
                ConvertedValue = PascalToNewtonPerSquareMeter(initialValue);
                break;
            case (PressureUnits.NewtonPerSquareMeter, PressureUnits.Pascal):
                ConvertedValue = NewtonPerSquareMeterToPascal(initialValue);
                break;
            case (PressureUnits.Pascal, PressureUnits.Bar):
                ConvertedValue = PascalToBar(initialValue);
                break;
            case (PressureUnits.Bar, PressureUnits.Pascal):
                ConvertedValue = BarToPascal(initialValue);
                break;
            case (PressureUnits.Pascal, PressureUnits.PoundsPerSquareInch):
                ConvertedValue = PascalToPoundsPerSquareInch(initialValue);
                break;
            case (PressureUnits.PoundsPerSquareInch, PressureUnits.Pascal):
                ConvertedValue = PoundsPerSquareInchToPascal(initialValue);
                break;
            default:
                throw new ArgumentException("Invalid pressure units provided.");
        }
        return ConvertedValue;
    }

    public double PascalToNewtonPerSquareMeter(double initialValue) => Math.Round(Pressure.FromPascals(initialValue).NewtonsPerSquareMeter, 3);
    public double NewtonPerSquareMeterToPascal(double initialValue) => Math.Round(Pressure.FromNewtonsPerSquareMeter(initialValue).Pascals, 3);
    public double PascalToBar(double initialValue) => Math.Round(Pressure.FromPascals(initialValue).Bars, 3);
    public double BarToPascal(double initialValue) => Math.Round(Pressure.FromBars(initialValue).Pascals, 3);
    public double PascalToPoundsPerSquareInch(double initialValue) => Math.Round(Pressure.FromPascals(initialValue).PoundsForcePerSquareInch, 3);
    public double PoundsPerSquareInchToPascal(double initialValue) => Math.Round(Pressure.FromPoundsForcePerSquareInch(initialValue).Pascals, 3);
}
