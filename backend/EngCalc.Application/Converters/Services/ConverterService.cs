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

    private static readonly IReadOnlyList<SelectOptionDto> TemperatureUnitsOptions = 
        Enum.GetValues<TemperatureUnits>()
            .Select(unit => new SelectOptionDto(unit.ToString(), unit switch
            {
                TemperatureUnits.Celsius => "Celsius (°C)",
                TemperatureUnits.Fahrenheit => "Fahrenheit (°F)",
                TemperatureUnits.Kelvin => "Kelvin (K)",
                _ => unit.ToString()
            }))
            .ToList();

    private static readonly IReadOnlyList<SelectOptionDto> PressureUnitsOptions =
        Enum.GetValues<PressureUnits>()
            .Select(unit => new SelectOptionDto(unit.ToString(), unit switch
            {
                PressureUnits.Pascal => "Pascal (Pa)",
                PressureUnits.Bar => "Bar (bar)",
                PressureUnits.NewtonPerSquareMeter => "Newton per square meter (N/m²)",
                PressureUnits.PoundsPerSquareInch => "Pounds per square inch (psi)",
                _ => unit.ToString()
            }))
            .ToList();

    private static readonly IReadOnlyList<SelectOptionDto> VolumeUnitsOptions =
    Enum.GetValues<VolumeUnits>()
            .Select(unit => new SelectOptionDto(unit.ToString(), unit switch
            {
                VolumeUnits.CubicMeters => "Cubic Meters (m³)",
                VolumeUnits.Liters => "Liters (L)",
                VolumeUnits.Gallons => "Gallons (gal)",
                _ => unit.ToString()
            }))
            .ToList();

    public ConverterOptionsDto GetOptions()
    {
        return new ConverterOptionsDto(PhysicalPropertiesOptions, TemperatureUnitsOptions, PressureUnitsOptions, VolumeUnitsOptions);
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

        switch (request.PhysicalPropertyType.Trim().ToLowerInvariant())
        {
            case "temperature":
                return Task.FromResult(ConvertTemperature(request));
            case "pressure":
                return Task.FromResult(ConvertPressure(request));
                    case "volume":
                        return Task.FromResult(ConvertVolume(request));
            default:
                throw new NotSupportedException($"Physical property type '{request.PhysicalPropertyType}' is not supported.");
        }
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

    public ConverterResponseDto ConvertPressure(ConverterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.InitialUnit) ||
            !Enum.TryParse<PressureUnits>(request.InitialUnit, true, out var initialUnit) ||
            !Enum.IsDefined(typeof(PressureUnits), initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {request.InitialUnit}", nameof(request.InitialUnit));
        }

        if (string.IsNullOrWhiteSpace(request.TargetUnit) ||
            !Enum.TryParse<PressureUnits>(request.TargetUnit, true, out var targetUnit) ||
            !Enum.IsDefined(typeof(PressureUnits), targetUnit))
        {
            throw new ArgumentException($"Invalid target unit: {request.TargetUnit}", nameof(request.TargetUnit));
        }

        double convertedValue = new PressureConverter(
            request.InitialValue, initialUnit, targetUnit).ConvertedValue;

        return new ConverterResponseDto(
            ConvertedValue: convertedValue,
            ConvertedUnit: request.TargetUnit);
    }

    public ConverterResponseDto ConvertVolume(ConverterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.InitialUnit) ||
            !Enum.TryParse<VolumeUnits>(request.InitialUnit, true, out var initialUnit) ||
            !Enum.IsDefined(typeof(VolumeUnits), initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {request.InitialUnit}", nameof(request.InitialUnit));
        }

        if (string.IsNullOrWhiteSpace(request.TargetUnit) ||
            !Enum.TryParse<VolumeUnits>(request.TargetUnit, true, out var targetUnit) ||
            !Enum.IsDefined(typeof(VolumeUnits), targetUnit))
        {
            throw new ArgumentException($"Invalid target unit: {request.TargetUnit}", nameof(request.TargetUnit));
        }

        double convertedValue = new VolumeConverter(
            request.InitialValue, initialUnit, targetUnit).ConvertedValue;

        return new ConverterResponseDto(
            ConvertedValue: convertedValue,
            ConvertedUnit: request.TargetUnit);
    }
}
