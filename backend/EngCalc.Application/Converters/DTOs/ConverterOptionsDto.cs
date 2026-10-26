using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.Converters.DTOs;

public record ConverterOptionsDto(
    IReadOnlyList<SelectOptionDto> PhysicalPropertiesTypes,
    IReadOnlyList<SelectOptionDto> TemperatureUnits,
    IReadOnlyList<SelectOptionDto> PressureUnits
    // IReadOnlyList<SelectOptionDto> VolumeUnits
);