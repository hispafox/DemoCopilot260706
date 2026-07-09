using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface ITareasService
{
    Task<IReadOnlyList<TareaDto>> ObtenerTodasAsync();

    Task<TareaDto?> ObtenerPorIdAsync(int id);

    Task<IReadOnlyList<TareaDto>> BuscarPorTituloAsync(string texto);

    Task<TareaDto> CrearAsync(CrearActualizarTareaRequest tarea);

    Task<TareaDto?> ActualizarAsync(int id, CrearActualizarTareaRequest tareaActualizada);

    Task<bool> EliminarAsync(int id);

    Task<TareaDto?> CompletarAsync(int id);

    Task<TareaDto?> CrearDesdePlantillaAsync(int plantillaId);
}
