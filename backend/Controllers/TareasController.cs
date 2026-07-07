using System;
using System.Collections.Generic;
using Backend.Contracts;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/tareas")]
public class TareasController : ControllerBase
{
    private readonly ITareasService _tareasService;

    public TareasController(ITareasService tareasService)
    {
        _tareasService = tareasService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TareaDto>> ObtenerTodas()
    {
        return Ok(_tareasService.ObtenerTodas());
    }

    [HttpGet("{id:int}")]
    public ActionResult<TareaDto> ObtenerPorId(int id)
    {
        var tarea = _tareasService.ObtenerPorId(id);
        return tarea is null ? NotFound() : Ok(tarea);
    }

    [HttpPost]
    public ActionResult<TareaDto> Crear([FromBody] CrearActualizarTareaRequest tarea)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaTarea = _tareasService.Crear(tarea);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TareaDto> Actualizar(int id, [FromBody] CrearActualizarTareaRequest tareaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var tarea = _tareasService.Actualizar(id, tareaActualizada);
        return tarea is null ? NotFound() : Ok(tarea);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        var eliminada = _tareasService.Eliminar(id);
        return eliminada ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/completar")]
    public ActionResult<TareaDto> Completar(int id)
    {
        var tarea = _tareasService.Completar(id);
        return tarea is null ? NotFound() : Ok(tarea);
    }

    [HttpPost("desde-plantilla/{plantillaId:int}")]
    public ActionResult<TareaDto> CrearDesdePlantilla(int plantillaId)
    {
        var nuevaTarea = _tareasService.CrearDesdePlantilla(plantillaId);
        if (nuevaTarea is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
    }
}