using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/plantillas")]
public class PlantillasTareaController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlantillaTarea>> ObtenerTodas()
    {
        lock (InMemoryStore.SyncRoot)
        {
            return Ok(InMemoryStore.Plantillas.ToList());
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlantillaTarea> ObtenerPorId(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantilla = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
            return plantilla is null ? NotFound() : Ok(plantilla);
        }
    }

    [HttpPost]
    public ActionResult<PlantillaTarea> Crear([FromBody] PlantillaTarea plantilla)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var nuevaPlantilla = new PlantillaTarea
        {
            Id = InMemoryStore.ObtenerSiguientePlantillaId(),
            Titulo = plantilla.Titulo,
            Notas = plantilla.Notas,
            EsRepetitiva = plantilla.EsRepetitiva,
            TipoRecurrencia = plantilla.TipoRecurrencia,
            CategoriaId = plantilla.CategoriaId,
            EstaActiva = plantilla.EstaActiva
        };

        lock (InMemoryStore.SyncRoot)
        {
            InMemoryStore.Plantillas.Add(nuevaPlantilla);
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaPlantilla.Id }, nuevaPlantilla);
    }

    [HttpPut("{id:int}")]
    public ActionResult<PlantillaTarea> Actualizar(int id, [FromBody] PlantillaTarea plantillaActualizada)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        lock (InMemoryStore.SyncRoot)
        {
            var plantillaExistente = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
            if (plantillaExistente is null)
            {
                return NotFound();
            }

            plantillaExistente.Titulo = plantillaActualizada.Titulo;
            plantillaExistente.Notas = plantillaActualizada.Notas;
            plantillaExistente.EsRepetitiva = plantillaActualizada.EsRepetitiva;
            plantillaExistente.TipoRecurrencia = plantillaActualizada.TipoRecurrencia;
            plantillaExistente.CategoriaId = plantillaActualizada.CategoriaId;
            plantillaExistente.EstaActiva = plantillaActualizada.EstaActiva;

            return Ok(plantillaExistente);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantillaExistente = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
            if (plantillaExistente is null)
            {
                return NotFound();
            }

            InMemoryStore.Plantillas.Remove(plantillaExistente);

            foreach (var tarea in InMemoryStore.Tareas.Where(item => item.PlantillaTareaId == id))
            {
                tarea.PlantillaTareaId = null;
                tarea.PlantillaTarea = null;
            }

            return NoContent();
        }
    }
}