using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.Converters.Services;
using Xunit;

namespace EngCalc.UnitTests.Application;

public class TemperatureConverterServiceTests
{
    [Fact]
    public async Task ConvertAsync_ShouldReturnTemperatureConverterResponseDto_WhenSuccessful()
    {
        // Arrange
        var dto = new TemperatureConverterRequestDto(25.0, "Celsius", "Fahrenheit");

        // Act
        var result = await new ConverterService().ConvertAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(77.0, result.ConvertedValue);
        Assert.Equal("Fahrenheit", result.ConvertedUnit);
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowArgumentOutOfRangeException_WhenInitialValueIsNotFinite()
    {
        // Arrange
        var dto = new TemperatureConverterRequestDto(double.NaN, "Celsius", "Fahrenheit");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await new ConverterService().ConvertAsync(dto));
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowArgumentException_WhenInitialUnitIsInvalid()
    {
        // Arrange
        var dto = new TemperatureConverterRequestDto(25.0, "InvalidUnit", "Fahrenheit");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await new ConverterService().ConvertAsync(dto));
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrowArgumentException_WhenTargetUnitIsInvalid()
    {
        // Arrange
        var dto = new TemperatureConverterRequestDto(25.0, "Celsius", "InvalidUnit");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await new ConverterService().ConvertAsync(dto));
    }
}