using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tipos-tarea")]
public class TiposTareaController : ControllerBase
{
    private readonly ITiposTareaService _tiposTareaService;

    public TiposTareaController(ITiposTareaService tiposTareaService)
    {
        _tiposTareaService = tiposTareaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoTareaDto>>> ObtenerTodos()
    {
        return Ok(await _tiposTareaService.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoTareaDto>> ObtenerPorId(int id)
    {
        var tipo = await _tiposTareaService.ObtenerPorIdAsync(id);
        return tipo is null ? NotFound() : Ok(tipo);
    }

    [HttpPost]
    public async Task<ActionResult<TipoTareaDto>> Crear([FromBody] CrearActualizarTipoTareaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevoTipo = await _tiposTareaService.CrearAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoTipo.Id }, nuevoTipo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TipoTareaDto>> Actualizar(int id, [FromBody] CrearActualizarTipoTareaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var tipoActualizado = await _tiposTareaService.ActualizarAsync(id, request);
        return tipoActualizado is null ? NotFound() : Ok(tipoActualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _tiposTareaService.EliminarAsync(id);
        if (resultado == ResultadoEliminacionTipoTarea.NoEncontrado)
        {
            return NotFound();
        }

        if (resultado == ResultadoEliminacionTipoTarea.TieneTareasAsociadas)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar el tipo porque existen tareas asociadas."
            });
        }

        return NoContent();
    }
}
