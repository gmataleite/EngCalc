namespace EngCalc.Domain.Converters;

using EngCalc.Domain.Converters.Enums;
using EngCalc.Domain.Converters.Interfaces;
using UnitsNet;
public class TemperatureConverter : IConverter
{
    public double CalculatedTemperature { get; private set; }

    public TemperatureConverter(double initialTemperature, TemperatureUnits initialUnit, TemperatureUnits targetUnit)
    {
        ConvertValue(initialTemperature, initialUnit, targetUnit);
    }

    public double ConvertValue(double initialTemperature, Enum initialUnit, Enum targetUnit)
    {
        switch (initialUnit, targetUnit)
        {
        case (TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit):
            CalculatedTemperature = CelsiusToFahrenheit(initialTemperature);
            break;
        case (TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius):
            CalculatedTemperature = FahrenheitToCelsius(initialTemperature);
            break;
        case (TemperatureUnits.Celsius, TemperatureUnits.Kelvin):
            CalculatedTemperature = CelsiusToKelvin(initialTemperature);
            break;
        case (TemperatureUnits.Fahrenheit, TemperatureUnits.Kelvin):
            CalculatedTemperature = FahrenheitToKelvin(initialTemperature);
            break;
        case (TemperatureUnits.Kelvin, TemperatureUnits.Fahrenheit):
            CalculatedTemperature = KelvinToFahrenheit(initialTemperature);
            break;
        case (TemperatureUnits.Kelvin, TemperatureUnits.Celsius):
            CalculatedTemperature = KelvinToCelsius(initialTemperature);
            break;
        default:
            throw new ArgumentException("Invalid temperature units provided.");
        }
        return CalculatedTemperature;
    }

    public double CelsiusToFahrenheit(double initialTemperature) => Math.Round(Temperature.FromDegreesCelsius(initialTemperature).DegreesFahrenheit, 2);
    public double FahrenheitToCelsius(double initialTemperature) => Math.Round(Temperature.FromDegreesFahrenheit(initialTemperature).DegreesCelsius, 2);
    public double CelsiusToKelvin(double initialTemperature) => Math.Round(Temperature.FromDegreesCelsius(initialTemperature).Kelvins, 2);
    public double FahrenheitToKelvin(double initialTemperature) => Math.Round(Temperature.FromDegreesFahrenheit(initialTemperature).Kelvins, 2);
    public double KelvinToFahrenheit(double initialTemperature) => Math.Round(Temperature.FromKelvins(initialTemperature).DegreesFahrenheit, 2);
    public double KelvinToCelsius(double initialTemperature) => Math.Round(Temperature.FromKelvins(initialTemperature).DegreesCelsius, 2);
}