using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.PressureVessels.DTOs;
using EngCalc.Application.PressureVessels.Services;
using EngCalc.Domain.Entities;

namespace EngCalc.Tests.PressureVessels;

public class HeadThicknessServiceTests
{
    private readonly Mock<IMaterialProvider> _materialProviderMock;
    private readonly HeadThicknessService _sut;

    public HeadThicknessServiceTests()
    {
        _materialProviderMock = new Mock<IMaterialProvider>();

        var sampleMaterial = new Material("SA-516-70", "SA-516", "70", "Plate", 485m, 260m, 425m);
        sampleMaterial.AddTemperaturePoint(40m, 138m, 260m);

        _materialProviderMock
            .Setup(p => p.GetMaterialByIdAsync("SA-516-70", It.IsAny<CancellationToken>()))
            .ReturnsAsync(sampleMaterial);

        _sut = new HeadThicknessService(_materialProviderMock.Object);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_EllipsoidalHead_ShouldReturnExpectedMawp()
    {
        // Arrange
        var request = new HeadThicknessRequestDto(
            UnitSystem: "metric",
            HeadType: "ellipsoidal",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InternalPressure: null,
            InsideDiameter: 2000.0,
            HeadHeight: null,
            CrownRadius: null,
            KnuckleRadius: null,
            InsideRadius: null,
            JointTypeId: "type_1",
            RtId: "spot", // E = 0.85
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null,
            Thickness: 20.0794);

        // Act
        var response = await _sut.CalculateAsync(request);

        // Assert
        Assert.Equal("mawp", response.Mode);
        Assert.NotNull(response.MaximumAllowableWorkingPressure);
        Assert.Equal(2.0, response.MaximumAllowableWorkingPressure.Value, 3);
        Assert.Equal(17.0794, response.CorrodedThickness!.Value, 4);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_TorisphericalHead_ShouldReturnExpectedMawp()
    {
        // Arrange
        var request = new HeadThicknessRequestDto(
            UnitSystem: "metric",
            HeadType: "torispherical",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InternalPressure: null,
            InsideDiameter: null,
            HeadHeight: null,
            CrownRadius: 2000.0,
            KnuckleRadius: 120.0,
            InsideRadius: null,
            JointTypeId: "type_1",
            RtId: "spot", // E = 0.85
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null,
            Thickness: 33.2406);

        // Act
        var response = await _sut.CalculateAsync(request);

        // Assert
        Assert.Equal("mawp", response.Mode);
        Assert.NotNull(response.MaximumAllowableWorkingPressure);
        Assert.Equal(2.0, response.MaximumAllowableWorkingPressure.Value, 3);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_HemisphericalHead_ShouldReturnExpectedMawp()
    {
        // Arrange
        var request = new HeadThicknessRequestDto(
            UnitSystem: "metric",
            HeadType: "hemispherical",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InternalPressure: null,
            InsideDiameter: null,
            HeadHeight: null,
            CrownRadius: null,
            KnuckleRadius: null,
            InsideRadius: 1000.0,
            JointTypeId: "type_1",
            RtId: "spot", // E = 0.85
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null,
            Thickness: 11.5397);

        // Act
        var response = await _sut.CalculateAsync(request);

        // Assert
        Assert.Equal("mawp", response.Mode);
        Assert.NotNull(response.MaximumAllowableWorkingPressure);
        Assert.Equal(2.0, response.MaximumAllowableWorkingPressure.Value, 3);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_WhenThicknessIsLessThanOrEqualToCa_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var request = new HeadThicknessRequestDto(
            UnitSystem: "metric",
            HeadType: "ellipsoidal",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InternalPressure: null,
            InsideDiameter: 1200.0,
            HeadHeight: null,
            CrownRadius: null,
            KnuckleRadius: null,
            InsideRadius: null,
            JointTypeId: "type_1",
            RtId: "full",
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null,
            Thickness: 2.5);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.CalculateAsync(request));
    }
}

