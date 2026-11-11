using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.StorageTanks.DTOs;

namespace EngCalc.Application.Converters.Interfaces;

public interface IConverterService
{
    ConverterOptionsDto GetOptions();
    Task<ConverterResponseDto> ConvertAsync(ConverterRequestDto request);
}

