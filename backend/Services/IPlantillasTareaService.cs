using System.Collections.Generic;
using Backend.Contracts;

namespace Backend.Services;

public interface IPlantillasTareaService
{
    IReadOnlyList<PlantillaTareaDto> ObtenerTodas();

    PlantillaTareaDto? ObtenerPorId(int id);

    PlantillaTareaDto Crear(CrearActualizarPlantillaTareaRequest plantilla);

    PlantillaTareaDto? Actualizar(int id, CrearActualizarPlantillaTareaRequest plantillaActualizada);

    bool Eliminar(int id);
}
