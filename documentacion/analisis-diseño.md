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
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        public bool EstaCompletada { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaVencimiento { get; set; }

        public string? Notas { get; set; }

        public int? CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }
}
```

### Categoria

| Campo | Tipo | Descripción |
|---|---|---|
| Id | `int` | Identificador de la categoría |
| Nombre | `string` | Nombre visible de la categoría |
| Color | `string` | Color asociado para representación visual |
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
}
```

No hay enums de dominio implementados en el estado actual del repositorio.

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

## 7. Pendientes / Preguntas abiertas

- **Implementación de persistencia**: falta crear `ApplicationDbContext` y configuración de EF Core con SQLite.
- **Implementación de API REST**: faltan controladores y endpoints CRUD de tareas definidos en el PRD.
- **Validación de longitud mínima tras trim**: el PRD/instrucciones exige longitud útil entre 1 y 200 tras trim; el modelo actual no garantiza explícitamente esa regla de trim.
- **Plantillas de tarea**: el PRD incluye CRUD de plantillas e instanciación, pero no existe entidad ni API asociada.
- **Recurrencia de tareas**: el PRD incluye recurrencia diaria/semanal/mensual y endpoint de completar con generación de siguiente ocurrencia; no está implementado.
- **Frontend React + TypeScript + Vite**: está definido en las instrucciones del proyecto, pero no existe carpeta `frontend` en el repositorio actual.
- **Pruebas automatizadas**: no hay proyecto de tests para validar reglas de negocio ni comportamiento HTTP.
