using EngCalc.Application.Materials.Services;
using EngCalc.Domain.Entities;
using EngCalc.Application.Materials.Interfaces;
using Moq;

namespace EngCalc.Tests.Materials.Application;

public class MaterialServiceTests
{
    [Fact]
    public async Task GetMaterialsAsync_ShouldSortAndFormatMaterialOptions()
    {
        var provider = new Mock<IMaterialProvider>();
        provider
            .Setup(x => x.GetAllMaterialsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new Material("B-2", "SA-516", "70", "Plate", 485, 260, 500),
                new Material("A-1", "SA-240", null, "Sheet", 485, 260, 500)
            ]);

        var result = await new MaterialService(provider.Object).GetMaterialsAsync();

        Assert.Equal(["A-1", "B-2"], result.Select(x => x.Id));
        Assert.Equal("ASTM/ASME SA-240 — Carbon/Alloy Steel Sheet", result[0].Label);
        Assert.Equal("ASTM/ASME SA-516 Gr. 70 — Carbon/Alloy Steel Plate", result[1].Label);
    }

}
