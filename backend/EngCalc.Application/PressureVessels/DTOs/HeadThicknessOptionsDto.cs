using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.PressureVessels.DTOs;

public record HeadThicknessOptionsDto(
    IReadOnlyList<SelectOptionDto> HeadTypes,
    IReadOnlyList<SelectOptionDto> JointTypes,
    IReadOnlyList<SelectOptionDto> RtOptions
);

