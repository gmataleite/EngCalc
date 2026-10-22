namespace EngCalc.Domain.Converters;

using EngCalc.Domain.Converters.Enums;
using UnitsNet;
public class TemperatureConverter
{
    public double CalculatedTemperature { get; private set; }

    public  TemperatureConverter(double initialTemperature, TemperatureUnits initialUnit, TemperatureUnits targetUnit)
    {
        if (initialUnit == TemperatureUnits.Celsius && targetUnit == TemperatureUnits.Fahrenheit)
        {
            CalculatedTemperature = CelsiusToFahrenheit(initialTemperature);
        }
        else if (initialUnit == TemperatureUnits.Fahrenheit && targetUnit == TemperatureUnits.Celsius)
        {
            CalculatedTemperature = FahrenheitToCelsius(initialTemperature);
        }
        else
        {
            throw new ArgumentException("Invalid temperature units provided.");
        }
    }

    // public double ConvertCelsiusToFahrenheit(double initialTemperature)
    // {
    //         return (initialTemperature * 9 / 5) + 32;
    // }

    public double CelsiusToFahrenheit(double initialTemperature) => Math.Round(Temperature.FromDegreesCelsius(initialTemperature).DegreesFahrenheit, 2);
    public double FahrenheitToCelsius(double initialTemperature) => Math.Round(Temperature.FromDegreesFahrenheit(initialTemperature).DegreesCelsius, 2);
}