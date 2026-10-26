using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.Converters;

public class PressureConverterTests
{
    [Theory]
    [InlineData(1.0, 1.0, PressureUnits.Pascal, PressureUnits.NewtonPerSquareMeter)]
    [InlineData(1000.0, 0.01, PressureUnits.Pascal, PressureUnits.Bar)]
    [InlineData(100000.0, 1.0, PressureUnits.Pascal, PressureUnits.Bar)]
    [InlineData(1000.0, 0.145, PressureUnits.Pascal, PressureUnits.PoundsPerSquareInch)]
    [InlineData(1.0, 1.0, PressureUnits.NewtonPerSquareMeter, PressureUnits.Pascal)]
    [InlineData(0.01, 1000.0, PressureUnits.Bar, PressureUnits.Pascal)]
    [InlineData(1.0, 100000.0, PressureUnits.Bar, PressureUnits.Pascal)]
    [InlineData(0.1, 689.476, PressureUnits.PoundsPerSquareInch, PressureUnits.Pascal)]
    [InlineData(1.0, 6894.757, PressureUnits.PoundsPerSquareInch, PressureUnits.Pascal)]
    [InlineData(0.0, 0.0, PressureUnits.Pascal, PressureUnits.Bar)]
    [InlineData(100.0, 100.0, PressureUnits.Pascal, PressureUnits.Pascal)]
    [InlineData(50.0, 50.0, PressureUnits.NewtonPerSquareMeter, PressureUnits.NewtonPerSquareMeter)]
    [InlineData(2.5, 2.5, PressureUnits.Bar, PressureUnits.Bar)]
    [InlineData(14.7, 14.7, PressureUnits.PoundsPerSquareInch, PressureUnits.PoundsPerSquareInch)]
    public void Convert_ShouldReturnCorrectValue(
        double initialPressure,
        double expectedPressure,
        PressureUnits initialUnit,
        PressureUnits targetUnit)
    {
        // Act
        var actualPressure = new PressureConverter(initialPressure, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedPressure, actualPressure);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite(double initialPressure)
    {
        // Arrange
        var initialUnit = PressureUnits.Pascal;
        var targetUnit = PressureUnits.NewtonPerSquareMeter;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new PressureConverter(initialPressure, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData((PressureUnits)999, PressureUnits.NewtonPerSquareMeter)]
    [InlineData((PressureUnits)(-1), PressureUnits.Pascal)]
    [InlineData(PressureUnits.Pascal, (PressureUnits)999)]
    [InlineData(PressureUnits.Bar, (PressureUnits)(-1))]
    [InlineData(PressureUnits.Bar, PressureUnits.PoundsPerSquareInch)]
    [InlineData(PressureUnits.Bar, PressureUnits.NewtonPerSquareMeter)]
    [InlineData(PressureUnits.NewtonPerSquareMeter, PressureUnits.Bar)]
    [InlineData(PressureUnits.NewtonPerSquareMeter, PressureUnits.PoundsPerSquareInch)]
    [InlineData(PressureUnits.PoundsPerSquareInch, PressureUnits.Bar)]
    [InlineData(PressureUnits.PoundsPerSquareInch, PressureUnits.NewtonPerSquareMeter)]
    public void Convert_ShouldThrowArgumentException_WhenUnitsAreInvalidOrUnsupported(
        PressureUnits initialUnit,
        PressureUnits targetUnit)
    {
        // Arrange
        var initialPressure = 25.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PressureConverter(initialPressure, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData(TemperatureUnits.Celsius, PressureUnits.Pascal)]
    [InlineData(PressureUnits.Pascal, TemperatureUnits.Celsius)]
    public void ConvertValue_ShouldThrowArgumentException_WhenEnumTypesAreInvalid(Enum initialUnit, Enum targetUnit)
    {
        // Arrange
        var converter = new PressureConverter(100.0, PressureUnits.Pascal, PressureUnits.Bar);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => converter.ConvertValue(100.0, initialUnit, targetUnit));
    }
}
