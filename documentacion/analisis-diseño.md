# Análisis y Diseño — Lista de Tareas

## 1. Objetivo del proyecto

El proyecto busca construir una aplicación web de gestión de tareas con enfoque didáctico para formación en GitHub Copilot.
Según el PRD, el alcance funcional objetivo incluye CRUD de tareas, plantillas y recurrencia básica.
En el estado actual del código, la implementación está en fase inicial y ya incluye entidades de dominio para tareas y categorías.

## 2. Stack tecnológico

| Tecnología | Versión | Rol |
|---|---|---|
| C# | No declarada en el repositorio | Lenguaje del backend |
| ASP.NET Core Web API | net10.0 | Capa HTTP del backend |
| .NET | net10.0 | Plataforma del backend |
| `System.ComponentModel.DataAnnotations` | Incluida en .NET | Validación declarativa del modelo (`Required`, `StringLength`) |
| Markdown | N/A | Documentación funcional y técnica |

## 3. Arquitectura de capas

| Capa | Carpeta | Responsabilidad |
|---|---|---|
| API | `backend/Controllers` | Endpoints HTTP de tareas, plantillas y usuarios |
| Contratos | `backend/Contracts` | DTOs de entrada y salida para la API |
| Servicios | `backend/Services` | Orquestación de casos de uso y acceso a persistencia |
| Dominio | `backend/Models` | Define entidades del negocio |
| Arranque | `backend/Program.cs` | Configuración mínima de ASP.NET Core y mapeo de controladores |
| Documentación | `documentacion` | PRD, análisis y guías operativas |
| Automatización documental | `scripts` | Pipeline para validar y generar informes |

Árbol de carpetas y archivos clave del estado actual:

```text
DemoCopilot260706.slnx
backend/
    Backend.Api.csproj
    Program.cs
    appsettings.json
    Contracts/
        ApiContracts.cs
    Controllers/
        PlantillasTareaController.cs
        TareasController.cs
        UsuariosController.cs
    Services/
        IPlantillasTareaService.cs
        ITareasService.cs
        IUsuariosService.cs
        PlantillasTareaService.cs
        TareasService.cs
        UsuariosService.cs
    Models/
        Categoria.cs
        PrioridadTarea.cs
        PlantillaTarea.cs
        Tarea.cs
        TipoRecurrencia.cs
        Usuario.cs
documentacion/
    PRD.md
    analisis-diseño.md
scripts/
    documentacion_pipeline.py
    md_to_docx.py
```

Reglas de diseño aplicadas y observables en código actual:

- Entidad de dominio separada en carpeta específica (`backend/Models`).
- Validación por atributos en el propio modelo (`Required`, `StringLength`).
- Uso de `DateTime.UtcNow` para inicializar la fecha de creación en UTC.
- Separación de contratos HTTP en `backend/Contracts` para no exponer entidades internas directamente desde los controladores.

Elementos no implementados todavía en el código:

- No existe frontend en `frontend/`.
- No existe proyecto de pruebas automatizadas para validar reglas de negocio y comportamiento HTTP.

## 4. Modelo de datos

### Tarea

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador de la tarea |
| Titulo | `string` | Título obligatorio de la tarea, máximo 200 caracteres |
| EstaCompletada | `bool` | Estado de completado |
| FechaCreacion | `DateTime` | Fecha de creación inicializada en UTC |
| FechaVencimiento | `DateTime?` | Fecha límite opcional |
| Notas | `string?` | Texto libre opcional |
| Prioridad | `PrioridadTarea` | Prioridad funcional de la tarea (`Baja`, `Normal`, `Alta`, `Urgente`) |
| EsRepetitiva | `bool` | Indica si la tarea participa en recurrencia |
| TipoRecurrencia | `TipoRecurrencia?` | Frecuencia de repetición (diaria, semanal o mensual) |
| ProximaRecurrencia | `DateTime?` | Próxima fecha planificada para la ocurrencia |
| PlantillaTareaId | `int?` | Identificador opcional de la plantilla origen |
| PlantillaTarea | `PlantillaTarea?` | Navegación hacia la plantilla origen |
| CategoriaId | `int?` | Identificador opcional de la categoría asociada |
| Categoria | `Categoria?` | Navegación hacia la categoría asignada |
| UsuarioId | `int?` | Identificador opcional del usuario asignado |
| Usuario | `Usuario?` | Navegación hacia el usuario asignado |

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Tarea
{
    public int Id { get; set; }

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "El titulo no puede estar vacio.")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    public bool EstaCompletada { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVencimiento { get; set; }

    public string? Notas { get; set; }

    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Normal;

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public DateTime? ProximaRecurrencia { get; set; }

    public int? PlantillaTareaId { get; set; }

    public PlantillaTarea? PlantillaTarea { get; set; }

    public int? CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public int? UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }
}
```

### Usuario

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador del usuario |
| Nombre | `string` | Nombre visible del usuario |
| Email | `string?` | Email opcional del usuario |
| DepartamentoId | `int` | Identificador obligatorio del departamento al que pertenece |
| Departamento | `Departamento` | Navegación al departamento del usuario |
| Tareas | `ICollection<Tarea>` | Tareas asignadas al usuario |

### Departamento

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador del departamento |
| Nombre | `string` | Nombre visible del departamento |
| Usuarios | `ICollection<Usuario>` | Usuarios que pertenecen al departamento |

### PlantillaTarea

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador de la plantilla |
| Titulo | `string` | Título obligatorio de la plantilla, máximo 200 caracteres |
| Notas | `string?` | Notas opcionales para la plantilla |
| EsRepetitiva | `bool` | Indica si las tareas creadas desde la plantilla son repetitivas |
| TipoRecurrencia | `TipoRecurrencia?` | Frecuencia por defecto para tareas repetitivas creadas desde plantilla |
| CategoriaId | `int?` | Identificador opcional de categoría por defecto |
| Categoria | `Categoria?` | Navegación a la categoría asociada |
| EstaActiva | `bool` | Indica si la plantilla está disponible para instanciación |

```csharp
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class PlantillaTarea
{
    public int Id { get; set; }

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "El titulo no puede estar vacio.")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    public string? Notas { get; set; }

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public int? CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public bool EstaActiva { get; set; } = true;
}
```

### Categoria

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador de la categoría |
| Nombre | `string` | Nombre visible de la categoría |
| Color | `string` | Color asociado para representación visual |
| EstaActiva | `bool` | Indica si la categoría está activa |
| Tareas | `ICollection<Tarea>` | Tareas asociadas a la categoría |

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = string.Empty;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public Categoria(string nombre, string color)
    {
        Nombre = nombre;
        Color = color;
    }

    public bool EstaActiva { get; set; } = true;
}
```

### TipoRecurrencia

```csharp
namespace Backend.Models;

public enum TipoRecurrencia
{
    Diaria = 1,
    Semanal = 2,
    Mensual = 3
}
```

### PrioridadTarea

```csharp
namespace Backend.Models;

public enum PrioridadTarea
{
    Baja = 1,
    Normal = 2,
    Alta = 3,
    Urgente = 4
}
```

## 5. Endpoints API REST

La API REST está implementada con persistencia en SQLite mediante Entity Framework Core.

| Verbo | Ruta | Descripción | Respuesta OK | Error |
|---|---|---|---|---|
| GET | `/api/tareas` | Lista tareas ordenadas por fecha de creación desc, devolviendo `TareaDto` | `200 OK` | N/A |
| GET | `/api/tareas/{id}` | Obtiene tarea por id, devolviendo `TareaDto` | `200 OK` | `404 Not Found` |
| POST | `/api/tareas` | Crea tarea desde `CrearActualizarTareaRequest` | `201 Created` | `400 ValidationProblem` |
| PUT | `/api/tareas/{id}` | Actualiza tarea desde `CrearActualizarTareaRequest` | `200 OK` | `404 Not Found`, `400 ValidationProblem` |
| DELETE | `/api/tareas/{id}` | Elimina tarea | `204 NoContent` | `404 Not Found` |
| POST | `/api/tareas/{id}/completar` | Marca tarea como completada y genera siguiente ocurrencia si aplica, devolviendo `TareaDto` | `200 OK` | `404 Not Found` |
| POST | `/api/tareas/desde-plantilla/{plantillaId}` | Crea tarea desde plantilla existente y devuelve `TareaDto` | `201 Created` | `404 Not Found` |
| GET | `/api/plantillas` | Lista plantillas devolviendo `PlantillaTareaDto` | `200 OK` | N/A |
| GET | `/api/plantillas/{id}` | Obtiene plantilla por id devolviendo `PlantillaTareaDto` | `200 OK` | `404 Not Found` |
| POST | `/api/plantillas` | Crea plantilla desde `CrearActualizarPlantillaTareaRequest` | `201 Created` | `400 ValidationProblem` |
| PUT | `/api/plantillas/{id}` | Actualiza plantilla desde `CrearActualizarPlantillaTareaRequest` | `200 OK` | `404 Not Found`, `400 ValidationProblem` |
| DELETE | `/api/plantillas/{id}` | Elimina plantilla y desvincula tareas asociadas | `204 NoContent` | `404 Not Found` |
| GET | `/api/usuarios` | Lista usuarios devolviendo `UsuarioDto` | `200 OK` | N/A |
| GET | `/api/usuarios/{id}` | Obtiene usuario por id devolviendo `UsuarioDto` | `200 OK` | `404 Not Found` |
| POST | `/api/usuarios` | Crea usuario desde `CrearActualizarUsuarioRequest` | `201 Created` | `400 ValidationProblem` |
| PUT | `/api/usuarios/{id}` | Actualiza usuario desde `CrearActualizarUsuarioRequest` | `200 OK` | `404 Not Found`, `400 ValidationProblem` |
| DELETE | `/api/usuarios/{id}` | Elimina usuario | `204 NoContent` | `404 Not Found` |
| GET | `/api/departamentos` | Lista departamentos devolviendo `DepartamentoDto` | `200 OK` | N/A |
| GET | `/api/departamentos/{id}` | Obtiene departamento por id devolviendo `DepartamentoDto` | `200 OK` | `404 Not Found` |
| POST | `/api/departamentos` | Crea departamento desde `CrearActualizarDepartamentoRequest` | `201 Created` | `400 ValidationProblem` |
| PUT | `/api/departamentos/{id}` | Actualiza departamento desde `CrearActualizarDepartamentoRequest` | `200 OK` | `404 Not Found`, `400 ValidationProblem` |
| DELETE | `/api/departamentos/{id}` | Elimina departamento si no tiene usuarios asociados | `204 NoContent` | `404 Not Found`, `409 Conflict` |

## 6. Decisiones de diseño

- **Modelo canónico en castellano**: la entidad usa los nombres `Tarea`, `Id`, `Titulo`, `EstaCompletada`, `FechaCreacion`, `FechaVencimiento` y `Notas`, coherentes con las convenciones del proyecto.
- **Validación declarativa en el dominio**: `Titulo` se define como obligatorio y con límite máximo de 200 caracteres mediante Data Annotations, reduciendo lógica manual repetitiva.
- **Fecha de creación en UTC**: `FechaCreacion` se inicializa con `DateTime.UtcNow` para mantener consistencia temporal desde el origen de datos.
- **Campos opcionales como anulables**: `FechaVencimiento` y `Notas` se modelan como opcionales para permitir tareas sin fecha límite ni notas.
- **Priorización explícita de tareas**: `Tarea` incorpora `Prioridad` con valores `Baja`, `Normal`, `Alta` y `Urgente`, con valor por defecto `Normal`.
- **Clasificación visual por categoría**: se incorpora la entidad `Categoria` con `Nombre` y `Color` para habilitar taxonomía funcional y representación visual consistente.
- **Relación de categorización**: `Tarea` incorpora `CategoriaId` y navegación `Categoria`, y `Categoria` expone la colección `Tareas` para representar la asociación en ambos sentidos.
- **Recurrencia explícita en dominio**: `Tarea` incorpora `EsRepetitiva`, `TipoRecurrencia` y `ProximaRecurrencia` para modelar reglas de recurrencia declaradas en PRD.
- **Plantillas reutilizables**: se incorpora la entidad `PlantillaTarea` y su relación opcional con `Tarea` mediante `PlantillaTareaId`.
- **Asignación opcional de responsable**: `Tarea` incorpora `UsuarioId` y navegación `Usuario`, y `Usuario` expone colección `Tareas` para modelar relación 1:N con borrado en `SetNull`.
- **Usuarios con departamento obligatorio**: `Usuario` incorpora `DepartamentoId` y navegación `Departamento`, estableciendo relación 1:N (`Departamento` -> `Usuarios`) con restricción de borrado.
- **Mantenimiento de departamentos**: se incorpora recurso API específico para alta, consulta, edición y eliminación controlada de departamentos.
- **Contratos HTTP desacoplados**: las acciones de API usan DTOs dedicados para entrada y salida, preservando el modelo de dominio interno.

## 7. Pendientes / Preguntas abiertas

- **Consolidar solución**: definir si se mantiene `slnx` como formato oficial o si se incorpora también `.sln` para compatibilidad con tooling externo.
- **Frontend React + TypeScript + Vite**: está definido en las instrucciones del proyecto, pero no existe carpeta `frontend` en el repositorio actual.
- **Pruebas automatizadas**: no hay proyecto de tests para validar reglas de negocio ni comportamiento HTTP.
