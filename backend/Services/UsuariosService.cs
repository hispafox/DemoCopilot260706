using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class UsuariosService : IUsuariosService
{
    private readonly ApplicationDbContext _dbContext;

    public UsuariosService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync()
    {
        return await _dbContext.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                DepartamentoId = u.DepartamentoId,
                DepartamentoNombre = u.Departamento.Nombre
            })
            .ToListAsync();
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                DepartamentoId = u.DepartamentoId,
                DepartamentoNombre = u.Departamento.Nombre
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UsuarioDto> CrearAsync(CrearActualizarUsuarioRequest usuario)
    {
        var nuevoUsuario = new Usuario
        {
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            DepartamentoId = usuario.DepartamentoId
        };

        _dbContext.Usuarios.Add(nuevoUsuario);
        await _dbContext.SaveChangesAsync();

        return Mapear(nuevoUsuario);
    }

    public async Task<UsuarioDto?> ActualizarAsync(int id, CrearActualizarUsuarioRequest usuarioActualizado)
    {
        var usuarioExistente = await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuarioExistente is null)
        {
            return null;
        }

        usuarioExistente.Nombre = usuarioActualizado.Nombre;
        usuarioExistente.Email = usuarioActualizado.Email;
        usuarioExistente.DepartamentoId = usuarioActualizado.DepartamentoId;

        await _dbContext.SaveChangesAsync();
        return Mapear(usuarioExistente);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var usuarioExistente = await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuarioExistente is null)
        {
            return false;
        }

        _dbContext.Usuarios.Remove(usuarioExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static UsuarioDto Mapear(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            DepartamentoId = usuario.DepartamentoId,
            DepartamentoNombre = usuario.Departamento?.Nombre ?? string.Empty
        };
    }
}
