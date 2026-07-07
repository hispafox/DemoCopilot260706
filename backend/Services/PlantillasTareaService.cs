using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class PlantillasTareaService : IPlantillasTareaService
{
    private readonly ApplicationDbContext _dbContext;

    public PlantillasTareaService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PlantillaTareaDto>> ObtenerTodasAsync()
    {
        return await _dbContext.PlantillasTarea
            .AsNoTracking()
            .Select(plantilla => new PlantillaTareaDto
            {
                Id = plantilla.Id,
                Titulo = plantilla.Titulo,
                Notas = plantilla.Notas,
                EsRepetitiva = plantilla.EsRepetitiva,
                TipoRecurrencia = plantilla.TipoRecurrencia,
                CategoriaId = plantilla.CategoriaId,
                EstaActiva = plantilla.EstaActiva
            })
            .ToListAsync();
    }

    public async Task<PlantillaTareaDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.PlantillasTarea
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(plantilla => new PlantillaTareaDto
            {
                Id = plantilla.Id,
                Titulo = plantilla.Titulo,
                Notas = plantilla.Notas,
                EsRepetitiva = plantilla.EsRepetitiva,
                TipoRecurrencia = plantilla.TipoRecurrencia,
                CategoriaId = plantilla.CategoriaId,
                EstaActiva = plantilla.EstaActiva
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PlantillaTareaDto> CrearAsync(CrearActualizarPlantillaTareaRequest plantilla)
    {
        var nuevaPlantilla = new PlantillaTarea
        {
            Titulo = plantilla.Titulo,
            Notas = plantilla.Notas,
            EsRepetitiva = plantilla.EsRepetitiva,
            TipoRecurrencia = plantilla.TipoRecurrencia,
            CategoriaId = plantilla.CategoriaId,
            EstaActiva = plantilla.EstaActiva
        };

        _dbContext.PlantillasTarea.Add(nuevaPlantilla);
        await _dbContext.SaveChangesAsync();
        return Mapear(nuevaPlantilla);
    }

    public async Task<PlantillaTareaDto?> ActualizarAsync(int id, CrearActualizarPlantillaTareaRequest plantillaActualizada)
    {
        var plantillaExistente = await _dbContext.PlantillasTarea.FirstOrDefaultAsync(item => item.Id == id);
        if (plantillaExistente is null)
        {
            return null;
        }

        plantillaExistente.Titulo = plantillaActualizada.Titulo;
        plantillaExistente.Notas = plantillaActualizada.Notas;
        plantillaExistente.EsRepetitiva = plantillaActualizada.EsRepetitiva;
        plantillaExistente.TipoRecurrencia = plantillaActualizada.TipoRecurrencia;
        plantillaExistente.CategoriaId = plantillaActualizada.CategoriaId;
        plantillaExistente.EstaActiva = plantillaActualizada.EstaActiva;

        await _dbContext.SaveChangesAsync();
        return Mapear(plantillaExistente);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var plantillaExistente = await _dbContext.PlantillasTarea.FirstOrDefaultAsync(item => item.Id == id);
        if (plantillaExistente is null)
        {
            return false;
        }

        var tareasAsociadas = await _dbContext.Tareas
            .Where(item => item.PlantillaTareaId == id)
            .ToListAsync();

        foreach (var tarea in tareasAsociadas)
        {
            tarea.PlantillaTareaId = null;
            tarea.PlantillaTarea = null;
        }

        _dbContext.PlantillasTarea.Remove(plantillaExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static PlantillaTareaDto Mapear(PlantillaTarea plantilla)
    {
        return new PlantillaTareaDto
        {
            Id = plantilla.Id,
            Titulo = plantilla.Titulo,
            Notas = plantilla.Notas,
            EsRepetitiva = plantilla.EsRepetitiva,
            TipoRecurrencia = plantilla.TipoRecurrencia,
            CategoriaId = plantilla.CategoriaId,
            EstaActiva = plantilla.EstaActiva
        };
    }
}
