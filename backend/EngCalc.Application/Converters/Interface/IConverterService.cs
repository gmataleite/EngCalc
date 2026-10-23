using EngCalc.Application.Converters.DTOs;

namespace EngCalc.Application.Converters.Interfaces;

public interface IConverterService
{
    Task<ConverterResponseDto> ConvertAsync(ConverterRequestDto request);
}

