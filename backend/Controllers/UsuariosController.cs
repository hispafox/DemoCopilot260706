using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuariosService _usuariosService;
    private readonly IDepartamentosService _departamentosService;

    public UsuariosController(IUsuariosService usuariosService, IDepartamentosService departamentosService)
    {
        _usuariosService = usuariosService;
        _departamentosService = departamentosService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerTodos()
    {
        return Ok(await _usuariosService.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
    {
        var usuario = await _usuariosService.ObtenerPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CrearActualizarUsuarioRequest usuario)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var departamento = await _departamentosService.ObtenerPorIdAsync(usuario.DepartamentoId);
        if (departamento is null)
        {
            ModelState.AddModelError(nameof(usuario.DepartamentoId), "El departamento indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var nuevoUsuario = await _usuariosService.CrearAsync(usuario);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoUsuario.Id }, nuevoUsuario);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(int id, [FromBody] CrearActualizarUsuarioRequest usuarioActualizado)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var departamento = await _departamentosService.ObtenerPorIdAsync(usuarioActualizado.DepartamentoId);
        if (departamento is null)
        {
            ModelState.AddModelError(nameof(usuarioActualizado.DepartamentoId), "El departamento indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var usuario = await _usuariosService.ActualizarAsync(id, usuarioActualizado);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _usuariosService.EliminarAsync(id);
        return eliminado ? NoContent() : NotFound();
    }
}
