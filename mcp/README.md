# Backend.Mcp — Servidor MCP de Tareas

Servidor [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) que expone la
lógica de tareas del backend a clientes compatibles con MCP (VS Code Copilot, Claude
Desktop, etc.). Reutiliza los servicios existentes del proyecto `Backend.Api`, por lo que
**no duplica lógica de negocio**: es solo una capa de transporte sobre `ITareasService`.

Comparte la misma base de datos SQLite (`backend/backend.db`) que la API y el frontend
React, así que todo lo que crees, completes o borres por MCP se refleja en la aplicación.

## Arquitectura

```text
Cliente MCP (Copilot / Claude)
        │  (stdio, JSON-RPC)
        ▼
Backend.Mcp  ──►  TareasTools  ──►  ITareasService  ──►  ApplicationDbContext ──► backend.db
```

- **Transporte:** `stdio` (local, mismo equipo).
- **Reutilización:** referencia a `Backend.Api.csproj`; inyecta `ITareasService` por
  parámetro dentro de un scope de dependencias por invocación (DbContext fresco por llamada).
- **Logging:** enviado a *stderr* para no corromper el protocolo JSON-RPC de *stdout*.

## Tools disponibles

Todas operan sobre la entidad `Tarea` y respetan las mismas validaciones que la API
(título 1–200 caracteres, `TipoTareaId` obligatorio, recurrencias, fechas en UTC).

| Tool | Descripción | Parámetros |
|------|-------------|------------|
| `listar_tareas` | Devuelve todas las tareas (orden descendente por fecha de creación). | — |
| `buscar_tareas` | Busca tareas cuyo título contenga un texto (coincidencias parciales, sin distinguir mayúsculas). | `texto` |
| `obtener_tarea` | Devuelve una tarea por `Id` (o `null` si no existe). | `id` |
| `crear_tarea` | Crea una tarea nueva. | `titulo`, `tipoTareaId` (obligatorios); `notas`, `fechaVencimiento`, `prioridad`, `usuarioId` (opcionales) |
| `actualizar_tarea` | Actualiza parcialmente una tarea (estilo PATCH): solo cambia los campos indicados. | `id` (obligatorio); `titulo`, `estaCompletada`, `notas`, `fechaVencimiento`, `prioridad`, `tipoTareaId`, `usuarioId` (opcionales) |
| `completar_tarea` | Marca una tarea como completada; si es repetitiva genera la siguiente ocurrencia. | `id` |
| `eliminar_tarea` | Elimina una tarea por `Id`. | `id` |

## Qué puedes pedirle (lenguaje natural)

- *"¿Cuántas tareas tengo pendientes y cuáles son las más urgentes?"*
- *"Busca tareas que contengan 'demo' en el título."*
- *"Crea una tarea 'Revisar informe trimestral' para el viernes con prioridad alta."*
- *"Cambia la prioridad de la tarea 3 a Normal y actualiza sus notas."*
- *"Marca como completada la tarea número 12."*
- *"Resume mis tareas de esta semana y borra las que ya no apliquen."*
- *"¿Qué tarea es la #5 y qué notas tiene?"*

El agente encadena varias tools (leer → razonar → actuar) para resolver peticiones
compuestas sin tocar la API a mano.

## Casos de uso

- Asistente conversacional de productividad dentro del editor, sin abrir el frontend.
- Automatización guiada por IA (p. ej. preparar tareas recurrentes).
- Puente estándar para que otros agentes/flujos consulten el backend sin acoplarse a los
  endpoints HTTP.
- Demostración didáctica de MCP + reutilización de capas (servicio → transporte).

## Puesta en marcha

> El MCP accede **directamente** a la base de datos SQLite, así que **no necesita el
> backend en marcha** para funcionar. El backend solo hace falta si quieres ver/usar los
> mismos datos vía API. Como `backend/backend.db` ya existe y está migrada, el esquema
> está listo.

### Paso 1 — Compilar (deja el DLL actualizado)

```powershell
dotnet build
```

Repite este paso **cada vez que cambies el código del MCP**: VS Code arranca el DLL, no el
código fuente.

### Paso 2 — Arrancar el servidor MCP

En VS Code:

1. Abre `.vscode/mcp.json` y pulsa **Start** en el CodeLens sobre `"tareas-mcp"` — o abre
   la paleta (`Ctrl+Shift+P`) → **MCP: List Servers** → `tareas-mcp` → **Start Server**.
2. Verás las 7 tools (`listar_tareas`, `buscar_tareas`, `obtener_tarea`, `crear_tarea`,
   `actualizar_tarea`, `completar_tarea`, `eliminar_tarea`) disponibles en **Copilot Chat
   en modo Agente**.

### Paso 3 — Usarlo

Escribe en Copilot Chat (modo Agente):

- *"Lista mis tareas."*
- *"Crea una tarea 'Preparar demo' con tipoTareaId 1 y prioridad Alta."*
- *"Marca como completada la tarea 3."*

### Paso 4 (opcional) — Backend API para ver los mismos datos

Lánzalo desde VS Code (perfil `Backend.Api`) o:

```powershell
dotnet run --project backend
```

Quedará en `https://localhost:55145`. Lo que crees por MCP aparece aquí y viceversa.

### Notas de lanzamiento

- **SQLite concurrente:** permite varios lectores y un escritor; en uso didáctico no da
  problemas. Evita escrituras masivas simultáneas desde backend y MCP a la vez.
- **No borres `backend/backend.db`:** el MCP no aplica migraciones (solo abre el contexto).
  Si se borra, arranca antes el backend una vez para recrear el esquema.
- Tras cambiar el código del servidor, vuelve a ejecutar `dotnet build` y reinicia el
  servidor MCP para cargar el nuevo DLL.

## Límites actuales (por diseño)

- Cubre únicamente la entidad `Tarea` (alcance mínimo inicial).
- Transporte `stdio` local; no está expuesto por red.
- Sin autenticación: pensado para desarrollo local, no para producción.
