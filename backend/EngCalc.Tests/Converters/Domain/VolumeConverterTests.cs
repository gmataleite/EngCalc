using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.Converters;

public class VolumeConverterTests
{
    [Theory]
    [InlineData(1.0, 1000.0, VolumeUnits.CubicMeters, VolumeUnits.Liters)]
    [InlineData(1000.0, 1.0, VolumeUnits.Liters, VolumeUnits.CubicMeters)]
    [InlineData(1.0, 264.17, VolumeUnits.CubicMeters, VolumeUnits.Gallons)]
    [InlineData(264.17, 1.0, VolumeUnits.Gallons, VolumeUnits.CubicMeters)]
    [InlineData(100.0, 26.42, VolumeUnits.Liters, VolumeUnits.Gallons)]
    [InlineData(10.0, 37.85, VolumeUnits.Gallons, VolumeUnits.Liters)]
    [InlineData(0.0, 0.0, VolumeUnits.CubicMeters, VolumeUnits.Liters)]
    [InlineData(0.0, 0.0, VolumeUnits.Liters, VolumeUnits.Gallons)]
    [InlineData(0.0, 0.0, VolumeUnits.Gallons, VolumeUnits.CubicMeters)]
    [InlineData(5.5, 5.5, VolumeUnits.CubicMeters, VolumeUnits.CubicMeters)]
    [InlineData(25.0, 25.0, VolumeUnits.Liters, VolumeUnits.Liters)]
    [InlineData(12.34, 12.34, VolumeUnits.Gallons, VolumeUnits.Gallons)]
    public void Convert_ShouldReturnCorrectValue(
        double initialVolume,
        double expectedVolume,
        VolumeUnits initialUnit,
        VolumeUnits targetUnit)
    {
        // Act
        var actualVolume = new VolumeConverter(initialVolume, initialUnit, targetUnit).ConvertedValue;

        // Assert
        Assert.Equal(expectedVolume, actualVolume);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite(double initialVolume)
    {
        // Arrange
        var initialUnit = VolumeUnits.CubicMeters;
        var targetUnit = VolumeUnits.Liters;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new VolumeConverter(initialVolume, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData(-0.01, VolumeUnits.CubicMeters, VolumeUnits.Liters)]
    [InlineData(-10.0, VolumeUnits.Liters, VolumeUnits.Gallons)]
    [InlineData(-100.0, VolumeUnits.Gallons, VolumeUnits.CubicMeters)]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNegative(
        double initialVolume,
        VolumeUnits initialUnit,
        VolumeUnits targetUnit)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new VolumeConverter(initialVolume, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData((VolumeUnits)999, VolumeUnits.Liters)]
    [InlineData((VolumeUnits)(-1), VolumeUnits.CubicMeters)]
    [InlineData(VolumeUnits.CubicMeters, (VolumeUnits)999)]
    [InlineData(VolumeUnits.Gallons, (VolumeUnits)(-1))]
    public void Convert_ShouldThrowArgumentException_WhenUnitIsInvalid(
        VolumeUnits initialUnit,
        VolumeUnits targetUnit)
    {
        // Arrange
        var initialVolume = 10.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new VolumeConverter(initialVolume, initialUnit, targetUnit));
    }

    [Theory]
    [InlineData(TemperatureUnits.Celsius, VolumeUnits.Liters)]
    [InlineData(VolumeUnits.Liters, TemperatureUnits.Celsius)]
    public void ConvertValue_ShouldThrowArgumentException_WhenEnumTypesAreInvalid(Enum initialUnit, Enum targetUnit)
    {
        // Arrange
        var converter = new VolumeConverter(1.0, VolumeUnits.CubicMeters, VolumeUnits.Liters);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => converter.ConvertValue(1.0, initialUnit, targetUnit));
    }
}

