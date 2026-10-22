using Microsoft.AspNetCore.Mvc;
using EngCalc.Application.Materials.DTOs;
using EngCalc.Application.Materials.Interfaces;

namespace EngCalc.API.Controllers;

[ApiController]
[Route("api/materials")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaterialOptionDto>>> GetMaterials(CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetMaterialsAsync(cancellationToken);
        return Ok(materials);
    }
}

