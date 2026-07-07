using System.Collections.Generic;
using Backend.Models;

namespace Backend.Services;

public static class InMemoryStore
{
    private static int _siguienteTareaId = 1;
    private static int _siguientePlantillaId = 1;

    public static object SyncRoot { get; } = new();

    public static List<Tarea> Tareas { get; } = new();

    public static List<PlantillaTarea> Plantillas { get; } = new();

    public static int ObtenerSiguienteTareaId()
    {
        return _siguienteTareaId++;
    }

    public static int ObtenerSiguientePlantillaId()
    {
        return _siguientePlantillaId++;
    }

    public static void Reiniciar()
    {
        lock (SyncRoot)
        {
            _siguienteTareaId = 1;
            _siguientePlantillaId = 1;
            Tareas.Clear();
            Plantillas.Clear();
        }
    }
}