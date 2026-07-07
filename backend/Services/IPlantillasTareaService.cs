using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface IPlantillasTareaService
{
    Task<IReadOnlyList<PlantillaTareaDto>> ObtenerTodasAsync();

    Task<PlantillaTareaDto?> ObtenerPorIdAsync(int id);

    Task<PlantillaTareaDto> CrearAsync(CrearActualizarPlantillaTareaRequest plantilla);

    Task<PlantillaTareaDto?> ActualizarAsync(int id, CrearActualizarPlantillaTareaRequest plantillaActualizada);

    Task<bool> EliminarAsync(int id);
}
