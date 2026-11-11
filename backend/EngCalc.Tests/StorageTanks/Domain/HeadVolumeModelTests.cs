using EngCalc.Domain.StorageTanks.Enums;
using EngCalc.Domain.StorageTanks.Factories;
using EngCalc.Domain.StorageTanks.Models;

namespace EngCalc.Tests.StorageTanks.Domain;

public class HeadVolumeModelTests
{
    [Theory]
    [InlineData(HeadTanksTypes.Flat, typeof(FlatHeadVolume))]
    [InlineData(HeadTanksTypes.Hemispherical, typeof(HemisphericalHeadVolume))]
    [InlineData(HeadTanksTypes.Ellipsoidal, typeof(EllipsoidalHeadVolume))]
    [InlineData(HeadTanksTypes.Torispherical, typeof(TorisphericalHeadVolume))]
    public void HeadVolumeFactory_ShouldCreateExpectedHeadType(
        HeadTanksTypes type,
        Type expectedType)
    {
        var result = HeadVolumeFactory.Create(type, 1000);

        Assert.IsType(expectedType, result);
    }

    [Fact]
    public void HeadVolumeFactory_ShouldCreateConicalHeadFromHeight()
    {
        var result = HeadVolumeFactory.Create(HeadTanksTypes.Conical, 1000, height: 200);

        Assert.IsType<ConicalHeadVolume>(result);
        Assert.Equal(Math.PI * 500 * 500 * 200 / 3, result.InternalVolume);
    }

    [Fact]
    public void HeadVolumeFactory_ShouldRejectUnknownHeadType()
    {
        Assert.Throws<ArgumentException>(() =>
            HeadVolumeFactory.Create((HeadTanksTypes)999, 1000));
    }

    [Fact]
    public void CylindricalBodyVolume_ShouldReturnFullVolumeAtFullHorizontalHeight()
    {
        var body = new CylindricalBodyVolume(100, 200);

        Assert.Equal(body.InternalVolume, body.CalculateHorizontalProductVolume(100));
        Assert.Equal(0, body.CalculateHorizontalProductVolume(0));
    }

    [Fact]
    public void DishedHeadVolume_ShouldRejectHorizontalProductHeightAboveDiameter()
    {
        var head = new HemisphericalHeadVolume(100);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            head.CalculateHorizontalProductVolume(101));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(50, 0.875)]
    [InlineData(100, 1)]
    public void ConicalHeadVolume_ShouldCalculateVerticalProductFraction(
        double height,
        double expectedFraction)
    {
        var head = new ConicalHeadVolume(100, angleToHorizontal: null, internalHeight: 100);

        Assert.Equal(
            head.InternalVolume * expectedFraction,
            head.CalculateVerticalProductVolume(height),
            10);
    }
}
