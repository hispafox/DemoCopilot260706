using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tareas")]
public class TareasController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Tarea>> ObtenerTodas()
    {
        lock (InMemoryStore.SyncRoot)
        {
            return Ok(InMemoryStore.Tareas
                .OrderByDescending(tarea => tarea.FechaCreacion)
                .ToList());
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Tarea> ObtenerPorId(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tarea = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            return tarea is null ? NotFound() : Ok(tarea);
        }
    }

    [HttpPost]
    public ActionResult<Tarea> Crear([FromBody] Tarea tarea)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaTarea = new Tarea
        {
            Id = InMemoryStore.ObtenerSiguienteTareaId(),
            Titulo = tarea.Titulo,
            EstaCompletada = tarea.EstaCompletada,
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = tarea.FechaVencimiento,
            Notas = tarea.Notas,
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
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Tarea> Actualizar(int id, [FromBody] Tarea tareaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            if (tareaExistente is null)
            {
                return NotFound();
            }

            tareaExistente.Titulo = tareaActualizada.Titulo;
            tareaExistente.EstaCompletada = tareaActualizada.EstaCompletada;
            tareaExistente.FechaVencimiento = tareaActualizada.FechaVencimiento;
            tareaExistente.Notas = tareaActualizada.Notas;
            tareaExistente.EsRepetitiva = tareaActualizada.EsRepetitiva;
            tareaExistente.TipoRecurrencia = tareaActualizada.TipoRecurrencia;
            tareaExistente.ProximaRecurrencia = tareaActualizada.ProximaRecurrencia;
            tareaExistente.PlantillaTareaId = tareaActualizada.PlantillaTareaId;
            tareaExistente.PlantillaTarea = tareaActualizada.PlantillaTareaId is int plantillaId
                ? InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == plantillaId)
                : null;
            tareaExistente.CategoriaId = tareaActualizada.CategoriaId;

            return Ok(tareaExistente);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            if (tareaExistente is null)
            {
                return NotFound();
            }

            InMemoryStore.Tareas.Remove(tareaExistente);
            return NoContent();
        }
    }

    [HttpPost("{id:int}/completar")]
    public ActionResult<Tarea> Completar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var tareaExistente = InMemoryStore.Tareas.FirstOrDefault(item => item.Id == id);
            if (tareaExistente is null)
            {
                return NotFound();
            }

            if (tareaExistente.EstaCompletada)
            {
                return Ok(tareaExistente);
            }

            tareaExistente.EstaCompletada = true;

            if (!tareaExistente.EsRepetitiva || tareaExistente.TipoRecurrencia is null)
            {
                return Ok(tareaExistente);
            }

            var siguiente = CrearSiguienteOcurrencia(tareaExistente);
            InMemoryStore.Tareas.Add(siguiente);

            return Ok(tareaExistente);
        }
    }

    [HttpPost("desde-plantilla/{plantillaId:int}")]
    public ActionResult<Tarea> CrearDesdePlantilla(int plantillaId)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantilla = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == plantillaId);
            if (plantilla is null)
            {
                return NotFound();
            }

            var nuevaTarea = new Tarea
            {
                Id = InMemoryStore.ObtenerSiguienteTareaId(),
                Titulo = plantilla.Titulo,
                EstaCompletada = false,
                FechaCreacion = DateTime.UtcNow,
                Notas = plantilla.Notas,
                EsRepetitiva = plantilla.EsRepetitiva,
                TipoRecurrencia = plantilla.TipoRecurrencia,
                PlantillaTareaId = plantilla.Id,
                PlantillaTarea = plantilla,
                CategoriaId = plantilla.CategoriaId,
                ProximaRecurrencia = ObtenerPrimeraRecurrencia(plantilla.TipoRecurrencia)
            };

            InMemoryStore.Tareas.Add(nuevaTarea);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaTarea.Id }, nuevaTarea);
        }
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