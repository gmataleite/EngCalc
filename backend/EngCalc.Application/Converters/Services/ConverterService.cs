using System.Globalization;
using UnitsNet;
using EngCalc.Application.Common.DTOs;
using EngCalc.Application.Materials.Interfaces;
using EngCalc.Application.Converters.DTOs;
using EngCalc.Domain.Converters;
using EngCalc.Domain.Converters.Enums;

namespace EngCalc.Application.Converters.Services;

public class ConvertTemperatureService
{
    public async Task<TemperatureConverterResponseDto> CalculateAsync(
        TemperatureConverterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        double convertedValue = new TemperatureConverter(
            request.InitialValue,
            Enum.Parse<TemperatureUnits>(request.InitialUnit),
            Enum.Parse<TemperatureUnits>(request.targetUnit)).ConvertedValue;

        return new TemperatureConverterResponseDto(
            ConvertedValue: convertedValue,
            ConvertedUnit: request.targetUnit);
    }
}

