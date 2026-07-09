using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerTodas()
    {
        return Ok(await _tareasService.ObtenerTodasAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TareaDto>> ObtenerPorId(int id)
    {
        var tarea = await _tareasService.ObtenerPorIdAsync(id);
        return tarea is null ? NotFound() : Ok(tarea);
    }

    [HttpPost]
    public async Task<ActionResult<TareaDto>> Crear([FromBody] CrearActualizarTareaRequest tarea)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var nuevaTarea = await _tareasService.CrearAsync(tarea);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? nameof(tarea.TipoTareaId), ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TareaDto>> Actualizar(int id, [FromBody] CrearActualizarTareaRequest tareaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var tarea = await _tareasService.ActualizarAsync(id, tareaActualizada);
            return tarea is null ? NotFound() : Ok(tarea);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? nameof(tareaActualizada.TipoTareaId), ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminada = await _tareasService.EliminarAsync(id);
        return eliminada ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/completar")]
    public async Task<ActionResult<TareaDto>> Completar(int id)
    {
        var tarea = await _tareasService.CompletarAsync(id);
        return tarea is null ? NotFound() : Ok(tarea);
    }

    [HttpPost("desde-plantilla/{plantillaId:int}")]
    public async Task<ActionResult<TareaDto>> CrearDesdePlantilla(int plantillaId)
    {
        var nuevaTarea = await _tareasService.CrearDesdePlantillaAsync(plantillaId);
        if (nuevaTarea is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
    }
}