using EngCalc.Application.Converters.DTOs;

namespace EngCalc.Application.Converters.Interfaces;

public interface IConverterService
{
    ConverterOptionsDto GetOptions();
    Task<ConverterResponseDto> ConvertAsync(ConverterRequestDto request);
}

