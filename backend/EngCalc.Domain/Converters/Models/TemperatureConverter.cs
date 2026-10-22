namespace EngCalc.Domain.Converters.Models;

using EngCalc.Domain.Converters.Enums;
using EngCalc.Domain.Converters.Interfaces;
using UnitsNet;
public class TemperatureConverter : IConverter
{
    public double ConvertedValue { get; private set; }

    public TemperatureConverter(double initialValue, TemperatureUnits initialUnit, TemperatureUnits targetUnit)
    {
        ConvertValue(initialValue, initialUnit, targetUnit);
    }

    public double ConvertValue(double initialValue, Enum initialUnit, Enum targetUnit)
    {
        switch (initialUnit, targetUnit)
        {
        case (TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit):
            ConvertedValue = CelsiusToFahrenheit(initialValue);
            break;
        case (TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius):
            ConvertedValue = FahrenheitToCelsius(initialValue);
            break;
        case (TemperatureUnits.Celsius, TemperatureUnits.Kelvin):
            ConvertedValue = CelsiusToKelvin(initialValue);
            break;
        case (TemperatureUnits.Fahrenheit, TemperatureUnits.Kelvin):
            ConvertedValue = FahrenheitToKelvin(initialValue);
            break;
        case (TemperatureUnits.Kelvin, TemperatureUnits.Fahrenheit):
            ConvertedValue = KelvinToFahrenheit(initialValue);
            break;
        case (TemperatureUnits.Kelvin, TemperatureUnits.Celsius):
            ConvertedValue = KelvinToCelsius(initialValue);
            break;
        default:
            throw new ArgumentException("Invalid temperature units provided.");
        }
        return ConvertedValue;
    }

    public double CelsiusToFahrenheit(double initialValue) => Math.Round(Temperature.FromDegreesCelsius(initialValue).DegreesFahrenheit, 2);
    public double FahrenheitToCelsius(double initialValue) => Math.Round(Temperature.FromDegreesFahrenheit(initialValue).DegreesCelsius, 2);
    public double CelsiusToKelvin(double initialValue) => Math.Round(Temperature.FromDegreesCelsius(initialValue).Kelvins, 2);
    public double FahrenheitToKelvin(double initialValue) => Math.Round(Temperature.FromDegreesFahrenheit(initialValue).Kelvins, 2);
    public double KelvinToFahrenheit(double initialValue) => Math.Round(Temperature.FromKelvins(initialValue).DegreesFahrenheit, 2);
    public double KelvinToCelsius(double initialValue) => Math.Round(Temperature.FromKelvins(initialValue).DegreesCelsius, 2);
}