using System;
using System.Collections.Generic;
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
    public ActionResult<IEnumerable<PlantillaTareaDto>> ObtenerTodas()
    {
        return Ok(_plantillasTareaService.ObtenerTodas());
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlantillaTareaDto> ObtenerPorId(int id)
    {
        var plantilla = _plantillasTareaService.ObtenerPorId(id);
        return plantilla is null ? NotFound() : Ok(plantilla);
    }

    [HttpPost]
    public ActionResult<PlantillaTareaDto> Crear([FromBody] CrearActualizarPlantillaTareaRequest plantilla)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaPlantilla = _plantillasTareaService.Crear(plantilla);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaPlantilla.Id }, nuevaPlantilla);
    }

    [HttpPut("{id:int}")]
    public ActionResult<PlantillaTareaDto> Actualizar(int id, [FromBody] CrearActualizarPlantillaTareaRequest plantillaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var plantilla = _plantillasTareaService.Actualizar(id, plantillaActualizada);
        return plantilla is null ? NotFound() : Ok(plantilla);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        var eliminada = _plantillasTareaService.Eliminar(id);
        return eliminada ? NoContent() : NotFound();
    }
}