using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.StorageTanks.DTOs;

public record TankHeadOptionsDto(
    IReadOnlyList<SelectOptionDto> TankHeadTypes
);