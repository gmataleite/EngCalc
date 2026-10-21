using Microsoft.AspNetCore.Mvc;
using VesselCalc.Application.DTOs;
using VesselCalc.Application.Interfaces;

namespace VesselCalc.API.Controllers;

[ApiController]
[Route("api/calculations/head-thickness")]
public class HeadThicknessController : ControllerBase
{
    private readonly IHeadThicknessService _headThicknessService;

    public HeadThicknessController(IHeadThicknessService headThicknessService)
    {
        _headThicknessService = headThicknessService;
    }

    [HttpGet("options")]
    public ActionResult<HeadThicknessOptionsDto> GetOptions()
    {
        return Ok(_headThicknessService.GetOptions());
    }

    [HttpPost]
    public async Task<ActionResult<HeadThicknessResponseDto>> Calculate(
        [FromBody] HeadThicknessRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _headThicknessService.CalculateAsync(request, cancellationToken);
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

