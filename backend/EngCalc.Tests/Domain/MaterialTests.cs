using EngCalc.Domain.Entities;

namespace EngCalc.Tests.Domain;

public class MaterialTests
{
    [Fact]
    public void GetAllowableStress_ShouldInterpolateBetweenTemperaturePoints()
    {
        var material = new Material("M-1", "SA-516", "70", "Plate", 485, 260, 500);
        material.AddTemperaturePoint(100, 138, 260);
        material.AddTemperaturePoint(200, 120, 240);

        var result = material.GetAllowableStress(150);

        Assert.Equal(129, result);
    }

    [Fact]
    public void GetAllowableStress_ShouldUseBoundaryValuesOutsideLowerRange()
    {
        var material = new Material("M-1", "SA-516", null, null, 485, 260, 500);
        material.AddTemperaturePoint(100, 138, null);

        Assert.Equal(138, material.GetAllowableStress(-20));
        Assert.Equal(138, material.GetAllowableStress(100));
        Assert.Null(material.GetAllowableStress(501));
    }

    [Fact]
    public void GetAllowableStress_ShouldReturnNullWhenNoAllowableStressExists()
    {
        var material = new Material("M-1", "SA-516", null, null, 485, 260, 500);
        material.AddTemperaturePoint(100, null, 260);

        Assert.Null(material.GetAllowableStress(100));
    }

    [Fact]
    public void AddTemperaturePoint_ShouldRejectDuplicateTemperature()
    {
        var material = new Material("M-1", "SA-516", null, null, 485, 260, 500);
        material.AddTemperaturePoint(100, 138, 260);

        Assert.Throws<InvalidOperationException>(() =>
            material.AddTemperaturePoint(100, 130, 250));
    }
}
