# Plan: Población de residencia del usuario

## 1. Resumen

Se introduce la entidad `Poblacion` (municipio o localidad) y se asocia obligatoriamente a cada `Usuario`, de modo que quede registrado en qué población vive. El patrón es idéntico al ya existente para `Sede` y `Departamento`: entidad con CRUD propio + relación 1:N hacia `Usuario`.

## 2. Requisitos

- Debe existir una entidad `Poblacion` con al menos `Id` y `Nombre`.
- Cada `Usuario` debe pertenecer a una `Poblacion` (relación obligatoria).
- La API debe exponer CRUD completo para `Poblacion`.
- Al crear o actualizar un usuario, el campo `PoblacionId` es obligatorio.
- No puede eliminarse una `Poblacion` que tenga usuarios asociados (comportamiento igual a `Sede`).
- La respuesta al consultar un usuario (`UsuarioDto`) debe incluir `PoblacionId` y `PoblacionNombre`.
- `Nombre` en `Poblacion` es obligatorio, sin espacios en extremos, máximo 100 caracteres.

## 3. Cambios en el modelo

### Nueva entidad `Poblacion`

| Campo | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Clave primaria |
| `Nombre` | `string` | Nombre de la población, obligatorio, máximo 100 caracteres |
| `Usuarios` | `ICollection<Usuario>` | Usuarios que viven en esta población |

```csharp
// backend/Models/Poblacion.cs
public class Poblacion
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
```

### Modificación en `Usuario`

Añadir dos nuevas propiedades:

```csharp
public int PoblacionId { get; set; }

public Poblacion Poblacion { get; set; } = null!;
```

El modelo `Usuario` queda con: `Id`, `Nombre`, `Email`, `DepartamentoId`, `Departamento`, `SedeId`, `Sede`, **`PoblacionId`**, **`Poblacion`**, `Tareas`.

## 4. DTOs

### `PoblacionDto` (salida)

| Campo | Tipo |
|---|---|
| `Id` | `int` |
| `Nombre` | `string` |

### `CrearActualizarPoblacionRequest` (entrada)

| Campo | Tipo | Validación |
|---|---|---|
| `Nombre` | `string` | `[Required]`, `[StringLength(100)]`, `[MinLength(1)]`, trim en setter |

### Cambios en `UsuarioDto`

Añadir:
- `PoblacionId` (`int`)
- `PoblacionNombre` (`string`)

### Cambios en `CrearActualizarUsuarioRequest`

Añadir:
- `PoblacionId` (`int`) con `[Range(1, int.MaxValue, ErrorMessage = "La poblacion es obligatoria.")]`

## 5. Endpoints

### Nuevos endpoints `Poblacion` — ruta base `/api/poblaciones`

| Verbo | Ruta | Descripción | Respuesta OK | Error |
|---|---|---|---|---|
| GET | `/api/poblaciones` | Lista todas las poblaciones | `200 OK` | — |
| GET | `/api/poblaciones/{id}` | Obtiene población por id | `200 OK` | `404 Not Found` |
| POST | `/api/poblaciones` | Crea población | `201 Created` | `400 ValidationProblem` |
| PUT | `/api/poblaciones/{id}` | Actualiza población | `200 OK` | `404 Not Found`, `400 ValidationProblem` |
| DELETE | `/api/poblaciones/{id}` | Elimina población si no tiene usuarios | `204 NoContent` | `404 Not Found`, `409 Conflict` |

### Ejemplos para `Backend.Api.http` (puerto `http://localhost:55146`)

```http
### Listar poblaciones
GET {{host}}/api/poblaciones
Accept: {{json}}

### Crear población (201)
POST {{host}}/api/poblaciones
Content-Type: {{json}}
Accept: {{json}}

{
  "nombre": "Barcelona"
}

### Obtener población inexistente (error esperado 404)
GET {{host}}/api/poblaciones/999999
Accept: {{json}}

### Crear población inválida (400)
POST {{host}}/api/poblaciones
Content-Type: {{json}}
Accept: {{json}}

{
  "nombre": "   "
}

### Eliminar población con usuarios asociados (error esperado 409)
DELETE {{host}}/api/poblaciones/1
Accept: {{json}}
```

### Endpoint afectado: `POST /api/usuarios` y `PUT /api/usuarios/{id}`

El body de creación y actualización de usuario debe incluir `poblacionId`. Añadir ejemplo al `.http`:

```http
### Crear usuario con poblacion (201)
POST {{host}}/api/usuarios
Content-Type: {{json}}
Accept: {{json}}

{
  "nombre": "Usuario Demo",
  "email": "usuario.demo@example.com",
  "departamentoId": 1,
  "sedeId": 1,
  "poblacionId": 1
}
```

## 6. Lógica de negocio

- **`PoblacionId` obligatorio en `Usuario`**: la validación se aplica mediante `[Range(1, int.MaxValue)]` en `CrearActualizarUsuarioRequest`. No se duplica en el modelo de dominio.
- **Borrado protegido de `Poblacion`**: al eliminar una `Poblacion`, verificar primero si tiene usuarios asociados; si los hay, devolver `409 Conflict` (igual que `Sede`).
- **Configuración Fluent API en `ApplicationDbContext`**: relación `Usuario` → `Poblacion` con `OnDelete(DeleteBehavior.Restrict)` para impedir borrado en cascada.
- **Proyección en el servicio**: al mapear `Usuario` → `UsuarioDto`, incluir `PoblacionId` y `PoblacionNombre` (requiere `.Include(u => u.Poblacion)` en la consulta, usando `AsNoTracking` en lecturas).
- **Migración EF Core**: generar migración para la nueva tabla `Poblaciones` y la nueva columna `PoblacionId` en `Usuarios`. No editar la base de datos manualmente.
- **Nombre con trim**: `CrearActualizarPoblacionRequest.Nombre` aplica trim en el setter, igual que los otros `Request` del proyecto.
- **SQLite**: la columna `PoblacionId` en `Usuarios` es `NOT NULL` (entero no anulable); la migración debe reflejarlo correctamente.

## 7. Capas afectadas

| Capa | Archivo | Cambio |
|---|---|---|
| **Models** | `backend/Models/Poblacion.cs` | Nueva entidad |
| **Models** | `backend/Models/Usuario.cs` | Añadir `PoblacionId` y `Poblacion` |
| **Data** | `backend/Data/ApplicationDbContext.cs` | Nuevo `DbSet<Poblacion> Poblaciones`, configuración Fluent API de relación `Usuario → Poblacion` |
| **Migraciones** | `backend/Data/Migrations/` | Nueva migración: tabla `Poblaciones` + columna `PoblacionId` en `Usuarios` |
| **Contracts** | `backend/Contracts/ApiContracts.cs` | Nuevos `PoblacionDto` y `CrearActualizarPoblacionRequest`; añadir `PoblacionId`/`PoblacionNombre` a `UsuarioDto`; añadir `PoblacionId` a `CrearActualizarUsuarioRequest` |
| **Services** | `backend/Services/IPoblacionesService.cs` | Nueva interfaz de servicio |
| **Services** | `backend/Services/PoblacionesService.cs` | Nueva implementación |
| **Services** | `backend/Services/UsuariosService.cs` | Actualizar mapeo a `UsuarioDto` para incluir `PoblacionId`/`PoblacionNombre`; añadir `.Include(u => u.Poblacion)` |
| **Controllers** | `backend/Controllers/PoblacionesController.cs` | Nuevo controlador CRUD |
| **Program.cs** | `backend/Program.cs` | Registrar `IPoblacionesService` → `PoblacionesService` |
| **HTTP** | `backend/Backend.Api.http` | Añadir ejemplos OK y error para los cinco endpoints de `/api/poblaciones` y actualizar ejemplo de creación de usuario con `poblacionId` |

## 8. Tests a implementar

> **Excepción temporal activa**: según las instrucciones del proyecto, no se crean ni actualizan tests salvo petición expresa del usuario. Esta sección queda pendiente/no aplicable hasta que se reactive la cobertura.

Cuando se reactive testing, los tests previstos son:
- Servicio: crear, obtener, actualizar, eliminar población (incluyendo caso de eliminación con usuarios asociados → error).
- Servicio: mapeo correcto de `PoblacionId`/`PoblacionNombre` en `UsuarioDto`.
- Controlador/integración: `POST /api/poblaciones` con nombre vacío → `400`; con nombre válido → `201`.
- Controlador/integración: `DELETE /api/poblaciones/{id}` con usuarios → `409`; sin usuarios → `204`.

## 9. Criterios de aceptación

- `GET /api/poblaciones` devuelve lista de poblaciones con `200 OK`.
- `POST /api/poblaciones` con nombre válido devuelve `201 Created`.
- `POST /api/poblaciones` con nombre vacío o solo espacios devuelve `400 ValidationProblem`.
- `GET /api/poblaciones/999999` devuelve `404 Not Found`.
- `DELETE /api/poblaciones/{id}` sobre una población con usuarios devuelve `409 Conflict`.
- `DELETE /api/poblaciones/{id}` sobre una población sin usuarios devuelve `204 NoContent`.
- `POST /api/usuarios` y `PUT /api/usuarios/{id}` sin `poblacionId` válido devuelven `400 ValidationProblem`.
- `GET /api/usuarios` y `GET /api/usuarios/{id}` incluyen `poblacionId` y `poblacionNombre` en la respuesta.
- La migración aplica correctamente en SQLite sin errores.
- **El archivo `backend/Backend.Api.http` incluye ejemplos OK y de error para cada endpoint nuevo o modificado, alineado con el puerto real `http://localhost:55146` de `backend/Properties/launchSettings.json`.**

## 10. Skills a invocar

Dado que el cambio abarca varias capas (modelo, persistencia, contratos, servicios y controlador), el desarrollador debe coordinarlos mediante el skill **`orquestador-skills`**, que fuerza la secuencia base del repositorio con sus gates de control:

1. **`modelo-aplicacion`** — Crear `Poblacion.cs` y modificar `Usuario.cs`.
2. **`base-datos-aplicacion`** — Actualizar `ApplicationDbContext` (nuevo `DbSet`, Fluent API) y generar la migración.
3. **`dtos-aplicacion`** — Añadir `PoblacionDto`, `CrearActualizarPoblacionRequest` y actualizar `UsuarioDto` / `CrearActualizarUsuarioRequest`.
4. **`validaciones-aplicacion`** — Verificar que las validaciones declarativas del request cubren los requisitos (trim, rango mínimo, longitud máxima).
5. **`logica-negocio`** — No hay lógica de negocio compleja; confirmar regla de borrado protegido en el servicio.
6. **`servicios-aplicacion`** — Crear `IPoblacionesService` / `PoblacionesService`; actualizar `UsuariosService` con include y proyección; registrar en `Program.cs`.
7. **`controladores-api`** — Crear `PoblacionesController`; actualizar `Backend.Api.http`.
