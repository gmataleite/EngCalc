namespace EngCalc.Application.Converters.DTOs;

public record TemperatureConverterRequestDto(
    double InitialValue,
    string InitialUnit,
    string TargetUnit
);

