using Microsoft.AspNetCore.Mvc;
using VesselCalc.Application.DTOs;
using VesselCalc.Application.Interfaces;

namespace VesselCalc.API.Controllers;

[ApiController]
[Route("api/materials")]
public class MaterialsController : ControllerBase
{
    private readonly IShellThicknessService _shellThicknessService;

    public MaterialsController(IShellThicknessService shellThicknessService)
    {
        _shellThicknessService = shellThicknessService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaterialOptionDto>>> GetMaterials(CancellationToken cancellationToken)
    {
        var materials = await _shellThicknessService.GetMaterialsAsync(cancellationToken);
        return Ok(materials);
    }
}

