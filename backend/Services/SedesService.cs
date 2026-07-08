using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class SedesService : ISedesService
{
    private readonly ApplicationDbContext _dbContext;

    public SedesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SedeDto>> ObtenerTodosAsync()
    {
        return await _dbContext.Sedes
            .AsNoTracking()
            .OrderBy(s => s.Nombre)
            .Select(s => new SedeDto
            {
                Id = s.Id,
                Nombre = s.Nombre
            })
            .ToListAsync();
    }

    public async Task<SedeDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Sedes
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SedeDto
            {
                Id = s.Id,
                Nombre = s.Nombre
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SedeDto> CrearAsync(CrearActualizarSedeRequest sede)
    {
        var nuevaSede = new Sede
        {
            Nombre = sede.Nombre
        };

        _dbContext.Sedes.Add(nuevaSede);
        await _dbContext.SaveChangesAsync();

        return new SedeDto
        {
            Id = nuevaSede.Id,
            Nombre = nuevaSede.Nombre
        };
    }

    public async Task<SedeDto?> ActualizarAsync(int id, CrearActualizarSedeRequest sedeActualizada)
    {
        var sedeExistente = await _dbContext.Sedes.FirstOrDefaultAsync(s => s.Id == id);
        if (sedeExistente is null)
        {
            return null;
        }

        sedeExistente.Nombre = sedeActualizada.Nombre;
        await _dbContext.SaveChangesAsync();

        return new SedeDto
        {
            Id = sedeExistente.Id,
            Nombre = sedeExistente.Nombre
        };
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var sedeExistente = await _dbContext.Sedes.FirstOrDefaultAsync(s => s.Id == id);
        if (sedeExistente is null)
        {
            return false;
        }

        var tieneUsuarios = await _dbContext.Usuarios.AnyAsync(u => u.SedeId == id);
        if (tieneUsuarios)
        {
            return false;
        }

        _dbContext.Sedes.Remove(sedeExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
