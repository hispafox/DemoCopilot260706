using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface IPoblacionesService
{
    Task<IReadOnlyList<PoblacionDto>> ObtenerTodosAsync();

    Task<PoblacionDto?> ObtenerPorIdAsync(int id);

    Task<PoblacionDto> CrearAsync(CrearActualizarPoblacionRequest poblacion);

    Task<PoblacionDto?> ActualizarAsync(int id, CrearActualizarPoblacionRequest poblacionActualizada);

    Task<bool> EliminarAsync(int id);
}
