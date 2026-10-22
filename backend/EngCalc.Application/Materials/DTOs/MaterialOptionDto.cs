namespace EngCalc.Application.Materials.DTOs;

public record MaterialOptionDto(
    string Id,
    string Label,
    string Specification,
    string? Grade,
    string? ProductForm,
    decimal MaxTemperatureLimit
);

