using EngCalc.Domain.StorageTanks.Entities;
using EngCalc.Domain.StorageTanks.Enums;

namespace EngCalc.Tests.StorageTanks;

public class VerticalCylindricalTankVolumeTests
{
    [Fact]
    public void VolumeConstructor_ShouldCalculateInternalVolume()
    {
        var tank = new VerticalCylindricalTankVolume(10, 20, 30);

        Assert.Equal(10, tank.BodyVolume);
        Assert.Equal(20, tank.HeadVolume);
        Assert.Equal(30, tank.BottomVolume);
        Assert.Equal(60, tank.InternalVolume);
        Assert.Equal(0, tank.ProductVolume);
    }

    [Fact]
    public void GeometricConstructor_ShouldCalculateBodyHeadBottomAndInternalVolumes()
    {
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 10,
            internalBodyHeight: 20,
            bottomType: HeadTanksTypes.Flat,
            bottomHeight: null,
            bottomAngle: null,
            headType: HeadTanksTypes.Spherical,
            headHeight: null,
            headAngle: null);

        var bodyVolume = Math.PI * 25 * 20;
        var sphericalVolume = Math.PI * 125 * 2 / 3;

        Assert.Equal(bodyVolume, tank.BodyVolume);
        Assert.Equal(sphericalVolume, tank.HeadVolume);
        Assert.Equal(0, tank.BottomVolume);
        Assert.Equal(bodyVolume + sphericalVolume, tank.InternalVolume);
        Assert.Equal(0, tank.ProductVolume);
    }

    [Fact]
    public void ProductConstructor_ShouldCalculateProductVolume()
    {
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 10,
            internalBodyHeight: 20,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: 5,
            bottomAngle: null,
            headType: HeadTanksTypes.Flat,
            headHeight: null,
            headAngle: null,
            productHeight: 8);

        var bodyVolume = Math.PI * 25 * 20;
        var bottomVolume = Math.PI * 25 * 5 / 3;
        var productVolume = Math.PI * 25 * (8 - 5) + bottomVolume;

        Assert.Equal(bodyVolume, tank.BodyVolume);
        Assert.Equal(0, tank.HeadVolume);
        Assert.Equal(bottomVolume, tank.BottomVolume);
        Assert.Equal(bodyVolume + bottomVolume, tank.InternalVolume);
        Assert.Equal(productVolume, tank.ProductVolume);
    }

    [Fact]
    public void ProductConstructor_ShouldNotExceedInternalVolume_WhenProductHeightExceedsBodyHeight()
    {
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 1000,
            internalBodyHeight: 1000,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: null,
            bottomAngle: 45,
            headType: HeadTanksTypes.Flat,
            headHeight: null,
            headAngle: null,
            productHeight: 1200);

        Assert.Equal(
            tank.BottomVolume + Math.PI * 500 * 500 * 700,
            tank.ProductVolume,
            1e-6);
        Assert.True(tank.ProductVolume <= tank.InternalVolume);
    }

    [Fact]
    public void ProductConstructor_ShouldFillBottomBodyAndPartialTopHead()
    {
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 1000,
            internalBodyHeight: 1000,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: 200,
            bottomAngle: null,
            headType: HeadTanksTypes.Conical,
            headHeight: 200,
            headAngle: null,
            productHeight: 1300);

        var expectedBottomVolume = Math.PI * 500 * 500 * 200 / 3;
        var expectedBodyVolume = Math.PI * 500 * 500 * 1000;
        var expectedHeadVolume =
            Math.PI * 500 * 500 * 200 / 3 * (1 - Math.Pow(1 - 100d / 200, 3));

        Assert.Equal(
            expectedBottomVolume + expectedBodyVolume + expectedHeadVolume,
            tank.ProductVolume,
            1e-6);
        Assert.True(tank.ProductVolume < tank.InternalVolume);
    }

    [Fact]
    public void ProductConstructor_ShouldMeasureProductHeightFromBottomOfTank()
    {
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 10,
            internalBodyHeight: 20,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: 5,
            bottomAngle: null,
            headType: HeadTanksTypes.Flat,
            headHeight: null,
            headAngle: null,
            productHeight: 5);

        Assert.Equal(tank.BottomVolume, tank.ProductVolume, 1e-9);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Flat, null, null, 0)]
    [InlineData(HeadTanksTypes.Hemispherical, null, null, Math.PI * 125 * 2 / 3)]
    [InlineData(HeadTanksTypes.Ellipsoidal, null, null, Math.PI * 100 * 2.5 / 6)]
    [InlineData(HeadTanksTypes.Torispherical, null, null, 0.0847 * 1000)]
    [InlineData(HeadTanksTypes.Conical, 5d, null, Math.PI * 25 * 5 / 3)]
    public void CalculateHeadVolume_ShouldReturnExpectedVolume(
        HeadTanksTypes type,
        double? height,
        double? angle,
        double expected)
    {
        var result = VerticalCylindricalTankVolume.CalculateHeadVolume(type, 10, height, angle);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateHeadVolume_ShouldSupportSphericalAlias()
    {
        var result = VerticalCylindricalTankVolume.CalculateHeadVolume(HeadTanksTypes.Spherical, 10, null, null);
        Assert.Equal(Math.PI * 125 * 2 / 3, result);
    }

    [Fact]
    public void CalculateHeadVolume_ShouldCalculateConicalVolumeFromAngle()
    {
        var angle = Math.Atan(5 / 5d) * 180 / Math.PI;

        var result = VerticalCylindricalTankVolume.CalculateHeadVolume(
            HeadTanksTypes.Conical, 10, null, angle);

        Assert.Equal(Math.PI * 25 * 5 / 3, result);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Flat, 0)]
    [InlineData(HeadTanksTypes.Hemispherical, Math.PI * 125 * 2 / 3)]
    [InlineData(HeadTanksTypes.Ellipsoidal, Math.PI * 100 * 2.5 / 6)]
    [InlineData(HeadTanksTypes.Torispherical, 0.0847 * 1000)]
    [InlineData(HeadTanksTypes.Conical, Math.PI * 25 * 5 / 3)]
    public void CalculateBottomVolume_ShouldReturnExpectedVolume(
        HeadTanksTypes type,
        double expected)
    {
        double? height = type == HeadTanksTypes.Conical ? 5 : null;

        var result = VerticalCylindricalTankVolume.CalculateBottomVolume(
            type, 10, height, null);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateInternalVolume_ShouldReturnSumOfParts()
    {
        var tank = new VerticalCylindricalTankVolume(10, 20, 30);

        Assert.Equal(60, tank.CalculateInternalVolume());
    }

    [Fact]
    public void CalculateProductVolume_ShouldReturnBodyAndBottomVolume()
    {
        var result = VerticalCylindricalTankVolume.CalculateProductVolume(
            internalDiameter: 10,
            productHeight: 8,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: 5,
            bottomAngle: null);

        Assert.Equal(Math.PI * 25 * (8 - 5) + Math.PI * 25 * 5 / 3, result);
    }

    [Theory]
    [InlineData(0, 20, "internalDiameter")]
    [InlineData(-1, 20, "internalDiameter")]
    [InlineData(10, 0, "internalBodyHeight")]
    [InlineData(10, -1, "internalBodyHeight")]
    public void GeometricConstructor_ShouldThrowForInvalidDimensions(
        double diameter,
        double bodyHeight,
        string parameterName)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new VerticalCylindricalTankVolume(
                diameter,
                bodyHeight,
                HeadTanksTypes.Flat,
                null,
                null,
                HeadTanksTypes.Flat,
                null,
                null));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ProductConstructor_ShouldThrowForInvalidProductHeight(double productHeight)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new VerticalCylindricalTankVolume(
                10,
                20,
                HeadTanksTypes.Flat,
                null,
                null,
                HeadTanksTypes.Flat,
                null,
                null,
                productHeight));

        Assert.Equal("productHeight", exception.ParamName);
    }

    [Fact]
    public void ProductConstructor_WhenLiquidExceedsBodyHeight_ShouldFillIntoTopHeadAndFlagExceedsWeldSeamLine()
    {
        // Tank with diameter 10 (R=5), body height 20, hemispherical top head (head height 5). Total height = 25.
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 10,
            internalBodyHeight: 20,
            bottomType: HeadTanksTypes.Flat,
            bottomHeight: null,
            bottomAngle: null,
            headType: HeadTanksTypes.Hemispherical,
            headHeight: null,
            headAngle: null,
            productHeight: 22);

        Assert.True(tank.ExceedsWeldSeamLine);
        Assert.Equal(25, tank.TotalTankHeight);

        // Expected: bottom (0) + full body (pi * 25 * 20) + partial head at z=2: pi * 2 * (25 - 4/3)
        double expectedBody = Math.PI * 25 * 20;
        double expectedHeadPartial = Math.PI * 2 * (25 - 4.0 / 3.0);
        Assert.Equal(expectedBody + expectedHeadPartial, tank.ProductVolume, 1e-9);
    }

    [Fact]
    public void ProductConstructor_WhenLiquidFillsToTop_ShouldEqualInternalVolume()
    {
        // Total tank height = 20 + 5 = 25
        var tank = new VerticalCylindricalTankVolume(
            internalDiameter: 10,
            internalBodyHeight: 20,
            bottomType: HeadTanksTypes.Flat,
            bottomHeight: null,
            bottomAngle: null,
            headType: HeadTanksTypes.Hemispherical,
            headHeight: null,
            headAngle: null,
            productHeight: 25);

        Assert.True(tank.ExceedsWeldSeamLine);
        Assert.Equal(tank.InternalVolume, tank.ProductVolume, 1e-9);
    }

    [Fact]
    public void ProductConstructor_ShouldThrow_WhenProductHeightExceedsTotalTankHeight()
    {
        // Total tank height = 20 + 5 = 25. Passing 25.1 must throw.
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new VerticalCylindricalTankVolume(
                internalDiameter: 10,
                internalBodyHeight: 20,
                bottomType: HeadTanksTypes.Flat,
                bottomHeight: null,
                bottomAngle: null,
                headType: HeadTanksTypes.Hemispherical,
                headHeight: null,
                headAngle: null,
                productHeight: 25.1));

        Assert.Equal("productHeight", exception.ParamName);
        Assert.Contains("total tank height", exception.Message);
    }

    [Fact]
    public void CalculateHeadVolume_ShouldThrowForInvalidType()
    {
        Assert.Throws<ArgumentException>(() =>
            VerticalCylindricalTankVolume.CalculateHeadVolume(
                (HeadTanksTypes)999, 10, null, null));
    }

    [Fact]
    public void CalculateBottomVolume_ShouldThrowForInvalidType()
    {
        Assert.Throws<ArgumentException>(() =>
            VerticalCylindricalTankVolume.CalculateBottomVolume(
                (HeadTanksTypes)999, 10, null, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateHeadVolume_ShouldThrowForInvalidDiameter(double diameter)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VerticalCylindricalTankVolume.CalculateHeadVolume(
                HeadTanksTypes.Flat, diameter, null, null));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(5d, 45d)]
    public void CalculateHeadVolume_ShouldThrowWhenConicalParametersAreInvalid(
        double? height,
        double? angle)
    {
        Assert.Throws<ArgumentException>(() =>
            VerticalCylindricalTankVolume.CalculateHeadVolume(
                HeadTanksTypes.Conical, 10, height, angle));
    }

    [Fact]
    public void CalculateProductVolume_ShouldThrowForInvalidProductHeight()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VerticalCylindricalTankVolume.CalculateProductVolume(
                10, 0, HeadTanksTypes.Flat, null, null));
    }
}
