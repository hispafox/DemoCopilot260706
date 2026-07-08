using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface ITiposTareaService
{
    Task<IReadOnlyList<TipoTareaDto>> ObtenerTodosAsync();

    Task<TipoTareaDto?> ObtenerPorIdAsync(int id);

    Task<TipoTareaDto> CrearAsync(CrearActualizarTipoTareaRequest request);

    Task<TipoTareaDto?> ActualizarAsync(int id, CrearActualizarTipoTareaRequest request);

    Task<ResultadoEliminacionTipoTarea> EliminarAsync(int id);

    Task<bool> ExisteAsync(int id);
}

public enum ResultadoEliminacionTipoTarea
{
    Eliminado,
    NoEncontrado,
    TieneTareasAsociadas
}
