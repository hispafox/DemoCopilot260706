using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/departamentos")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentosService _departamentosService;

    public DepartamentosController(IDepartamentosService departamentosService)
    {
        _departamentosService = departamentosService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartamentoDto>>> ObtenerTodos()
    {
        return Ok(await _departamentosService.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartamentoDto>> ObtenerPorId(int id)
    {
        var departamento = await _departamentosService.ObtenerPorIdAsync(id);
        return departamento is null ? NotFound() : Ok(departamento);
    }

    [HttpPost]
    public async Task<ActionResult<DepartamentoDto>> Crear([FromBody] CrearActualizarDepartamentoRequest departamento)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevoDepartamento = await _departamentosService.CrearAsync(departamento);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoDepartamento.Id }, nuevoDepartamento);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartamentoDto>> Actualizar(int id, [FromBody] CrearActualizarDepartamentoRequest departamentoActualizado)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var departamento = await _departamentosService.ActualizarAsync(id, departamentoActualizado);
        return departamento is null ? NotFound() : Ok(departamento);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var departamento = await _departamentosService.ObtenerPorIdAsync(id);
        if (departamento is null)
        {
            return NotFound();
        }

        var eliminado = await _departamentosService.EliminarAsync(id);
        if (!eliminado)
        {
            return Conflict(new { mensaje = "No se puede eliminar el departamento porque tiene usuarios asignados." });
        }

        return NoContent();
    }
}
