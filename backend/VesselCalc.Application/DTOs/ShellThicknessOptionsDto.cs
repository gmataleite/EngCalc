namespace VesselCalc.Application.DTOs;

public record SelectOptionDto(string Id, string Label);

public record ShellThicknessOptionsDto(
    IReadOnlyList<SelectOptionDto> JointTypes,
    IReadOnlyList<SelectOptionDto> RtOptions
);

