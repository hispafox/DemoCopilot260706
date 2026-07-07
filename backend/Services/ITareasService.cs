using System.Collections.Generic;
using Backend.Contracts;

namespace Backend.Services;

public interface ITareasService
{
    IReadOnlyList<TareaDto> ObtenerTodas();

    TareaDto? ObtenerPorId(int id);

    TareaDto Crear(CrearActualizarTareaRequest tarea);

    TareaDto? Actualizar(int id, CrearActualizarTareaRequest tareaActualizada);

    bool Eliminar(int id);

    TareaDto? Completar(int id);

    TareaDto? CrearDesdePlantilla(int plantillaId);
}
