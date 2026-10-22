using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.Converters.TemperatureConverters;

public class TemperatureConverterTests
{
    [Theory]
    [InlineData(25.0, 77.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit)]
    [InlineData(0.0, 32.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit)]
    [InlineData(100.0, 212.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit)]
    [InlineData(77.0, 25.0, TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius)]
    [InlineData(32.0, 0.0, TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius)]
    [InlineData(0.0, -17.78, TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius)]
    [InlineData(212.0, 100.0, TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius)]
    [InlineData(-40.0, -40.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit)]
    [InlineData(0.0, 273.15, TemperatureUnits.Celsius, TemperatureUnits.Kelvin)]
    [InlineData(0.0, 255.37, TemperatureUnits.Fahrenheit, TemperatureUnits.Kelvin)]
    [InlineData(0.0, -459.67, TemperatureUnits.Kelvin, TemperatureUnits.Fahrenheit)]
    [InlineData(0.0, -273.15, TemperatureUnits.Kelvin, TemperatureUnits.Celsius)]
    public void Convert_ShouldReturnCorrectValue(double initialTemperature, double expectedTemperature, TemperatureUnits initialUnit, TemperatureUnits targetUnit)
    {
        // Arrange
        
        // Act
        var actualTemperature = new TemperatureConverter(initialTemperature, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedTemperature, actualTemperature);
    }

}