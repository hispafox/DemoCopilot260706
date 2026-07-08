using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/poblaciones")]
public class PoblacionesController : ControllerBase
{
    private readonly IPoblacionesService _poblacionesService;

    public PoblacionesController(IPoblacionesService poblacionesService)
    {
        _poblacionesService = poblacionesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PoblacionDto>>> ObtenerTodos()
    {
        return Ok(await _poblacionesService.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PoblacionDto>> ObtenerPorId(int id)
    {
        var poblacion = await _poblacionesService.ObtenerPorIdAsync(id);
        return poblacion is null ? NotFound() : Ok(poblacion);
    }

    [HttpPost]
    public async Task<ActionResult<PoblacionDto>> Crear([FromBody] CrearActualizarPoblacionRequest poblacion)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaPoblacion = await _poblacionesService.CrearAsync(poblacion);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaPoblacion.Id }, nuevaPoblacion);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PoblacionDto>> Actualizar(int id, [FromBody] CrearActualizarPoblacionRequest poblacionActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var poblacion = await _poblacionesService.ActualizarAsync(id, poblacionActualizada);
        return poblacion is null ? NotFound() : Ok(poblacion);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var poblacion = await _poblacionesService.ObtenerPorIdAsync(id);
        if (poblacion is null)
        {
            return NotFound();
        }

        var eliminada = await _poblacionesService.EliminarAsync(id);
        if (!eliminada)
        {
            return Conflict(new { mensaje = "No se puede eliminar la poblacion porque tiene usuarios asignados." });
        }

        return NoContent();
    }
}
