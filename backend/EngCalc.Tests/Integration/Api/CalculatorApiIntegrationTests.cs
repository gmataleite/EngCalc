using System.Net;
using System.Net.Http.Json;
using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.PressureVessels.DTOs;
using EngCalc.Application.StorageTanks.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EngCalc.Tests.Integration.Api;

public class CalculatorApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CalculatorApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Converter_ShouldReturnConvertedValue()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/converters",
            new ConverterRequestDto("temperature", 25, "Celsius", "Fahrenheit"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ConverterResponseDto>();
        Assert.NotNull(result);
        Assert.Equal(77, result.ConvertedValue, 10);
    }

    [Fact]
    public async Task Converter_ShouldReturnBadRequestForUnsupportedProperty()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/converters",
            new ConverterRequestDto("mass", 10, "Kilogram", "Gram"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Tank_ShouldReturnCalculatedVolumes()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/tanks",
            new TankVolumeRequestDto(
                1000, 1000, "Conical", 200, null, "Flat", null, null, 700, "Vertical"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TankVolumeResponseDto>();
        Assert.NotNull(result);
        Assert.True(result.InternalVolume > result.ProductVolume);
    }

    [Fact]
    public async Task Tank_ShouldReturnBadRequestForInvalidOrientation()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/tanks",
            new TankVolumeRequestDto(
                1000, 1000, "Flat", null, null, "Flat", null, null, null, "Diagonal"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PressureVesselShell_ShouldReturnCalculatedThickness()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/calculations/pressure-vessels/shell-thickness",
            new ShellThicknessRequestDto(
                "metric", "cylindrical", "min_thickness", string.Empty, 40, 1000, 2,
                null, "type_1", "full", 3, 138));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ShellThicknessResponseDto>();
        Assert.NotNull(result);
        Assert.True(result.GoverningRequiredThickness > 0);
    }

    [Fact]
    public async Task PressureVesselHead_ShouldReturnBadRequestForUnsupportedType()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/calculations/pressure-vessels/head-thickness",
            new HeadThicknessRequestDto(
                "metric", "conical", "min_thickness", string.Empty, 40, 2,
                2000, null, null, null, null, "type_1", "full", 3, 138));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
