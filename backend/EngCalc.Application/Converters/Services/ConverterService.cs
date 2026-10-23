using System.Globalization;
using UnitsNet;
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

    public async Task<ConverterResponseDto> ConvertAsync(ConverterRequestDto request)
    {
        
        if (!double.IsFinite(request.InitialValue) || request.InitialValue > double.MaxValue || request.InitialValue < double.MinValue)
        {
            throw new ArgumentOutOfRangeException($"Invalid initial value: {request.InitialValue}", nameof(request.InitialValue));
        }

        switch (request.PhysicalPropertyType.ToLowerInvariant())
        {
            case "temperature":
                return ConvertTemperature(request);
            default:
                throw new NotSupportedException($"Physical property type '{request.PhysicalPropertyType}' is not supported.");
        }
    }

    public ConverterResponseDto ConvertTemperature(ConverterRequestDto request)
    {
        if (!Enum.TryParse<TemperatureUnits>(request.InitialUnit, true, out var initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {request.InitialUnit}", nameof(request.InitialUnit));
        }

        if (!Enum.TryParse<TemperatureUnits>(request.TargetUnit, true, out var targetUnit))
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

