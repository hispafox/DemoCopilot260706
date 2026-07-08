using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class DepartamentosService : IDepartamentosService
{
    private readonly ApplicationDbContext _dbContext;

    public DepartamentosService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DepartamentoDto>> ObtenerTodosAsync()
    {
        return await _dbContext.Departamentos
            .AsNoTracking()
            .OrderBy(d => d.Nombre)
            .Select(d => new DepartamentoDto
            {
                Id = d.Id,
                Nombre = d.Nombre
            })
            .ToListAsync();
    }

    public async Task<DepartamentoDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Departamentos
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartamentoDto
            {
                Id = d.Id,
                Nombre = d.Nombre
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DepartamentoDto> CrearAsync(CrearActualizarDepartamentoRequest departamento)
    {
        var nuevoDepartamento = new Departamento
        {
            Nombre = departamento.Nombre
        };

        _dbContext.Departamentos.Add(nuevoDepartamento);
        await _dbContext.SaveChangesAsync();

        return new DepartamentoDto
        {
            Id = nuevoDepartamento.Id,
            Nombre = nuevoDepartamento.Nombre
        };
    }

    public async Task<DepartamentoDto?> ActualizarAsync(int id, CrearActualizarDepartamentoRequest departamentoActualizado)
    {
        var departamentoExistente = await _dbContext.Departamentos.FirstOrDefaultAsync(d => d.Id == id);
        if (departamentoExistente is null)
        {
            return null;
        }

        departamentoExistente.Nombre = departamentoActualizado.Nombre;
        await _dbContext.SaveChangesAsync();

        return new DepartamentoDto
        {
            Id = departamentoExistente.Id,
            Nombre = departamentoExistente.Nombre
        };
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var departamentoExistente = await _dbContext.Departamentos.FirstOrDefaultAsync(d => d.Id == id);
        if (departamentoExistente is null)
        {
            return false;
        }

        var tieneUsuarios = await _dbContext.Usuarios.AnyAsync(u => u.DepartamentoId == id);
        if (tieneUsuarios)
        {
            return false;
        }

        _dbContext.Departamentos.Remove(departamentoExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
