using System;
using System.Collections.Generic;
using UnitsNet;
using Xunit;
using EngCalc.Domain.TemperatureConverters;

namespace EngCalc.Tests.TemperatureConverters;

public class TemperatureConverterTests
{
    [Fact]
    public void ConvertCelsiusToFahrenheit_ShouldReturnCorrectValue()
    {
        // Arrange
        double celsius = 25.0;
        double expectedFahrenheit = 77.0;

        // Act
        double actualFahrenheit = TemperatureConverter.ConvertCelsiusToFahrenheit(celsius);

        // Assert
        Assert.Equal(expectedFahrenheit, actualFahrenheit, 1);
    }
}

