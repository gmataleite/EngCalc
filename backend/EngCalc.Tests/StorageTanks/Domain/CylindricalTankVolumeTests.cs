using EngCalc.Domain.StorageTanks.Entities;
using EngCalc.Domain.StorageTanks.Enums;

namespace EngCalc.Tests.StorageTanks;

public class CylindricalTankVolumeTests
{
    [Fact]
    public void VolumeConstructor_ShouldCalculateInternalVolume()
    {
        var tank = new CylindricalTankVolume(10, 20, 30);

        Assert.Equal(10, tank.BodyVolume);
        Assert.Equal(20, tank.HeadVolume);
        Assert.Equal(30, tank.BottomVolume);
        Assert.Equal(60, tank.InternalVolume);
        Assert.Equal(0, tank.ProductVolume);
    }

    [Fact]
    public void GeometricConstructor_ShouldCalculateBodyHeadBottomAndInternalVolumes()
    {
        var tank = new CylindricalTankVolume(
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
        var tank = new CylindricalTankVolume(
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
        var productVolume = Math.PI * 25 * 8 + bottomVolume;

        Assert.Equal(bodyVolume, tank.BodyVolume);
        Assert.Equal(0, tank.HeadVolume);
        Assert.Equal(bottomVolume, tank.BottomVolume);
        Assert.Equal(bodyVolume + bottomVolume, tank.InternalVolume);
        Assert.Equal(productVolume, tank.ProductVolume);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Flat, null, null, 0)]
    [InlineData(HeadTanksTypes.Spherical, null, null, Math.PI * 125 * 2 / 3)]
    [InlineData(HeadTanksTypes.Conical, 5d, null, Math.PI * 25 * 5 / 3)]
    public void CalculateHeadVolume_ShouldReturnExpectedVolume(
        HeadTanksTypes type,
        double? height,
        double? angle,
        double expected)
    {
        var result = CylindricalTankVolume.CalculateHeadVolume(type, 10, height, angle);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateHeadVolume_ShouldCalculateConicalVolumeFromAngle()
    {
        var angle = Math.Atan(5 / 5d) * 180 / Math.PI;

        var result = CylindricalTankVolume.CalculateHeadVolume(
            HeadTanksTypes.Conical, 10, null, angle);

        Assert.Equal(Math.PI * 25 * 5 / 3, result);
    }

    [Theory]
    [InlineData(HeadTanksTypes.Flat, 0)]
    [InlineData(HeadTanksTypes.Spherical, Math.PI * 125 * 2 / 3)]
    [InlineData(HeadTanksTypes.Conical, Math.PI * 25 * 5 / 3)]
    public void CalculateBottomVolume_ShouldReturnExpectedVolume(
        HeadTanksTypes type,
        double expected)
    {
        double? height = type == HeadTanksTypes.Conical ? 5 : null;

        var result = CylindricalTankVolume.CalculateBottomVolume(
            type, 10, height, null);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateInternalVolume_ShouldReturnSumOfParts()
    {
        var tank = new CylindricalTankVolume(10, 20, 30);

        Assert.Equal(60, tank.CalculateInternalVolume());
    }

    [Fact]
    public void CalculateProductVolume_ShouldReturnBodyAndBottomVolume()
    {
        var result = CylindricalTankVolume.CalculateProductVolume(
            internalDiameter: 10,
            productHeight: 8,
            bottomType: HeadTanksTypes.Conical,
            bottomHeight: 5,
            bottomAngle: null);

        Assert.Equal(Math.PI * 25 * 8 + Math.PI * 25 * 5 / 3, result);
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
            new CylindricalTankVolume(
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
            new CylindricalTankVolume(
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
    public void CalculateHeadVolume_ShouldThrowForInvalidType()
    {
        Assert.Throws<ArgumentException>(() =>
            CylindricalTankVolume.CalculateHeadVolume(
                (HeadTanksTypes)999, 10, null, null));
    }

    [Fact]
    public void CalculateBottomVolume_ShouldThrowForInvalidType()
    {
        Assert.Throws<ArgumentException>(() =>
            CylindricalTankVolume.CalculateBottomVolume(
                (HeadTanksTypes)999, 10, null, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateHeadVolume_ShouldThrowForInvalidDiameter(double diameter)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CylindricalTankVolume.CalculateHeadVolume(
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
            CylindricalTankVolume.CalculateHeadVolume(
                HeadTanksTypes.Conical, 10, height, angle));
    }

    [Fact]
    public void CalculateProductVolume_ShouldThrowForInvalidProductHeight()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CylindricalTankVolume.CalculateProductVolume(
                10, 0, HeadTanksTypes.Flat, null, null));
    }
}
