using System;
using UnitsNet;
using Xunit;
using VesselCalc.Domain;

namespace VesselCalc.Tests;

public class TorisphericalHeadTests
{

    [Fact]
    public void CalculateMinimumRequiredThickness_DefaultRatio_ShouldReturnCorrectThickness()
    { 
        // CENÁRIO: Tampo Torisférico padrão 6% sob pressão interna (ASME VIII Div.1, UG-32(c))
        // Arrange
        var designPressure = Pressure.FromMegapascals(1.5);
        var crownRadius = Length.FromMillimeters(2000.0);
        var knuckleRadius = Length.FromMillimeters(120.0); // 6% of crown radius
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);
        
        var expectedThickness = Length.FromMillimeters(33.241);

        var torisphericalHead = new TorisphericalHead(
            designPressure,
            crownRadius,
            knuckleRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = torisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }

    [Fact]
    public void CalculateMinimumRequiredThickness_DefaultRatioTen_ShouldReturnCorrectThickness()
    {        
        // CENÁRIO: Tampo Torisférico 10% sob pressão interna (ASME VIII Div.1, UG-32(c))
        // Arrange
        var designPressure = Pressure.FromMegapascals(1.5);
        var crownRadius = Length.FromMillimeters(2000.0);
        var knuckleRadius = Length.FromMillimeters(200.0); // 10% of crown radius
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);
        
        var expectedThickness = Length.FromMillimeters(29.312);

        var torisphericalHead = new TorisphericalHead(
            designPressure,
            crownRadius,
            knuckleRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = torisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }

    [Fact]
    public void CalculateMinimumRequiredThickness_WithCrownRadiusAndKnuckleRadiusInput_ShouldReturnCorrectThickness()
    {        
        // Arrange
        var designPressure = Pressure.FromMegapascals(1.5);
        var crownRadius = Length.FromMillimeters(2000.0);
        var knuckleRadius = Length.FromMillimeters(200.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);

        var expectedThickness = Length.FromMillimeters(29.312);

        var torisphericalHead = new TorisphericalHead(
            designPressure,
            crownRadius,
            knuckleRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = torisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }

    [Fact]
    public void CalculateMinimumRequiredThickness_WithDiameterAndRatioInput_ShouldReturnCorrectThickness()
    {        
        // Arrange
        var designPressure = Pressure.FromMegapascals(1.5);
        var internalDiameter = Length.FromMillimeters(2000.0);
        var ratio = 0.1;
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);

        var expectedThickness = Length.FromMillimeters(29.312);

        var torisphericalHead = new TorisphericalHead(
            designPressure,
            internalDiameter,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure,
            ratio);

        // Act
        var actualThickness = torisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }
}