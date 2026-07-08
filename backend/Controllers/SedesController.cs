using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/sedes")]
public class SedesController : ControllerBase
{
    private readonly ISedesService _sedesService;

    public SedesController(ISedesService sedesService)
    {
        _sedesService = sedesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SedeDto>>> ObtenerTodos()
    {
        return Ok(await _sedesService.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SedeDto>> ObtenerPorId(int id)
    {
        var sede = await _sedesService.ObtenerPorIdAsync(id);
        return sede is null ? NotFound() : Ok(sede);
    }

    [HttpPost]
    public async Task<ActionResult<SedeDto>> Crear([FromBody] CrearActualizarSedeRequest sede)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaSede = await _sedesService.CrearAsync(sede);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaSede.Id }, nuevaSede);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SedeDto>> Actualizar(int id, [FromBody] CrearActualizarSedeRequest sedeActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var sede = await _sedesService.ActualizarAsync(id, sedeActualizada);
        return sede is null ? NotFound() : Ok(sede);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var sede = await _sedesService.ObtenerPorIdAsync(id);
        if (sede is null)
        {
            return NotFound();
        }

        var eliminada = await _sedesService.EliminarAsync(id);
        if (!eliminada)
        {
            return Conflict(new { mensaje = "No se puede eliminar la sede porque tiene usuarios asignados." });
        }

        return NoContent();
    }
}
