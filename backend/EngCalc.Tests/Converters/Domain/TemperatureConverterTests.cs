using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.Converters;

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
    [InlineData(25.0, 25.0, TemperatureUnits.Celsius, TemperatureUnits.Celsius)]
    public void Convert_ShouldReturnCorrectValue(double initialTemperature, double expectedTemperature, TemperatureUnits initialUnit, TemperatureUnits targetUnit)
    {
        // Act
        var actualTemperature = new TemperatureConverter(initialTemperature, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedTemperature, actualTemperature);
    }

    [Fact]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite()
    {
        // Arrange
        var initialTemperature = double.NaN;
        var initialUnit = TemperatureUnits.Celsius;
        var targetUnit = TemperatureUnits.Fahrenheit;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }

    [Fact]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenBelowAbsoluteZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TemperatureConverter(-300.0, TemperatureUnits.Celsius, TemperatureUnits.Kelvin));
    }

    [Fact]
    public void Convert_ShouldThrowArgumentException_WhenInitialUnitIsInvalid()
    {
        // Arrange
        var initialTemperature = 25.0;
        var initialUnit = (TemperatureUnits)999; // Invalid value
        var targetUnit = TemperatureUnits.Fahrenheit;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }

    [Fact]
    public void Convert_ShouldThrowArgumentException_WhenTargetUnitIsInvalid()
    {
        // Arrange
        var initialTemperature = 25.0;
        var initialUnit = TemperatureUnits.Celsius;
        var targetUnit = (TemperatureUnits)999; // Invalid value

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }
}
