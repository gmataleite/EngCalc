using System;
using System.Collections.Generic;
using UnitsNet;
using Xunit;
using EngCalc.Domain.Converters;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Tests.TemperatureConverters;

public class TemperatureConverterTests
{
    [Fact]
    public void ConvertCelsiusToFahrenheit_ShouldReturnCorrectValue()
    {
        // Arrange
        double initialTemperatureCelsius = 25.0;
        double expectedTemperatureFahrenheit = 77.0;
        TemperatureUnits initialUnit = TemperatureUnits.Celsius;
        TemperatureUnits targetUnit = TemperatureUnits.Fahrenheit;
        
        // Act
        var actualTemperature = new TemperatureConverter(initialTemperatureCelsius, initialUnit, targetUnit).CalculatedTemperature;

        // Assert
        Assert.Equal(expectedTemperatureFahrenheit, actualTemperature);
    }

    [Fact]
    public void ConvertFahrenheitToCelsius_ShouldReturnCorrectValue()
    {
        // Arrange
        double initialTemperatureFahrenheit = 77.0;
        double expectedTemperatureCelsius = 25.0;
        TemperatureUnits initialUnit = TemperatureUnits.Fahrenheit;
        TemperatureUnits targetUnit = TemperatureUnits.Celsius;
        
        // Act
        var actualTemperature = new TemperatureConverter(initialTemperatureFahrenheit, initialUnit, targetUnit).CalculatedTemperature;

        // Assert
        Assert.Equal(expectedTemperatureCelsius, actualTemperature);
    }
}