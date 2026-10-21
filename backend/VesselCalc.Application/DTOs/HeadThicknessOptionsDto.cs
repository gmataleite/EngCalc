namespace VesselCalc.Application.DTOs;

public record HeadThicknessOptionsDto(
    IReadOnlyList<SelectOptionDto> HeadTypes,
    IReadOnlyList<SelectOptionDto> JointTypes,
    IReadOnlyList<SelectOptionDto> RtOptions
);

