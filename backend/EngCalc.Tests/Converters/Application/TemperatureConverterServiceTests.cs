using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.Converters.Services;
using Xunit;

namespace EngCalc.UnitTests.Application;

public class MachineServiceTests
{
    [Theory]
    [InlineData(25.0, 77.0, "Celsius", "Fahrenheit")]
    [InlineData(0.0, 32.0, "Celsius", "Fahrenheit")]
    [InlineData(100.0, 212.0, "Celsius", "Fahrenheit")]
    [InlineData(77.0, 25.0, "Fahrenheit", "Celsius")]
    [InlineData(32.0, 0.0, "Fahrenheit", "Celsius")]
    [InlineData(0.0, -17.78, "Fahrenheit", "Celsius")]
    [InlineData(212.0, 100.0, "Fahrenheit", "Celsius")]
    [InlineData(-40.0, -40.0, "Celsius", "Fahrenheit")]
    [InlineData(0.0, 273.15, "Celsius", "Kelvin")]
    [InlineData(0.0, 255.37, "Fahrenheit", "Kelvin")]
    [InlineData(0.0, -459.67, "Kelvin", "Fahrenheit")]
    [InlineData(0.0, -273.15, "Kelvin", "Celsius")]
    public async Task ConvertAsync_ShouldReturnTemperatureConverterResponseDto_WhenSuccessful(double initialValue, double expectedValue, string initialUnit, string targetUnit)
    {
        // Arrange
        var dto = new TemperatureConverterRequestDto(initialValue, initialUnit, targetUnit);

        // Act
        var result = await new ConverterService().ConvertAsync(dto);


        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue, result.ConvertedValue);
        Assert.Equal(targetUnit, result.ConvertedUnit);
    }
}