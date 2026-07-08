using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class TareasService : ITareasService
{
    private readonly ApplicationDbContext _dbContext;

    public TareasService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TareaDto>> ObtenerTodasAsync()
    {
        return await _dbContext.Tareas
            .AsNoTracking()
            .OrderByDescending(tarea => tarea.FechaCreacion)
            .Select(tarea => new TareaDto
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                EstaCompletada = tarea.EstaCompletada,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento,
                Notas = tarea.Notas,
                Prioridad = tarea.Prioridad,
                EsRepetitiva = tarea.EsRepetitiva,
                TipoRecurrencia = tarea.TipoRecurrencia,
                ProximaRecurrencia = tarea.ProximaRecurrencia,
                PlantillaTareaId = tarea.PlantillaTareaId,
                CategoriaId = tarea.CategoriaId,
                UsuarioId = tarea.UsuarioId,
                UsuarioNombre = tarea.Usuario != null ? tarea.Usuario.Nombre : null
            })
            .ToListAsync();
    }

    public async Task<TareaDto?> ObtenerPorIdAsync(int id)
    {
        return await _dbContext.Tareas
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(tarea => new TareaDto
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                EstaCompletada = tarea.EstaCompletada,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento,
                Notas = tarea.Notas,
                Prioridad = tarea.Prioridad,
                EsRepetitiva = tarea.EsRepetitiva,
                TipoRecurrencia = tarea.TipoRecurrencia,
                ProximaRecurrencia = tarea.ProximaRecurrencia,
                PlantillaTareaId = tarea.PlantillaTareaId,
                CategoriaId = tarea.CategoriaId,
                UsuarioId = tarea.UsuarioId,
                UsuarioNombre = tarea.Usuario != null ? tarea.Usuario.Nombre : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TareaDto> CrearAsync(CrearActualizarTareaRequest tarea)
    {
        var nuevaTarea = new Tarea
        {
            Titulo = tarea.Titulo,
            EstaCompletada = tarea.EstaCompletada,
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = tarea.FechaVencimiento,
            Notas = tarea.Notas,
            Prioridad = tarea.Prioridad,
            EsRepetitiva = tarea.EsRepetitiva,
            TipoRecurrencia = tarea.TipoRecurrencia,
            ProximaRecurrencia = tarea.ProximaRecurrencia,
            PlantillaTareaId = tarea.PlantillaTareaId,
            CategoriaId = tarea.CategoriaId,
            UsuarioId = tarea.UsuarioId
        };

        if (nuevaTarea.PlantillaTareaId is int plantillaId)
        {
            nuevaTarea.PlantillaTarea = await _dbContext.PlantillasTarea
                .FirstOrDefaultAsync(item => item.Id == plantillaId);
        }

        _dbContext.Tareas.Add(nuevaTarea);
        await _dbContext.SaveChangesAsync();
        return await ObtenerPorIdAsync(nuevaTarea.Id) ?? Mapear(nuevaTarea);
    }

    public async Task<TareaDto?> ActualizarAsync(int id, CrearActualizarTareaRequest tareaActualizada)
    {
        var tareaExistente = await _dbContext.Tareas.FirstOrDefaultAsync(item => item.Id == id);
        if (tareaExistente is null)
        {
            return null;
        }

        tareaExistente.Titulo = tareaActualizada.Titulo;
        tareaExistente.EstaCompletada = tareaActualizada.EstaCompletada;
        tareaExistente.FechaVencimiento = tareaActualizada.FechaVencimiento;
        tareaExistente.Notas = tareaActualizada.Notas;
        tareaExistente.Prioridad = tareaActualizada.Prioridad;
        tareaExistente.EsRepetitiva = tareaActualizada.EsRepetitiva;
        tareaExistente.TipoRecurrencia = tareaActualizada.TipoRecurrencia;
        tareaExistente.ProximaRecurrencia = tareaActualizada.ProximaRecurrencia;
        tareaExistente.PlantillaTareaId = tareaActualizada.PlantillaTareaId;
        tareaExistente.CategoriaId = tareaActualizada.CategoriaId;
        tareaExistente.UsuarioId = tareaActualizada.UsuarioId;

        await _dbContext.SaveChangesAsync();
        return await ObtenerPorIdAsync(tareaExistente.Id) ?? Mapear(tareaExistente);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var tareaExistente = await _dbContext.Tareas.FirstOrDefaultAsync(item => item.Id == id);
        if (tareaExistente is null)
        {
            return false;
        }

        _dbContext.Tareas.Remove(tareaExistente);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<TareaDto?> CompletarAsync(int id)
    {
        var tareaExistente = await _dbContext.Tareas.FirstOrDefaultAsync(item => item.Id == id);
        if (tareaExistente is null)
        {
            return null;
        }

        if (tareaExistente.EstaCompletada)
        {
            return Mapear(tareaExistente);
        }

        tareaExistente.EstaCompletada = true;

        if (tareaExistente.EsRepetitiva && tareaExistente.TipoRecurrencia is not null)
        {
            var siguiente = CrearSiguienteOcurrencia(tareaExistente);
            _dbContext.Tareas.Add(siguiente);
        }

        await _dbContext.SaveChangesAsync();
        return await ObtenerPorIdAsync(tareaExistente.Id) ?? Mapear(tareaExistente);
    }

    public async Task<TareaDto?> CrearDesdePlantillaAsync(int plantillaId)
    {
        var plantilla = await _dbContext.PlantillasTarea.FirstOrDefaultAsync(item => item.Id == plantillaId);
        if (plantilla is null)
        {
            return null;
        }

        var nuevaTarea = new Tarea
        {
            Titulo = plantilla.Titulo,
            EstaCompletada = false,
            FechaCreacion = DateTime.UtcNow,
            Notas = plantilla.Notas,
            Prioridad = PrioridadTarea.Normal,
            EsRepetitiva = plantilla.EsRepetitiva,
            TipoRecurrencia = plantilla.TipoRecurrencia,
            PlantillaTareaId = plantilla.Id,
            CategoriaId = plantilla.CategoriaId,
            ProximaRecurrencia = plantilla.EsRepetitiva ? ObtenerPrimeraRecurrencia(plantilla.TipoRecurrencia) : null
        };

        _dbContext.Tareas.Add(nuevaTarea);
        await _dbContext.SaveChangesAsync();
        return await ObtenerPorIdAsync(nuevaTarea.Id) ?? Mapear(nuevaTarea);
    }

    private static TareaDto Mapear(Tarea tarea)
    {
        return new TareaDto
        {
            Id = tarea.Id,
            Titulo = tarea.Titulo,
            EstaCompletada = tarea.EstaCompletada,
            FechaCreacion = tarea.FechaCreacion,
            FechaVencimiento = tarea.FechaVencimiento,
            Notas = tarea.Notas,
            Prioridad = tarea.Prioridad,
            EsRepetitiva = tarea.EsRepetitiva,
            TipoRecurrencia = tarea.TipoRecurrencia,
            ProximaRecurrencia = tarea.ProximaRecurrencia,
            PlantillaTareaId = tarea.PlantillaTareaId,
            CategoriaId = tarea.CategoriaId,
            UsuarioId = tarea.UsuarioId,
            UsuarioNombre = tarea.Usuario?.Nombre
        };
    }

    private static Tarea CrearSiguienteOcurrencia(Tarea tareaOrigen)
    {
        return new Tarea
        {
            Titulo = tareaOrigen.Titulo,
            EstaCompletada = false,
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = tareaOrigen.FechaVencimiento,
            Notas = tareaOrigen.Notas,
            Prioridad = tareaOrigen.Prioridad,
            EsRepetitiva = tareaOrigen.EsRepetitiva,
            TipoRecurrencia = tareaOrigen.TipoRecurrencia,
            ProximaRecurrencia = CalcularSiguienteFecha(tareaOrigen),
            PlantillaTareaId = tareaOrigen.PlantillaTareaId,
            PlantillaTarea = tareaOrigen.PlantillaTarea,
            CategoriaId = tareaOrigen.CategoriaId,
            Categoria = tareaOrigen.Categoria,
            UsuarioId = tareaOrigen.UsuarioId,
            Usuario = tareaOrigen.Usuario
        };
    }

    private static DateTime? ObtenerPrimeraRecurrencia(TipoRecurrencia? tipoRecurrencia)
    {
        if (tipoRecurrencia is null)
        {
            return null;
        }

        return SumarRecurrencia(DateTime.UtcNow, tipoRecurrencia.Value);
    }

    private static DateTime CalcularSiguienteFecha(Tarea tarea)
    {
        var baseRecurrencia = tarea.ProximaRecurrencia ?? tarea.FechaVencimiento ?? DateTime.UtcNow;
        return SumarRecurrencia(baseRecurrencia, tarea.TipoRecurrencia!.Value);
    }

    private static DateTime SumarRecurrencia(DateTime fechaBase, TipoRecurrencia tipoRecurrencia)
    {
        return tipoRecurrencia switch
        {
            TipoRecurrencia.Diaria => fechaBase.AddDays(1),
            TipoRecurrencia.Semanal => fechaBase.AddDays(7),
            TipoRecurrencia.Mensual => fechaBase.AddMonths(1),
            _ => fechaBase
        };
    }
}
