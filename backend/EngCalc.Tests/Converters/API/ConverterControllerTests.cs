using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Xunit;
using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.Converters.Interfaces;

namespace EngCalc.Tests.Converters.API;

public class ConverterControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EndpointUrl = "api/calculations/converters/";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IConverterService> _mockConverterService;

    public ConverterControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockConverterService = new Mock<IConverterService>();

        // Customiza a fábrica para injetar o Mock no lugar do serviço real
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove o registro original do serviço se ele existir
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IConverterService));
                if (descriptor != null) services.Remove(descriptor);

                // Adiciona o Mock
                services.AddScoped(_ => _mockConverterService.Object);
            });
        });
    }

    [Fact]
    public async Task Calculate_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestDto = new ConverterRequestDto("temperature", 25.0, "Celsius", "Fahrenheit");
        var expectedResponse = new ConverterResponseDto(77.0, "Fahrenheit");

        _mockConverterService
            .Setup(s => s.ConvertAsync(It.IsAny<ConverterRequestDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await client.PostAsJsonAsync(EndpointUrl, requestDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultDto = await response.Content.ReadFromJsonAsync<ConverterResponseDto>();
        Assert.NotNull(resultDto);
        Assert.Equal(expectedResponse.ConvertedValue, resultDto.ConvertedValue);
        Assert.Equal(expectedResponse.ConvertedUnit, resultDto.ConvertedUnit);
        _mockConverterService.Verify(s => s.ConvertAsync(requestDto), Times.Once);
    }

    [Fact]
    public async Task Calculate_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestDto = new ConverterRequestDto("temperature", 25.0, "Celsius", "Fahrenheit");

        _mockConverterService
            .Setup(s => s.ConvertAsync(It.IsAny<ConverterRequestDto>()))
            .ThrowsAsync(new KeyNotFoundException("Conversão não encontrada."));

        // Act
        var response = await client.PostAsJsonAsync(EndpointUrl, requestDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseMock>();
        Assert.Equal("Conversão não encontrada.", errorResponse?.Message);
    }

    [Fact]
    public async Task Calculate_ShouldReturnBadRequest_WhenNotSupportedExceptionIsThrown()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestDto = new ConverterRequestDto("temperature", 25.0, "Celsius", "Fahrenheit");

        _mockConverterService
            .Setup(s => s.ConvertAsync(It.IsAny<ConverterRequestDto>()))
            .ThrowsAsync(new NotSupportedException("Tipo de conversão não suportado."));

        // Act
        var response = await client.PostAsJsonAsync(EndpointUrl, requestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseMock>();
        Assert.Equal("Tipo de conversão não suportado.", errorResponse?.Message);
    }

    [Fact]
    public async Task Calculate_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestDto = new ConverterRequestDto("temperature", 25.0, "InvalidUnit", "Fahrenheit");

        _mockConverterService
            .Setup(s => s.ConvertAsync(It.IsAny<ConverterRequestDto>()))
            .ThrowsAsync(new ArgumentException("Unidade inicial inválida."));

        // Act
        var response = await client.PostAsJsonAsync(EndpointUrl, requestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseMock>();
        Assert.Equal("Unidade inicial inválida.", errorResponse?.Message);
    }

    [Fact]
    public async Task Calculate_ShouldReturnBadRequest_WhenInvalidOperationExceptionIsThrown()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestDto = new ConverterRequestDto("temperature", 25.0, "Celsius", "Fahrenheit");

        _mockConverterService
            .Setup(s => s.ConvertAsync(It.IsAny<ConverterRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Operação de conversão inválida."));

        // Act
        var response = await client.PostAsJsonAsync(EndpointUrl, requestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseMock>();
        Assert.Equal("Operação de conversão inválida.", errorResponse?.Message);
    }

    // Classe auxiliar para deserializar as respostas de erro anônimas do catch
    private class ErrorResponseMock
    {
        public string Message { get; set; } = string.Empty;
    }
}
