using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.Converters;

public class PressureConverterTests
{
    [Theory]
    [InlineData(1.0, 1.0, PressureUnits.Pascal, PressureUnits.NewtonPerSquareMeter)]
    [InlineData(1000.0, 0.01, PressureUnits.Pascal, PressureUnits.Bar)]
    [InlineData(1000.0, 0.145, PressureUnits.Pascal, PressureUnits.PoundsPerSquareInch)]
    [InlineData(1.0, 1.0, PressureUnits.NewtonPerSquareMeter, PressureUnits.Pascal)]
    [InlineData(0.01, 1000.0, PressureUnits.Bar, PressureUnits.Pascal)]
    [InlineData(0.1, 689.476, PressureUnits.PoundsPerSquareInch, PressureUnits.Pascal)]
    public void Convert_ShouldReturnCorrectValue(double initialPressure, double expectedPressure, PressureUnits initialUnit, PressureUnits targetUnit)
    {
        // Act
        var actualPressure = new PressureConverter(initialPressure, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedPressure, actualPressure);
    }

    [Fact]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite()
    {
        // Arrange
        var initialPressure = double.NaN;
        var initialUnit = PressureUnits.Pascal;
        var targetUnit = PressureUnits.NewtonPerSquareMeter;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new PressureConverter(initialPressure, initialUnit, targetUnit));
    }

    [Fact]
    public void Convert_ShouldThrowArgumentException_WhenInitialUnitIsInvalid()
    {
        // Arrange
        var initialPressure = 25.0;
        var initialUnit = (PressureUnits)999; // Invalid value
        var targetUnit = PressureUnits.NewtonPerSquareMeter;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PressureConverter(initialPressure, initialUnit, targetUnit));
    }

    [Fact]
    public void Convert_ShouldThrowArgumentException_WhenTargetUnitIsInvalid()
    {
        // Arrange
        var initialPressure = 25.0;
        var initialUnit = PressureUnits.Pascal;
        var targetUnit = (PressureUnits)999; // Invalid value

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PressureConverter(initialPressure, initialUnit, targetUnit));
    }
}
