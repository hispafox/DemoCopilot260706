using System.ComponentModel;
using Backend.Contracts;
using Backend.Models;
using Backend.Services;
using ModelContextProtocol.Server;

namespace Backend.Mcp.Tools;

/// <summary>
/// Tools MCP sobre la entidad Tarea. Cada tool reutiliza <see cref="ITareasService"/>,
/// que se inyecta por parametro dentro de un scope de dependencias por invocacion,
/// de modo que el ApplicationDbContext es fresco y seguro en cada llamada.
/// </summary>
[McpServerToolType]
public static class TareasTools
{
    [McpServerTool(Name = "listar_tareas")]
    [Description("Devuelve todas las tareas ordenadas por fecha de creacion descendente.")]
    public static async Task<IReadOnlyList<TareaDto>> ListarTareas(ITareasService tareasService)
    {
        return await tareasService.ObtenerTodasAsync();
    }

    [McpServerTool(Name = "obtener_tarea")]
    [Description("Devuelve una tarea concreta por su Id, o null si no existe.")]
    public static async Task<TareaDto?> ObtenerTarea(
        ITareasService tareasService,
        [Description("Id de la tarea a consultar.")] int id)
    {
        return await tareasService.ObtenerPorIdAsync(id);
    }

    [McpServerTool(Name = "buscar_tareas")]
    [Description("Busca tareas cuyo titulo contenga el texto indicado (coincidencias parciales, sin distinguir mayusculas). Util para encontrar tareas parecidas por nombre.")]
    public static async Task<IReadOnlyList<TareaDto>> BuscarTareas(
        ITareasService tareasService,
        [Description("Texto a buscar dentro del titulo de las tareas.")] string texto)
    {
        return await tareasService.BuscarPorTituloAsync(texto);
    }

    [McpServerTool(Name = "crear_tarea")]
    [Description("Crea una tarea nueva. El titulo (1-200 caracteres) y el tipo de tarea son obligatorios.")]
    public static async Task<TareaDto> CrearTarea(
        ITareasService tareasService,
        [Description("Titulo de la tarea (1-200 caracteres).")] string titulo,
        [Description("Id del tipo de tarea. Es obligatorio y debe existir previamente.")] int tipoTareaId,
        [Description("Notas opcionales de la tarea.")] string? notas = null,
        [Description("Fecha de vencimiento opcional, en UTC (ISO 8601).")] DateTime? fechaVencimiento = null,
        [Description("Prioridad de la tarea: Baja, Normal, Alta o Urgente.")] PrioridadTarea prioridad = PrioridadTarea.Normal,
        [Description("Id del usuario asignado (opcional).")] int? usuarioId = null)
    {
        var request = new CrearActualizarTareaRequest
        {
            Titulo = titulo,
            TipoTareaId = tipoTareaId,
            Notas = notas,
            FechaVencimiento = fechaVencimiento,
            Prioridad = prioridad,
            UsuarioId = usuarioId
        };

        return await tareasService.CrearAsync(request);
    }

    [McpServerTool(Name = "actualizar_tarea")]
    [Description("Actualiza parcialmente una tarea existente (estilo PATCH): solo cambia los campos indicados; los omitidos conservan su valor actual. Devuelve la tarea actualizada o un mensaje si no existe.")]
    public static async Task<object> ActualizarTarea(
        ITareasService tareasService,
        [Description("Id de la tarea a actualizar.")] int id,
        [Description("Nuevo titulo (1-200 caracteres). Omitir para no cambiarlo.")] string? titulo = null,
        [Description("Nuevo estado de completada. Omitir para no cambiarlo.")] bool? estaCompletada = null,
        [Description("Nuevas notas. Omitir para no cambiarlas.")] string? notas = null,
        [Description("Nueva fecha de vencimiento en UTC (ISO 8601). Omitir para no cambiarla.")] DateTime? fechaVencimiento = null,
        [Description("Nueva prioridad: Baja, Normal, Alta o Urgente. Omitir para no cambiarla.")] PrioridadTarea? prioridad = null,
        [Description("Nuevo tipo de tarea (debe existir). Omitir para no cambiarlo.")] int? tipoTareaId = null,
        [Description("Nuevo usuario asignado. Omitir para no cambiarlo.")] int? usuarioId = null)
    {
        var actual = await tareasService.ObtenerPorIdAsync(id);
        if (actual is null)
        {
            return $"No existe ninguna tarea con Id {id}.";
        }

        var request = new CrearActualizarTareaRequest
        {
            Titulo = titulo ?? actual.Titulo,
            EstaCompletada = estaCompletada ?? actual.EstaCompletada,
            Notas = notas ?? actual.Notas,
            FechaVencimiento = fechaVencimiento ?? actual.FechaVencimiento,
            Prioridad = prioridad ?? actual.Prioridad,
            EsRepetitiva = actual.EsRepetitiva,
            TipoRecurrencia = actual.TipoRecurrencia,
            ProximaRecurrencia = actual.ProximaRecurrencia,
            PlantillaTareaId = actual.PlantillaTareaId,
            CategoriaId = actual.CategoriaId,
            UsuarioId = usuarioId ?? actual.UsuarioId,
            TipoTareaId = tipoTareaId ?? actual.TipoTareaId
        };

        var actualizada = await tareasService.ActualizarAsync(id, request);
        return actualizada is not null
            ? actualizada
            : $"No existe ninguna tarea con Id {id}.";
    }

    [McpServerTool(Name = "completar_tarea")]
    [Description("Marca una tarea como completada. Si es repetitiva, genera automaticamente la siguiente ocurrencia.")]
    public static async Task<TareaDto?> CompletarTarea(
        ITareasService tareasService,
        [Description("Id de la tarea a completar.")] int id)
    {
        return await tareasService.CompletarAsync(id);
    }

    [McpServerTool(Name = "eliminar_tarea")]
    [Description("Elimina una tarea por su Id. Devuelve un mensaje indicando el resultado.")]
    public static async Task<string> EliminarTarea(
        ITareasService tareasService,
        [Description("Id de la tarea a eliminar.")] int id)
    {
        var eliminada = await tareasService.EliminarAsync(id);
        return eliminada
            ? $"Tarea {id} eliminada correctamente."
            : $"No existe ninguna tarea con Id {id}.";
    }
}
