namespace EngCalc.Tests.StorageTanks.Domain;

using System;
using EngCalc.Domain.StorageTanks.Entities;
using EngCalc.Domain.StorageTanks.Enums;
using Xunit;

public class HorizontalCylindricalTankVolumeTests
{
    private const double PrecisionTolerance = 1e-9;

    [Fact]
    public void VolumeConstructor_ShouldCalculateInternalVolumeAndTotalHeadsVolume()
    {
        var tank = new HorizontalCylindricalTankVolume(bodyVolume: 100, headVolume: 20);

        Assert.Equal(100, tank.BodyVolume);
        Assert.Equal(20, tank.HeadVolume);
        Assert.Equal(40, tank.TotalHeadsVolume);
        Assert.Equal(20, tank.BottomVolume); // backwards compatibility
        Assert.Equal(140, tank.InternalVolume);
        Assert.Equal(0, tank.ProductVolume);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Hemispherical)]
    [InlineData(HeadTanksTypes.Ellipsoidal)]
    [InlineData(HeadTanksTypes.Torispherical)]
    public void GeometricConstructor_ShouldSupportValidHorizontalHeads(HeadTanksTypes headType)
    {
        double diameter = 2.0;
        double length = 5.0;

        var tank = new HorizontalCylindricalTankVolume(
            internalDiameter: diameter,
            internalBodyLength: length,
            headType: headType);

        double radius = diameter / 2.0;
        double expectedBody = Math.PI * Math.Pow(radius, 2) * length;
        double expectedHead = HorizontalCylindricalTankVolume.CalculateHeadVolume(headType, diameter);

        Assert.Equal(expectedBody, tank.BodyVolume, PrecisionTolerance);
        Assert.Equal(expectedHead, tank.HeadVolume, PrecisionTolerance);
        Assert.Equal(expectedHead * 2.0, tank.TotalHeadsVolume, PrecisionTolerance);
        Assert.Equal(expectedBody + expectedHead * 2.0, tank.InternalVolume, PrecisionTolerance);
        Assert.Equal(0, tank.ProductVolume);
    }

    [Fact]
    public void SphericalAlias_ShouldBehaveIdenticallyToHemispherical()
    {
        var tankHemispherical = new HorizontalCylindricalTankVolume(2.0, 5.0, HeadTanksTypes.Hemispherical, productHeight: 1.0);
        var tankSpherical = new HorizontalCylindricalTankVolume(2.0, 5.0, HeadTanksTypes.Spherical, productHeight: 1.0);

        Assert.Equal(tankHemispherical.InternalVolume, tankSpherical.InternalVolume, PrecisionTolerance);
        Assert.Equal(tankHemispherical.ProductVolume, tankSpherical.ProductVolume, PrecisionTolerance);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Flat)]
    [InlineData(HeadTanksTypes.Conical)]
    public void GeometricConstructor_ShouldRejectInvalidHorizontalHeadTypes(HeadTanksTypes invalidHeadType)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new HorizontalCylindricalTankVolume(
                internalDiameter: 2.0,
                internalBodyLength: 5.0,
                headType: invalidHeadType));

        Assert.Contains("only Hemispherical, Ellipsoidal, or Torispherical heads are supported", ex.Message);
    }

    [Theory]
    [InlineData(0, 5.0)]
    [InlineData(-1, 5.0)]
    [InlineData(2.0, 0)]
    [InlineData(2.0, -1)]
    public void GeometricConstructor_ShouldThrowForInvalidDimensions(double diameter, double length)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new HorizontalCylindricalTankVolume(
                internalDiameter: diameter,
                internalBodyLength: length,
                headType: HeadTanksTypes.Hemispherical));
    }

    [Theory]
    [InlineData(HeadTanksTypes.Hemispherical)]
    [InlineData(HeadTanksTypes.Ellipsoidal)]
    [InlineData(HeadTanksTypes.Torispherical)]
    public void ProductVolume_AtZeroHeight_ShouldBeZero(HeadTanksTypes headType)
    {
        var tank = new HorizontalCylindricalTankVolume(
            internalDiameter: 2.0,
            internalBodyLength: 5.0,
            headType: headType,
            productHeight: 0.0);

        Assert.Equal(0.0, tank.ProductVolume, PrecisionTolerance);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Hemispherical)]
    [InlineData(HeadTanksTypes.Ellipsoidal)]
    [InlineData(HeadTanksTypes.Torispherical)]
    public void ProductVolume_AtFullHeight_ShouldEqualInternalVolume(HeadTanksTypes headType)
    {
        double diameter = 2.0;
        double length = 5.0;

        var tank = new HorizontalCylindricalTankVolume(
            internalDiameter: diameter,
            internalBodyLength: length,
            headType: headType,
            productHeight: diameter);

        Assert.Equal(tank.InternalVolume, tank.ProductVolume, PrecisionTolerance);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Hemispherical)]
    [InlineData(HeadTanksTypes.Ellipsoidal)]
    [InlineData(HeadTanksTypes.Torispherical)]
    public void ProductVolume_AtHalfHeight_ShouldEqualHalfInternalVolumeBySymmetry(HeadTanksTypes headType)
    {
        double diameter = 2.0;
        double length = 5.0;
        double halfHeight = diameter / 2.0;

        var tank = new HorizontalCylindricalTankVolume(
            internalDiameter: diameter,
            internalBodyLength: length,
            headType: headType,
            productHeight: halfHeight);

        double expectedHalfVolume = tank.InternalVolume / 2.0;
        Assert.Equal(expectedHalfVolume, tank.ProductVolume, PrecisionTolerance);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Hemispherical)]
    [InlineData(HeadTanksTypes.Ellipsoidal)]
    [InlineData(HeadTanksTypes.Torispherical)]
    public void ProductVolume_SymmetryCheck_QuarterAndThreeQuarterShouldSumToTotal(HeadTanksTypes headType)
    {
        double diameter = 2.0;
        double length = 5.0;
        double h1 = 0.25 * diameter;
        double h2 = 0.75 * diameter;

        var tank1 = new HorizontalCylindricalTankVolume(diameter, length, headType, productHeight: h1);
        var tank2 = new HorizontalCylindricalTankVolume(diameter, length, headType, productHeight: h2);

        // V(0.25 D) + V(0.75 D) must be exactly equal to V_total due to axial symmetry
        double sum = tank1.ProductVolume + tank2.ProductVolume;
        Assert.Equal(tank1.InternalVolume, sum, PrecisionTolerance);
    }

    [Fact]
    public void ProductVolume_ShouldThrow_WhenProductHeightExceedsDiameter()
    {
        double diameter = 2.0;
        double length = 5.0;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new HorizontalCylindricalTankVolume(
                internalDiameter: diameter,
                internalBodyLength: length,
                headType: HeadTanksTypes.Hemispherical,
                productHeight: 2.1));
    }

    [Fact]
    public void ProductVolume_ShouldThrow_WhenProductHeightIsNegative()
    {
        double diameter = 2.0;
        double length = 5.0;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new HorizontalCylindricalTankVolume(
                internalDiameter: diameter,
                internalBodyLength: length,
                headType: HeadTanksTypes.Hemispherical,
                productHeight: -0.1));
    }

    [Fact]
    public void EllipsoidalHead_WithCustomHeight_ShouldCalculateCorrectly()
    {
        double diameter = 2.0;
        double customHeight = 0.8; // different from default D/4 = 0.5

        var tank = new HorizontalCylindricalTankVolume(
            internalDiameter: diameter,
            internalBodyLength: 4.0,
            headType: HeadTanksTypes.Ellipsoidal,
            headHeight: customHeight,
            productHeight: diameter);

        double expectedHeadVolume = Math.PI * Math.Pow(diameter, 2) * customHeight / 6.0;
        Assert.Equal(expectedHeadVolume, tank.HeadVolume, PrecisionTolerance);
        Assert.Equal(tank.InternalVolume, tank.ProductVolume, PrecisionTolerance);
    }
}
