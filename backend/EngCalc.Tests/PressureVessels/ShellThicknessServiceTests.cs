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

public class ShellThicknessServiceTests
{
    private readonly Mock<IMaterialProvider> _materialProviderMock;
    private readonly ShellThicknessService _sut;

    public ShellThicknessServiceTests()
    {
        _materialProviderMock = new Mock<IMaterialProvider>();

        var sampleMaterial = new Material("SA-516-70", "SA-516", "70", "Plate", 485m, 260m, 425m);
        sampleMaterial.AddTemperaturePoint(40m, 138m, 260m);

        _materialProviderMock
            .Setup(p => p.GetMaterialByIdAsync("SA-516-70", It.IsAny<CancellationToken>()))
            .ReturnsAsync(sampleMaterial);

        _sut = new ShellThicknessService(_materialProviderMock.Object);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_ThinShell_ShouldReturnExpectedMawp()
    {
        // Arrange
        var request = new ShellThicknessRequestDto(
            UnitSystem: "metric",
            ShellType: "cylindrical",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InsideRadius: 1000.0,
            InternalPressure: null,
            Thickness: 17.6199,
            JointTypeId: "type_1",
            RtId: "full",
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null);

        // Act
        var response = await _sut.CalculateAsync(request);

        // Assert
        Assert.Equal("mawp", response.Mode);
        Assert.NotNull(response.GoverningMawp);
        Assert.NotNull(response.CircumferentialMawp);
        Assert.NotNull(response.LongitudinalMawp);
        Assert.Equal(2.0, response.GoverningMawp.Value, 3);
        Assert.Equal(14.6199, response.CorrodedThickness!.Value, 4);
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_ThickShell_ShouldReturnExpectedMawpAndWarning()
    {
        // Arrange
        var request = new ShellThicknessRequestDto(
            UnitSystem: "metric",
            ShellType: "cylindrical",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InsideRadius: 1000.0,
            InternalPressure: null,
            Thickness: 593.2550,
            JointTypeId: "type_1",
            RtId: "full",
            CorrosionAllowance: 0.0,
            AllowableStressOverride: null);

        // Act
        var response = await _sut.CalculateAsync(request);

        // Assert
        Assert.Equal("mawp", response.Mode);
        Assert.Equal(60.0, response.GoverningMawp!.Value, 3);
        Assert.Equal("Thick Wall (App. 1-2)", response.CircumferentialFormulaType);
        Assert.Contains(response.Warnings, w => w.Contains("Thick-wall shell threshold exceeded"));
    }

    [Fact]
    public async Task CalculateAsync_MawpMode_WhenThicknessIsLessThanOrEqualToCa_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var request = new ShellThicknessRequestDto(
            UnitSystem: "metric",
            ShellType: "cylindrical",
            Mode: "mawp",
            MaterialId: "SA-516-70",
            DesignTemperature: 40.0,
            InsideRadius: 1000.0,
            InternalPressure: null,
            Thickness: 3.0,
            JointTypeId: "type_1",
            RtId: "full",
            CorrosionAllowance: 3.0,
            AllowableStressOverride: null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.CalculateAsync(request));
    }
}

