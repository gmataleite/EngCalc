using EngCalc.Application.StorageTanks.DTOs;
using EngCalc.Application.StorageTanks.Services;

namespace EngCalc.Tests.StorageTanks.Application;

public class TankVolumeServiceTests
{
    private readonly TankVolumeService _service = new();

    [Fact]
    public void GetOptions_ShouldExposeAllHeadTypesAndOrientations()
    {
        var options = _service.GetOptions();

        Assert.Contains(options.TankHeadTypes, option => option.Id == "Conical");
        Assert.Contains(options.TankHeadTypes, option => option.Id == "Hemispherical");
        Assert.Contains(options.TankOrientationTypes, option => option.Id == "Vertical");
        Assert.Contains(options.TankOrientationTypes, option => option.Id == "Horizontal");
    }

    [Fact]
    public async Task CalculateVolumeAsync_ShouldCalculateHorizontalTank()
    {
        var result = await _service.CalculateVolumeAsync(new TankVolumeRequestDto(
            1000, 2000, "Hemispherical", null, null, "Hemispherical", null, null, 500, "Horizontal"));

        Assert.True(result.InternalVolume > 0);
        Assert.True(result.ProductVolume > 0);
        Assert.True(result.ProductVolume < result.InternalVolume);
    }

    [Theory]
    [InlineData(0, 1000)]
    [InlineData(1000, 0)]
    public async Task CalculateVolumeAsync_ShouldRejectInvalidDimensions(
        double diameter,
        double bodyHeight)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _service.CalculateVolumeAsync(new TankVolumeRequestDto(
                diameter, bodyHeight, "Flat", null, null, "Flat", null, null, null, "Vertical")));
    }

    [Fact]
    public async Task CalculateVolumeAsync_ShouldRejectMissingHeadType()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CalculateVolumeAsync(new TankVolumeRequestDto(
                1000, 1000, string.Empty, null, null, "Flat", null, null, null, "Vertical")));
    }
}
