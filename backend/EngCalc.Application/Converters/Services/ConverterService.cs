using System.Globalization;
using UnitsNet;
using EngCalc.Application.Converters.DTOs;
using EngCalc.Domain.Converters.Models;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Application.Converters.Services;

public class ConverterService
{
    public async Task<TemperatureConverterResponseDto> ConvertAsync(TemperatureConverterRequestDto request)
    {
        
        if (!double.IsFinite(request.InitialValue) || request.InitialValue > double.MaxValue || request.InitialValue < double.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InitialValue), $"{nameof(request.InitialValue)} must be a finite value greater than zero.");
        }

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

        return new TemperatureConverterResponseDto(
            ConvertedValue: convertedValue,
            ConvertedUnit: request.TargetUnit);
    }
}

