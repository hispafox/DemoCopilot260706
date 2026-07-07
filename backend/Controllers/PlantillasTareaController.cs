using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/plantillas")]
public class PlantillasTareaController : ControllerBase
{
    private readonly IPlantillasTareaService _plantillasTareaService;

    public PlantillasTareaController(IPlantillasTareaService plantillasTareaService)
    {
        _plantillasTareaService = plantillasTareaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantillaTareaDto>>> ObtenerTodas()
    {
        return Ok(await _plantillasTareaService.ObtenerTodasAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlantillaTareaDto>> ObtenerPorId(int id)
    {
        var plantilla = await _plantillasTareaService.ObtenerPorIdAsync(id);
        return plantilla is null ? NotFound() : Ok(plantilla);
    }

    [HttpPost]
    public async Task<ActionResult<PlantillaTareaDto>> Crear([FromBody] CrearActualizarPlantillaTareaRequest plantilla)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaPlantilla = await _plantillasTareaService.CrearAsync(plantilla);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaPlantilla.Id }, nuevaPlantilla);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PlantillaTareaDto>> Actualizar(int id, [FromBody] CrearActualizarPlantillaTareaRequest plantillaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var plantilla = await _plantillasTareaService.ActualizarAsync(id, plantillaActualizada);
        return plantilla is null ? NotFound() : Ok(plantilla);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminada = await _plantillasTareaService.EliminarAsync(id);
        return eliminada ? NoContent() : NotFound();
    }
}