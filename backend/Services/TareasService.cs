using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Contracts;
using Backend.Models;

namespace Backend.Services;

public class TareasService : ITareasService
{
    public IReadOnlyList<TareaDto> ObtenerTodas()
    {
        lock (InMemoryStore.SyncRoot)
        {
            return InMemoryStore.Tareas
                .OrderByDescending(tarea => tarea.FechaCreacion)
                .Select(Mapear)
                .ToList();
        }
    }

    public TareaDto? ObtenerPorId(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tarea = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            return tarea is null ? null : Mapear(tarea);
        }
    }

    public TareaDto Crear(CrearActualizarTareaRequest tarea)
    {
        var nuevaTarea = new Tarea
        {
            Id = InMemoryStore.ObtenerSiguienteTareaId(),
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
            CategoriaId = tarea.CategoriaId
        };

        lock (InMemoryStore.SyncRoot)
        {
            if (nuevaTarea.PlantillaTareaId is int plantillaId)
            {
                nuevaTarea.PlantillaTarea = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == plantillaId);
            }

            InMemoryStore.Tareas.Add(nuevaTarea);
            return Mapear(nuevaTarea);
        }
    }

    public TareaDto? Actualizar(int id, CrearActualizarTareaRequest tareaActualizada)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
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
            tareaExistente.PlantillaTarea = tareaActualizada.PlantillaTareaId is int plantillaId
                ? InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == plantillaId)
                : null;
            tareaExistente.CategoriaId = tareaActualizada.CategoriaId;

            return Mapear(tareaExistente);
        }
    }

    public bool Eliminar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            if (tareaExistente is null)
            {
                return false;
            }

            InMemoryStore.Tareas.Remove(tareaExistente);
            return true;
        }
    }

    public TareaDto? Completar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            if (tareaExistente is null)
            {
                return null;
            }

            if (tareaExistente.EstaCompletada)
            {
                return Mapear(tareaExistente);
            }

            tareaExistente.EstaCompletada = true;

            if (!tareaExistente.EsRepetitiva || tareaExistente.TipoRecurrencia is null)
            {
                return Mapear(tareaExistente);
            }

            var siguiente = CrearSiguienteOcurrencia(tareaExistente);
            InMemoryStore.Tareas.Add(siguiente);

            return Mapear(tareaExistente);
        }
    }

    public TareaDto? CrearDesdePlantilla(int plantillaId)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantilla = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == plantillaId);
            if (plantilla is null)
            {
                return null;
            }

            var nuevaTarea = new Tarea
            {
                Id = InMemoryStore.ObtenerSiguienteTareaId(),
                Titulo = plantilla.Titulo,
                EstaCompletada = false,
                FechaCreacion = DateTime.UtcNow,
                Notas = plantilla.Notas,
                Prioridad = PrioridadTarea.Normal,
                EsRepetitiva = plantilla.EsRepetitiva,
                TipoRecurrencia = plantilla.TipoRecurrencia,
                PlantillaTareaId = plantilla.Id,
                PlantillaTarea = plantilla,
                CategoriaId = plantilla.CategoriaId,
                ProximaRecurrencia = ObtenerPrimeraRecurrencia(plantilla.TipoRecurrencia)
            };

            InMemoryStore.Tareas.Add(nuevaTarea);
            return Mapear(nuevaTarea);
        }
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
            CategoriaId = tarea.CategoriaId
        };
    }

    private static Tarea CrearSiguienteOcurrencia(Tarea tareaOrigen)
    {
        return new Tarea
        {
            Id = InMemoryStore.ObtenerSiguienteTareaId(),
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
            Categoria = tareaOrigen.Categoria
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
