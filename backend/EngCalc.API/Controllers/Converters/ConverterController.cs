using Microsoft.AspNetCore.Mvc;
using EngCalc.Application.Converters.DTOs;
using EngCalc.Application.Converters.Interfaces;

namespace EngCalc.API.Controllers.Converters;

[ApiController]
[Route("api/calculations/converters/")]
public class ConverterController : ControllerBase
{
    private readonly IConverterService _converterService;

    public ConverterController(IConverterService converterService)
    {
        _converterService = converterService;
    }

    [HttpPost]
    public async Task<ActionResult<ConverterResponseDto>> Calculate(
        [FromBody] ConverterRequestDto request)
    {
        try
        {
            var result = await _converterService.ConvertAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

