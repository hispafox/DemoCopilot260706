# Análisis y Diseño — Lista de Tareas

## 1. Objetivo del proyecto

El proyecto busca construir una aplicación web de gestión de tareas con enfoque didáctico para formación en GitHub Copilot.
Según el PRD, el alcance funcional objetivo incluye CRUD de tareas, plantillas y recurrencia básica.
En el estado actual del código, la implementación está en fase inicial y ya incluye entidades de dominio para tareas y categorías.

## 2. Stack tecnológico

| Tecnología | Versión | Rol |
|---|---|---|
| C# | No declarada en el repositorio | Lenguaje del backend |
| .NET (espacios de nombres `System`) | No declarada en el repositorio | Plataforma base del modelo de dominio |
| `System.ComponentModel.DataAnnotations` | Incluida en .NET | Validación declarativa del modelo (`Required`, `StringLength`) |
| Markdown | N/A | Documentación funcional y técnica |

## 3. Arquitectura de capas

| Capa | Carpeta | Responsabilidad |
|---|---|---|
| Dominio | `backend/Models` | Define entidades del negocio |
| Documentación | `documentacion` | PRD, análisis y guías operativas |
| Automatización documental | `scripts` | Pipeline para validar y generar informes |

Árbol de carpetas y archivos clave del estado actual:

```text
backend/
    Models/
        Categoria.cs
        Tarea.cs
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

Elementos no implementados todavía en el código:

- No existe `DbContext` en `backend/Data`.
- No existen controladores en `backend/Controllers`.
- No existen servicios en `backend/Services`.
- No existe frontend en `frontend/`.

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
| EsRepetitiva | `bool` | Indica si la tarea participa en recurrencia |
| TipoRecurrencia | `TipoRecurrencia?` | Frecuencia de repetición (diaria, semanal o mensual) |
| ProximaRecurrencia | `DateTime?` | Próxima fecha planificada para la ocurrencia |
| PlantillaTareaId | `int?` | Identificador opcional de la plantilla origen |
| PlantillaTarea | `PlantillaTarea?` | Navegación hacia la plantilla origen |
| CategoriaId | `int?` | Identificador opcional de la categoría asociada |
| Categoria | `Categoria?` | Navegación hacia la categoría asignada |

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

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public DateTime? ProximaRecurrencia { get; set; }

    public int? PlantillaTareaId { get; set; }

    public PlantillaTarea? PlantillaTarea { get; set; }

    public int? CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}
```

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

## 5. Endpoints API REST

No hay endpoints implementados en el estado actual del código, porque no existen controladores en `backend/Controllers`.

| Verbo | Ruta | Descripción | Respuesta OK | Error |
|---|---|---|---|---|
| N/A | N/A | API REST no implementada todavía en este repositorio | N/A | N/A |

## 6. Decisiones de diseño

- **Modelo canónico en castellano**: la entidad usa los nombres `Tarea`, `Id`, `Titulo`, `EstaCompletada`, `FechaCreacion`, `FechaVencimiento` y `Notas`, coherentes con las convenciones del proyecto.
- **Validación declarativa en el dominio**: `Titulo` se define como obligatorio y con límite máximo de 200 caracteres mediante Data Annotations, reduciendo lógica manual repetitiva.
- **Fecha de creación en UTC**: `FechaCreacion` se inicializa con `DateTime.UtcNow` para mantener consistencia temporal desde el origen de datos.
- **Campos opcionales como anulables**: `FechaVencimiento` y `Notas` se modelan como opcionales para permitir tareas sin fecha límite ni notas.
- **Clasificación visual por categoría**: se incorpora la entidad `Categoria` con `Nombre` y `Color` para habilitar taxonomía funcional y representación visual consistente.
- **Relación de categorización**: `Tarea` incorpora `CategoriaId` y navegación `Categoria`, y `Categoria` expone la colección `Tareas` para representar la asociación en ambos sentidos.
- **Recurrencia explícita en dominio**: `Tarea` incorpora `EsRepetitiva`, `TipoRecurrencia` y `ProximaRecurrencia` para modelar reglas de recurrencia declaradas en PRD.
- **Plantillas reutilizables**: se incorpora la entidad `PlantillaTarea` y su relación opcional con `Tarea` mediante `PlantillaTareaId`.

## 7. Pendientes / Preguntas abiertas

- **Implementación de persistencia**: falta crear `ApplicationDbContext` y configuración de EF Core con SQLite.
- **Implementación de API REST**: faltan controladores y endpoints CRUD de tareas definidos en el PRD.
- **Validación de longitud mínima tras trim**: el PRD/instrucciones exige longitud útil entre 1 y 200 tras trim; el modelo actual no garantiza explícitamente esa regla de trim.
- **API de plantillas de tarea**: falta implementar controladores y endpoints para CRUD de `PlantillaTarea` e instanciación.
- **Flujo de completar con recurrencia**: falta implementar endpoint/caso de uso para completar tarea y generar siguiente ocurrencia de forma idempotente.
- **Frontend React + TypeScript + Vite**: está definido en las instrucciones del proyecto, pero no existe carpeta `frontend` en el repositorio actual.
- **Pruebas automatizadas**: no hay proyecto de tests para validar reglas de negocio ni comportamiento HTTP.
