using System;
using UnitsNet;
using Xunit;
using EngCalc.Domain.PressureVessels;

namespace EngCalc.Tests.PressureVessels;

public class HemisphericalHeadTests
{
    [Fact]
    public void CalculateMinimumRequiredThickness_ThinPlate_ShouldReturnCorrectThickness()
    { 
        // Arrange
        var designPressure = Pressure.FromMegapascals(1.5);
        var internalRadius = Length.FromMillimeters(1000.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);
        
        var expectedThickness = Length.FromMillimeters(11.540);

        var hemisphericalHead = new HemisphericalHead(
            designPressure,
            internalRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = hemisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }

    [Fact]
    public void CalculateMinimumRequiredThickness_ThickPlate_ShouldReturnCorrectThickness()
    {
        // Arrange
        var designPressure = Pressure.FromMegapascals(80);
        var internalRadius = Length.FromMillimeters(1000.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);
        
        var expectedThickness = Length.FromMillimeters(372.256);

        var hemisphericalHead = new HemisphericalHead(
            designPressure,
            internalRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = hemisphericalHead.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThickness.Millimeters, actualThickness.Millimeters, 3);
    }

    [Fact]
    public void CalculateMaximumAllowableWorkingPressure_ThinPlate_ShouldReturnCorrectMawp()
    {
        // Arrange
        var nominalThickness = Length.FromMillimeters(11.53969);
        var internalRadius = Length.FromMillimeters(1000.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);

        var hemisphericalHead = new HemisphericalHead(
            nominalThickness,
            internalRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualMawp = hemisphericalHead.CalculateMaximumAllowableWorkingPressure();

        // Assert
        Assert.Equal(1.500, actualMawp.Megapascals, 3);
    }

    [Fact]
    public void CalculateMaximumAllowableWorkingPressure_ThickPlate_ShouldReturnCorrectMawp()
    {
        // Arrange
        var nominalThickness = Length.FromMillimeters(372.25615);
        var internalRadius = Length.FromMillimeters(1000.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(3.0);
        var jointEfficiency = 0.85;
        var fluidColumnPressure = Pressure.FromMegapascals(0.5);

        var hemisphericalHead = new HemisphericalHead(
            nominalThickness,
            internalRadius,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var (actualMawp, isThickPlate) = hemisphericalHead.CalculateDetailedMAWP();

        // Assert
        Assert.True(isThickPlate);
        Assert.Equal(80.000, actualMawp.Megapascals, 3);
    }
}
