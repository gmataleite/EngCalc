using Microsoft.AspNetCore.Mvc;
using EngCalc.Application.StorageTanks.DTOs;
using EngCalc.Application.StorageTanks.Interfaces;

namespace EngCalc.API.Controllers.StorageTanks;

[ApiController]
[Route("api/calculations/storage-tanks/")]
public class StorageTankController : ControllerBase
{
    private readonly ITankVolumeService _storageTankVolumeService;

    public StorageTankController(ITankVolumeService tankVolumeService)
    {
        _storageTankVolumeService = tankVolumeService;
    }

    [HttpGet("options")]
    public ActionResult<TankOptionsDto> GetOptions()
    {
        return Ok(_storageTankVolumeService.GetOptions());
    }

    [HttpPost]
    public async Task<ActionResult<TankVolumeResponseDto>> Calculate(
        [FromBody] TankVolumeRequestDto request)
    {
        try
        {
            var result = await _storageTankVolumeService.CalculateVolumeAsync(request);
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

