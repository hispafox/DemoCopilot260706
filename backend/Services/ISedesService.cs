using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface ISedesService
{
    Task<IReadOnlyList<SedeDto>> ObtenerTodosAsync();

    Task<SedeDto?> ObtenerPorIdAsync(int id);

    Task<SedeDto> CrearAsync(CrearActualizarSedeRequest sede);

    Task<SedeDto?> ActualizarAsync(int id, CrearActualizarSedeRequest sedeActualizada);

    Task<bool> EliminarAsync(int id);
}
