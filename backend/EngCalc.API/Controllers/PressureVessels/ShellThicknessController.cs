using Microsoft.AspNetCore.Mvc;
using EngCalc.Application.PressureVessels.DTOs;
using EngCalc.Application.PressureVessels.Interfaces;

namespace EngCalc.API.Controllers.PressureVessels;

[ApiController]
[Route("api/calculations/pressure-vessels/shell-thickness")]
[Route("api/calculations/shell-thickness")]
public class ShellThicknessController : ControllerBase
{
    private readonly IShellThicknessService _shellThicknessService;

    public ShellThicknessController(IShellThicknessService shellThicknessService)
    {
        _shellThicknessService = shellThicknessService;
    }

    [HttpGet("options")]
    public ActionResult<ShellThicknessOptionsDto> GetOptions()
    {
        return Ok(_shellThicknessService.GetOptions());
    }

    [HttpPost]
    public async Task<ActionResult<ShellThicknessResponseDto>> Calculate(
        [FromBody] ShellThicknessRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shellThicknessService.CalculateAsync(request, cancellationToken);
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

