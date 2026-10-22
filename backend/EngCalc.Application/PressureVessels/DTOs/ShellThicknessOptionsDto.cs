using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.PressureVessels.DTOs;

public record ShellThicknessOptionsDto(
    IReadOnlyList<SelectOptionDto> JointTypes,
    IReadOnlyList<SelectOptionDto> RtOptions
);

