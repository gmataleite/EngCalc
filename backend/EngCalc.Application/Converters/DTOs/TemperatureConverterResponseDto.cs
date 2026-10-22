namespace EngCalc.Application.Converters.DTOs;

public record TemperatureConverterResponseDto(
    double ConvertedValue,
    string ConvertedUnit
);

