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
    private readonly IUsuariosService _usuariosService;
    private readonly ITiposTareaService _tiposTareaService;

    public TareasController(ITareasService tareasService, IUsuariosService usuariosService, ITiposTareaService tiposTareaService)
    {
        _tareasService = tareasService;
        _usuariosService = usuariosService;
        _tiposTareaService = tiposTareaService;
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

        if (tarea.UsuarioId is int usuarioId)
        {
            var usuario = await _usuariosService.ObtenerPorIdAsync(usuarioId);
            if (usuario is null)
            {
                ModelState.AddModelError(nameof(tarea.UsuarioId), "El usuario indicado no existe.");
                return ValidationProblem(ModelState);
            }
        }

        var tipoTareaExiste = await _tiposTareaService.ExisteAsync(tarea.TipoTareaId);
        if (!tipoTareaExiste)
        {
            ModelState.AddModelError(nameof(tarea.TipoTareaId), "El tipo de tarea indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var nuevaTarea = await _tareasService.CrearAsync(tarea);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TareaDto>> Actualizar(int id, [FromBody] CrearActualizarTareaRequest tareaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (tareaActualizada.UsuarioId is int usuarioId)
        {
            var usuario = await _usuariosService.ObtenerPorIdAsync(usuarioId);
            if (usuario is null)
            {
                ModelState.AddModelError(nameof(tareaActualizada.UsuarioId), "El usuario indicado no existe.");
                return ValidationProblem(ModelState);
            }
        }

        var tipoTareaExiste = await _tiposTareaService.ExisteAsync(tareaActualizada.TipoTareaId);
        if (!tipoTareaExiste)
        {
            ModelState.AddModelError(nameof(tareaActualizada.TipoTareaId), "El tipo de tarea indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var tarea = await _tareasService.ActualizarAsync(id, tareaActualizada);
        return tarea is null ? NotFound() : Ok(tarea);
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