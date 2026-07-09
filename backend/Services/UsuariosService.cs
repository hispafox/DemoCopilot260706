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
            .Include(u => u.Poblacion)
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                DepartamentoId = u.DepartamentoId,
                DepartamentoNombre = u.Departamento.Nombre,
                SedeId = u.SedeId,
                SedeNombre = u.Sede.Nombre,
                PoblacionId = u.PoblacionId,
                PoblacionNombre = u.Poblacion.Nombre,
                PoblacionCodigoIsoPais = u.Poblacion.CodigoIsoPais
            })
            .ToListAsync();
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.Poblacion)
            .Where(u => u.Id == id)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                DepartamentoId = u.DepartamentoId,
                DepartamentoNombre = u.Departamento.Nombre,
                SedeId = u.SedeId,
                SedeNombre = u.Sede.Nombre,
                PoblacionId = u.PoblacionId,
                PoblacionNombre = u.Poblacion.Nombre,
                PoblacionCodigoIsoPais = u.Poblacion.CodigoIsoPais
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UsuarioDto> CrearAsync(CrearActualizarUsuarioRequest usuario)
    {
        await ValidarReferenciasAsync(usuario);

        var nuevoUsuario = new Usuario
        {
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            DepartamentoId = usuario.DepartamentoId,
            SedeId = usuario.SedeId,
            PoblacionId = usuario.PoblacionId
        };

        _dbContext.Usuarios.Add(nuevoUsuario);
        await _dbContext.SaveChangesAsync();

        return await ObtenerPorIdInternoAsync(nuevoUsuario.Id) ?? Mapear(nuevoUsuario);
    }

    public async Task<UsuarioDto?> ActualizarAsync(int id, CrearActualizarUsuarioRequest usuarioActualizado)
    {
        await ValidarReferenciasAsync(usuarioActualizado);

        var usuarioExistente = await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuarioExistente is null)
        {
            return null;
        }

        usuarioExistente.Nombre = usuarioActualizado.Nombre;
        usuarioExistente.Email = usuarioActualizado.Email;
        usuarioExistente.DepartamentoId = usuarioActualizado.DepartamentoId;
        usuarioExistente.SedeId = usuarioActualizado.SedeId;
        usuarioExistente.PoblacionId = usuarioActualizado.PoblacionId;

        await _dbContext.SaveChangesAsync();
        return await ObtenerPorIdInternoAsync(usuarioExistente.Id) ?? Mapear(usuarioExistente);
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

    private async Task ValidarReferenciasAsync(CrearActualizarUsuarioRequest usuario)
    {
        var departamentoExiste = await _dbContext.Departamentos.AnyAsync(item => item.Id == usuario.DepartamentoId);
        if (!departamentoExiste)
        {
            throw new ArgumentException("El departamento indicado no existe.", nameof(CrearActualizarUsuarioRequest.DepartamentoId));
        }

        var sedeExiste = await _dbContext.Sedes.AnyAsync(item => item.Id == usuario.SedeId);
        if (!sedeExiste)
        {
            throw new ArgumentException("La sede indicada no existe.", nameof(CrearActualizarUsuarioRequest.SedeId));
        }

        var poblacionExiste = await _dbContext.Poblaciones.AnyAsync(item => item.Id == usuario.PoblacionId);
        if (!poblacionExiste)
        {
            throw new ArgumentException("La poblacion indicada no existe.", nameof(CrearActualizarUsuarioRequest.PoblacionId));
        }
    }

    private static UsuarioDto Mapear(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            DepartamentoId = usuario.DepartamentoId,
            DepartamentoNombre = usuario.Departamento?.Nombre ?? string.Empty,
            SedeId = usuario.SedeId,
            SedeNombre = usuario.Sede?.Nombre ?? string.Empty,
            PoblacionId = usuario.PoblacionId,
            PoblacionNombre = usuario.Poblacion?.Nombre ?? string.Empty,
            PoblacionCodigoIsoPais = usuario.Poblacion?.CodigoIsoPais ?? string.Empty
        };
    }

    private Task<UsuarioDto?> ObtenerPorIdInternoAsync(int id)
    {
        return _dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.Poblacion)
            .Where(u => u.Id == id)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                DepartamentoId = u.DepartamentoId,
                DepartamentoNombre = u.Departamento.Nombre,
                SedeId = u.SedeId,
                SedeNombre = u.Sede.Nombre,
                PoblacionId = u.PoblacionId,
                PoblacionNombre = u.Poblacion.Nombre,
                PoblacionCodigoIsoPais = u.Poblacion.CodigoIsoPais
            })
            .FirstOrDefaultAsync();
    }
}
