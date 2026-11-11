using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.StorageTanks.DTOs;

public record TankOptionsDto(
    IReadOnlyList<SelectOptionDto> TankHeadTypes,
    IReadOnlyList<SelectOptionDto> TankOrientationTypes
);