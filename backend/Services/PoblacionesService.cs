using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class PoblacionesService : IPoblacionesService
{
    private readonly ApplicationDbContext _dbContext;

    public PoblacionesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PoblacionDto>> ObtenerTodosAsync()
    {
        return await _dbContext.Poblaciones
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new PoblacionDto
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .ToListAsync();
    }

    public async Task<PoblacionDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Poblaciones
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PoblacionDto
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PoblacionDto> CrearAsync(CrearActualizarPoblacionRequest poblacion)
    {
        var nuevaPoblacion = new Poblacion
        {
            Nombre = poblacion.Nombre
        };

        _dbContext.Poblaciones.Add(nuevaPoblacion);
        await _dbContext.SaveChangesAsync();

        return new PoblacionDto
        {
            Id = nuevaPoblacion.Id,
            Nombre = nuevaPoblacion.Nombre
        };
    }

    public async Task<PoblacionDto?> ActualizarAsync(int id, CrearActualizarPoblacionRequest poblacionActualizada)
    {
        var poblacionExistente = await _dbContext.Poblaciones.FirstOrDefaultAsync(p => p.Id == id);
        if (poblacionExistente is null)
        {
            return null;
        }

        poblacionExistente.Nombre = poblacionActualizada.Nombre;
        await _dbContext.SaveChangesAsync();

        return new PoblacionDto
        {
            Id = poblacionExistente.Id,
            Nombre = poblacionExistente.Nombre
        };
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var poblacionExistente = await _dbContext.Poblaciones.FirstOrDefaultAsync(p => p.Id == id);
        if (poblacionExistente is null)
        {
            return false;
        }

        var tieneUsuarios = await _dbContext.Usuarios.AnyAsync(u => u.PoblacionId == id);
        if (tieneUsuarios)
        {
            return false;
        }

        _dbContext.Poblaciones.Remove(poblacionExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
