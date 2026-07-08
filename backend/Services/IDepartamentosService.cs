using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface IDepartamentosService
{
    Task<IReadOnlyList<DepartamentoDto>> ObtenerTodosAsync();

    Task<DepartamentoDto?> ObtenerPorIdAsync(int id);

    Task<DepartamentoDto> CrearAsync(CrearActualizarDepartamentoRequest departamento);

    Task<DepartamentoDto?> ActualizarAsync(int id, CrearActualizarDepartamentoRequest departamentoActualizado);

    Task<bool> EliminarAsync(int id);
}
