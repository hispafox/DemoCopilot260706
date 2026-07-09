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

    public UsuariosController(IUsuariosService usuariosService)
    {
        _usuariosService = usuariosService;
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

        try
        {
            var nuevoUsuario = await _usuariosService.CrearAsync(usuario);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoUsuario.Id }, nuevoUsuario);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? nameof(usuario.DepartamentoId), ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(int id, [FromBody] CrearActualizarUsuarioRequest usuarioActualizado)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var usuario = await _usuariosService.ActualizarAsync(id, usuarioActualizado);
            return usuario is null ? NotFound() : Ok(usuario);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? nameof(usuarioActualizado.DepartamentoId), ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _usuariosService.EliminarAsync(id);
        return eliminado ? NoContent() : NotFound();
    }
}
