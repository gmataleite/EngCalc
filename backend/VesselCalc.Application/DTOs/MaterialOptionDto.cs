namespace VesselCalc.Application.DTOs;

public record MaterialOptionDto(
    string Id,
    string Label,
    string Specification,
    string? Grade,
    string? ProductForm,
    decimal MaxTemperatureLimit
);

