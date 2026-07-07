using System.Collections.Generic;
using System.Linq;
using Backend.Contracts;
using Backend.Models;

namespace Backend.Services;

public class PlantillasTareaService : IPlantillasTareaService
{
    public IReadOnlyList<PlantillaTareaDto> ObtenerTodas()
    {
        lock (InMemoryStore.SyncRoot)
        {
            return InMemoryStore.Plantillas.Select(Mapear).ToList();
        }
    }

    public PlantillaTareaDto? ObtenerPorId(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantilla = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
            return plantilla is null ? null : Mapear(plantilla);
        }
    }

    public PlantillaTareaDto Crear(CrearActualizarPlantillaTareaRequest plantilla)
    {
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
            return Mapear(nuevaPlantilla);
        }
    }

    public PlantillaTareaDto? Actualizar(int id, CrearActualizarPlantillaTareaRequest plantillaActualizada)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantillaExistente = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
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

            return Mapear(plantillaExistente);
        }
    }

    public bool Eliminar(int id)
    {
        lock (InMemoryStore.SyncRoot)
        {
            var plantillaExistente = InMemoryStore.Plantillas.FirstOrDefault(item => item.Id == id);
            if (plantillaExistente is null)
            {
                return false;
            }

            InMemoryStore.Plantillas.Remove(plantillaExistente);

            foreach (var tarea in InMemoryStore.Tareas.Where(item => item.PlantillaTareaId == id))
            {
                tarea.PlantillaTareaId = null;
                tarea.PlantillaTarea = null;
            }

            return true;
        }
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
