namespace EngCalc.Application.Converters.DTOs;

public record ConverterRequestDto(
    string PhysicalPropertyType,
    double InitialValue,
    string InitialUnit,
    string TargetUnit
);

