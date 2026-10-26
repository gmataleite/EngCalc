using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.Converters.Services;
using Xunit;

namespace EngCalc.UnitTests.Application;

public class ConverterServiceTests
{
    private readonly ConverterService _sut = new();

    [Fact]
    public void GetOptions_ShouldReturnAllSupportedOptions()
    {
        // Act
        var options = _sut.GetOptions();

        // Assert
        Assert.NotNull(options);
        Assert.Equal(3, options.PhysicalPropertiesTypes.Count);
        Assert.Equal(3, options.TemperatureUnits.Count);
        Assert.Equal(4, options.PressureUnits.Count);
        Assert.Equal(3, options.VolumeUnits.Count);
    }

    [Theory]
    // Temperature
    [InlineData("temperature", 25.0, "Celsius", "Fahrenheit", 77.0)]
    [InlineData("temperature", 77.0, "Fahrenheit", "Celsius", 25.0)]
    [InlineData("temperature", 0.0, "Celsius", "Kelvin", 273.15)]
    [InlineData("temperature", 0.0, "Fahrenheit", "Kelvin", 255.37)]
    [InlineData("temperature", 0.0, "Kelvin", "Fahrenheit", -459.67)]
    [InlineData("temperature", 0.0, "Kelvin", "Celsius", -273.15)]
    [InlineData("temperature", 25.0, "Celsius", "Celsius", 25.0)]
    [InlineData("  TEMPERATURE  ", 100.0, "celsius", "fahrenheit", 212.0)]
    // Pressure
    [InlineData("pressure", 1.0, "Pascal", "NewtonPerSquareMeter", 1.0)]
    [InlineData("pressure", 1.0, "NewtonPerSquareMeter", "Pascal", 1.0)]
    [InlineData("pressure", 1000.0, "Pascal", "Bar", 0.01)]
    [InlineData("pressure", 0.01, "Bar", "Pascal", 1000.0)]
    [InlineData("pressure", 1000.0, "Pascal", "PoundsPerSquareInch", 0.145)]
    [InlineData("pressure", 0.1, "PoundsPerSquareInch", "Pascal", 689.476)]
    [InlineData("pressure", 100.0, "Pascal", "Pascal", 100.0)]
    [InlineData("  PRESSURE  ", 100000.0, "pascal", "bar", 1.0)]
    // Volume
    [InlineData("volume", 1.0, "CubicMeters", "Liters", 1000.0)]
    [InlineData("volume", 1000.0, "Liters", "CubicMeters", 1.0)]
    [InlineData("volume", 1.0, "CubicMeters", "Gallons", 264.17)]
    [InlineData("volume", 264.17, "Gallons", "CubicMeters", 1.0)]
    [InlineData("volume", 100.0, "Liters", "Gallons", 26.42)]
    [InlineData("volume", 10.0, "Gallons", "Liters", 37.85)]
    [InlineData("volume", 10.0, "Liters", "Liters", 10.0)]
    [InlineData("  VOLUME  ", 1.0, "cubicmeters", "liters", 1000.0)]
    public async Task ConvertAsync_ShouldReturnConverterResponseDto_WhenRequestIsValid(
        string propertyType,
        double initialValue,
        string initialUnit,
        string targetUnit,
        double expectedValue)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType, initialValue, initialUnit, targetUnit);

        // Act
        var result = await _sut.ConvertAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue, result.ConvertedValue);
        Assert.Equal(targetUnit, result.ConvertedUnit);
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ConvertAsync(null!));
    }

    [Theory]
    [InlineData("temperature", double.NaN, "Celsius", "Fahrenheit")]
    [InlineData("temperature", double.PositiveInfinity, "Celsius", "Fahrenheit")]
    [InlineData("temperature", double.NegativeInfinity, "Celsius", "Fahrenheit")]
    [InlineData("pressure", double.NaN, "Pascal", "Bar")]
    [InlineData("pressure", double.PositiveInfinity, "Pascal", "Bar")]
    [InlineData("pressure", double.NegativeInfinity, "Pascal", "Bar")]
    [InlineData("volume", double.NaN, "CubicMeters", "Liters")]
    [InlineData("volume", double.PositiveInfinity, "CubicMeters", "Liters")]
    [InlineData("volume", double.NegativeInfinity, "CubicMeters", "Liters")]
    public async Task ConvertAsync_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite(
        string propertyType,
        double initialValue,
        string initialUnit,
        string targetUnit)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType, initialValue, initialUnit, targetUnit);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.ConvertAsync(dto));
    }

    [Theory]
    [InlineData("temperature", -273.16, "Celsius", "Kelvin")]
    [InlineData("temperature", -459.68, "Fahrenheit", "Celsius")]
    [InlineData("temperature", -0.01, "Kelvin", "Celsius")]
    [InlineData("volume", -0.01, "CubicMeters", "Liters")]
    [InlineData("volume", -10.0, "Liters", "Gallons")]
    [InlineData("volume", -5.0, "Gallons", "CubicMeters")]
    public async Task ConvertAsync_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsOutOfPhysicalBounds(
        string propertyType,
        double initialValue,
        string initialUnit,
        string targetUnit)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType, initialValue, initialUnit, targetUnit);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.ConvertAsync(dto));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ConvertAsync_ShouldThrowArgumentException_WhenPhysicalPropertyTypeIsNullOrWhiteSpace(string? propertyType)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType!, 25.0, "Celsius", "Fahrenheit");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ConvertAsync(dto));
    }

    [Theory]
    [InlineData("mass")]
    [InlineData("length")]
    [InlineData("invalid_type")]
    public async Task ConvertAsync_ShouldThrowNotSupportedException_WhenPhysicalPropertyTypeIsNotSupported(string propertyType)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType, 25.0, "Celsius", "Fahrenheit");

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () => await _sut.ConvertAsync(dto));
    }

    [Theory]
    // Temperature invalid units
    [InlineData("temperature", null, "Fahrenheit")]
    [InlineData("temperature", "", "Fahrenheit")]
    [InlineData("temperature", "   ", "Fahrenheit")]
    [InlineData("temperature", "InvalidUnit", "Fahrenheit")]
    [InlineData("temperature", "999", "Fahrenheit")]
    [InlineData("temperature", "Celsius", null)]
    [InlineData("temperature", "Celsius", "")]
    [InlineData("temperature", "Celsius", "   ")]
    [InlineData("temperature", "Celsius", "InvalidUnit")]
    [InlineData("temperature", "Celsius", "999")]
    // Pressure invalid or unsupported units
    [InlineData("pressure", null, "Bar")]
    [InlineData("pressure", "", "Bar")]
    [InlineData("pressure", "   ", "Bar")]
    [InlineData("pressure", "InvalidUnit", "Bar")]
    [InlineData("pressure", "999", "Bar")]
    [InlineData("pressure", "Pascal", null)]
    [InlineData("pressure", "Pascal", "")]
    [InlineData("pressure", "Pascal", "   ")]
    [InlineData("pressure", "Pascal", "InvalidUnit")]
    [InlineData("pressure", "Pascal", "999")]
    [InlineData("pressure", "Bar", "PoundsPerSquareInch")]
    [InlineData("pressure", "Bar", "NewtonPerSquareMeter")]
    // Volume invalid units
    [InlineData("volume", null, "Liters")]
    [InlineData("volume", "", "Liters")]
    [InlineData("volume", "   ", "Liters")]
    [InlineData("volume", "InvalidUnit", "Liters")]
    [InlineData("volume", "999", "Liters")]
    [InlineData("volume", "CubicMeters", null)]
    [InlineData("volume", "CubicMeters", "")]
    [InlineData("volume", "CubicMeters", "   ")]
    [InlineData("volume", "CubicMeters", "InvalidUnit")]
    [InlineData("volume", "CubicMeters", "999")]
    public async Task ConvertAsync_ShouldThrowArgumentException_WhenUnitsAreInvalid(
        string propertyType,
        string? initialUnit,
        string? targetUnit)
    {
        // Arrange
        var dto = new ConverterRequestDto(propertyType, 25.0, initialUnit!, targetUnit!);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ConvertAsync(dto));
    }
}
