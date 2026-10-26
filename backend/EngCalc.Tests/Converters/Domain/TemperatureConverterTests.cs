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
    [InlineData(77.0, 77.0, TemperatureUnits.Fahrenheit, TemperatureUnits.Fahrenheit)]
    [InlineData(300.0, 300.0, TemperatureUnits.Kelvin, TemperatureUnits.Kelvin)]
    public void Convert_ShouldReturnCorrectValue(
        double initialTemperature,
        double expectedTemperature,
        TemperatureUnits initialUnit,
        TemperatureUnits targetUnit)
    {
        // Act
        var actualTemperature = new TemperatureConverter(initialTemperature, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedTemperature, actualTemperature);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite(double initialTemperature)
    {
        // Arrange
        var initialUnit = TemperatureUnits.Celsius;
        var targetUnit = TemperatureUnits.Fahrenheit;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData(-273.16, TemperatureUnits.Celsius, TemperatureUnits.Kelvin)]
    [InlineData(-300.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit)]
    [InlineData(-459.68, TemperatureUnits.Fahrenheit, TemperatureUnits.Celsius)]
    [InlineData(-500.0, TemperatureUnits.Fahrenheit, TemperatureUnits.Kelvin)]
    [InlineData(-0.01, TemperatureUnits.Kelvin, TemperatureUnits.Celsius)]
    [InlineData(-10.0, TemperatureUnits.Kelvin, TemperatureUnits.Fahrenheit)]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenBelowAbsoluteZero(
        double initialTemperature,
        TemperatureUnits initialUnit,
        TemperatureUnits targetUnit)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData((TemperatureUnits)999, TemperatureUnits.Fahrenheit)]
    [InlineData((TemperatureUnits)(-1), TemperatureUnits.Celsius)]
    [InlineData(TemperatureUnits.Celsius, (TemperatureUnits)999)]
    [InlineData(TemperatureUnits.Kelvin, (TemperatureUnits)(-1))]
    public void Convert_ShouldThrowArgumentException_WhenUnitIsInvalid(
        TemperatureUnits initialUnit,
        TemperatureUnits targetUnit)
    {
        // Arrange
        var initialTemperature = 25.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TemperatureConverter(initialTemperature, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData(PressureUnits.Pascal, TemperatureUnits.Celsius)]
    [InlineData(TemperatureUnits.Celsius, PressureUnits.Pascal)]
    public void ConvertValue_ShouldThrowArgumentException_WhenEnumTypesAreInvalid(Enum initialUnit, Enum targetUnit)
    {
        // Arrange
        var converter = new TemperatureConverter(25.0, TemperatureUnits.Celsius, TemperatureUnits.Fahrenheit);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => converter.ConvertValue(25.0, initialUnit, targetUnit));
    }
}
