using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class TiposTareaService : ITiposTareaService
{
    private readonly ApplicationDbContext _dbContext;

    public TiposTareaService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TipoTareaDto>> ObtenerTodosAsync()
    {
        return await _dbContext.TiposTarea
            .AsNoTracking()
            .OrderBy(tipo => tipo.Nombre)
            .Select(tipo => new TipoTareaDto
            {
                Id = tipo.Id,
                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion,
                EstaActivo = tipo.EstaActivo
            })
            .ToListAsync();
    }

    public async Task<TipoTareaDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.TiposTarea
            .AsNoTracking()
            .Where(tipo => tipo.Id == id)
            .Select(tipo => new TipoTareaDto
            {
                Id = tipo.Id,
                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion,
                EstaActivo = tipo.EstaActivo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TipoTareaDto> CrearAsync(CrearActualizarTipoTareaRequest request)
    {
        var nuevoTipo = new TipoTarea
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            EstaActivo = request.EstaActivo
        };

        _dbContext.TiposTarea.Add(nuevoTipo);
        await _dbContext.SaveChangesAsync();
        return Mapear(nuevoTipo);
    }

    public async Task<TipoTareaDto?> ActualizarAsync(int id, CrearActualizarTipoTareaRequest request)
    {
        var tipoExistente = await _dbContext.TiposTarea.FirstOrDefaultAsync(tipo => tipo.Id == id);
        if (tipoExistente is null)
        {
            return null;
        }

        tipoExistente.Nombre = request.Nombre;
        tipoExistente.Descripcion = request.Descripcion;
        tipoExistente.EstaActivo = request.EstaActivo;

        await _dbContext.SaveChangesAsync();
        return Mapear(tipoExistente);
    }

    public async Task<ResultadoEliminacionTipoTarea> EliminarAsync(int id)
    {
        var tipoExistente = await _dbContext.TiposTarea.FirstOrDefaultAsync(tipo => tipo.Id == id);
        if (tipoExistente is null)
        {
            return ResultadoEliminacionTipoTarea.NoEncontrado;
        }

        var existeTareaAsociada = await _dbContext.Tareas
            .AsNoTracking()
            .AnyAsync(tarea => tarea.TipoTareaId == id);

        if (existeTareaAsociada)
        {
            return ResultadoEliminacionTipoTarea.TieneTareasAsociadas;
        }

        _dbContext.TiposTarea.Remove(tipoExistente);
        await _dbContext.SaveChangesAsync();
        return ResultadoEliminacionTipoTarea.Eliminado;
    }

    public Task<bool> ExisteAsync(int id)
    {
        return _dbContext.TiposTarea
            .AsNoTracking()
            .AnyAsync(tipo => tipo.Id == id);
    }

    private static TipoTareaDto Mapear(TipoTarea tipo)
    {
        return new TipoTareaDto
        {
            Id = tipo.Id,
            Nombre = tipo.Nombre,
            Descripcion = tipo.Descripcion,
            EstaActivo = tipo.EstaActivo
        };
    }
}
