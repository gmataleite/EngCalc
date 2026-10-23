using EngCalc.Application.Converters.DTOs;
using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;
using EngCalc.Application.Converters.Interfaces;
using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.Converters.Services;

public class ConverterService : IConverterService
{
    private static readonly IReadOnlyList<SelectOptionDto> PhysicalPropertiesOptions =
    [
        new("temperature", "Temperature"),
        new("pressure", "Pressure"),
        new("volume", "Volume")
    ];

    public ConverterOptionsDto GetOptions()
    {
        return new ConverterOptionsDto(PhysicalPropertiesOptions);
    }

    public Task<ConverterResponseDto> ConvertAsync(ConverterRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!double.IsFinite(request.InitialValue))
        {
            throw new ArgumentOutOfRangeException(nameof(request.InitialValue), $"Invalid initial value: {request.InitialValue}");
        }

        if (string.IsNullOrWhiteSpace(request.PhysicalPropertyType))
        {
            throw new ArgumentException("Physical property type is required.", nameof(request.PhysicalPropertyType));
        }

        return request.PhysicalPropertyType.Trim().ToLowerInvariant() switch
        {
            "temperature" => Task.FromResult(ConvertTemperature(request)),
            _ => throw new NotSupportedException($"Physical property type '{request.PhysicalPropertyType}' is not supported.")
        };
    }

    public ConverterResponseDto ConvertTemperature(ConverterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.InitialUnit) ||
            !Enum.TryParse<TemperatureUnits>(request.InitialUnit, true, out var initialUnit) ||
            !Enum.IsDefined(typeof(TemperatureUnits), initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {request.InitialUnit}", nameof(request.InitialUnit));
        }

        if (string.IsNullOrWhiteSpace(request.TargetUnit) ||
            !Enum.TryParse<TemperatureUnits>(request.TargetUnit, true, out var targetUnit) ||
            !Enum.IsDefined(typeof(TemperatureUnits), targetUnit))
        {
            throw new ArgumentException($"Invalid target unit: {request.TargetUnit}", nameof(request.TargetUnit));
        }

        double convertedValue = new TemperatureConverter(
            request.InitialValue, initialUnit, targetUnit).ConvertedValue;

        return new ConverterResponseDto(
            ConvertedValue: convertedValue,
            ConvertedUnit: request.TargetUnit);
    }
}
