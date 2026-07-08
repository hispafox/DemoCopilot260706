# Plan: Sedes por usuario

## 1. Resumen
La funcionalidad incorpora el concepto de sede en la gestion de personas de la empresa para poder indicar y consultar en que sede trabaja cada usuario.
El objetivo es extender el dominio actual de usuarios/departamentos sin romper los flujos existentes de CRUD.

## 2. Requisitos
- Mantener la arquitectura actual por capas (Models, Contracts, Services, Controllers, Data) y estilo didactico del proyecto.
- Cada usuario debe quedar asociado obligatoriamente a una sede valida, ademas de su departamento obligatorio actual.
- La API debe permitir crear, listar, actualizar y eliminar sedes.
- Crear o actualizar un usuario con sede inexistente debe devolver error de validacion (400).
- No se debe permitir eliminar una sede con usuarios asociados (409 Conflict), siguiendo el mismo criterio ya aplicado para departamentos.
- Persistencia en SQLite mediante EF Core con migracion; no editar base de datos manualmente.
- Mantener asincronia en acceso a datos y AsNoTracking en lecturas.

## 3. Cambios en el modelo
- Nueva entidad Sede en backend/Models/Sede.cs:
  - Id (int)
  - Nombre (string, obligatorio, max 100)
- Cambios en Usuario en backend/Models/Usuario.cs:
  - Nuevo campo SedeId (int, obligatorio)
  - Nueva navegacion Sede (Sede, obligatoria)
- Cambios en Sede para navegacion inversa:
  - Usuarios (ICollection<Usuario>)
- Cambios en ApplicationDbContext:
  - Nuevo DbSet<Sede> Sedes
  - Configuracion Fluent API para:
    - Nombre de Sede requerido y maximo 100
    - Relacion Usuario -> Sede con OnDelete(DeleteBehavior.Restrict)
- Migracion EF Core para crear tabla Sedes, agregar columna SedeId en Usuarios, indice y FK.
- Estrategia de datos para migracion:
  - Insertar sede por defecto (por ejemplo, "Central")
  - Asignar SedeId por defecto a usuarios existentes para evitar nulos en esquema actual.

## 4. DTOs
- En backend/Contracts/ApiContracts.cs:
  - Nuevo SedeDto:
    - Id
    - Nombre
  - Nuevo CrearActualizarSedeRequest:
    - Nombre (Required, MinLength 1 tras trim, StringLength 100)
- Ajustes en UsuarioDto:
  - SedeId
  - SedeNombre
- Ajustes en CrearActualizarUsuarioRequest:
  - SedeId con [Range(1, int.MaxValue, ErrorMessage = "La sede es obligatoria.")]

## 5. Endpoints
- Nuevos endpoints de sedes:
  - GET /api/sedes -> 200 OK con lista de SedeDto
  - GET /api/sedes/{id} -> 200 OK con SedeDto, 404 si no existe
  - POST /api/sedes -> 201 Created, 400 ValidationProblem
  - PUT /api/sedes/{id} -> 200 OK, 404 si no existe, 400 ValidationProblem
  - DELETE /api/sedes/{id} -> 204 NoContent, 404 si no existe, 409 si tiene usuarios
- Endpoints existentes de usuarios afectados:
  - POST /api/usuarios: validar existencia de DepartamentoId y SedeId
  - PUT /api/usuarios/{id}: validar existencia de DepartamentoId y SedeId
  - GET /api/usuarios y GET /api/usuarios/{id}: incluir SedeId y SedeNombre en respuesta

## 6. Logica de negocio
- Mantener controladores ligeros y validacion de existencia de dependencias en capa de aplicacion actual.
- Reglas de usuarios:
  - Usuario no puede crearse ni actualizarse sin sede valida.
  - Si la sede no existe, devolver ValidationProblem en campo SedeId.
- Reglas de sedes:
  - No eliminar sede con usuarios asociados.
- Reglas de calidad tecnica:
  - Consultas de lectura con AsNoTracking.
  - Operaciones de datos con EF Core async.
  - Restricciones y FK reflejadas en migracion compatible con SQLite.

## 7. Capas afectadas
- Models:
  - Nueva entidad Sede.
  - Extension de Usuario con SedeId y navegacion.
- Dtos:
  - Nuevos DTO/request de Sede.
  - Extension de DTO/request de Usuario.
- LogicaNegocio:
  - Reglas de validacion de sede y restriccion de borrado.
- Services:
  - Nuevo ISedesService/SedesService.
  - Ajustes en IUsuariosService/UsuariosService para mapear y persistir SedeId.
- Controllers:
  - Nuevo SedesController.
  - Ajustes en UsuariosController para validar sede.
- Migraciones:
  - Nueva migracion de EF Core y snapshot actualizado.

## 8. Tests a implementar
La excepcion temporal del curso sigue activa para tests (no crear/actualizar pruebas salvo peticion explicita).
Estado para esta funcionalidad: pendiente/no aplicable en esta iteracion.

## 9. Criterios de aceptacion
- Existe CRUD completo de sedes con respuestas HTTP consistentes.
- Usuario siempre se crea y actualiza con DepartamentoId y SedeId validos.
- GET de usuarios devuelve informacion de sede (id y nombre) junto con la de departamento.
- DELETE de sede con usuarios asociados devuelve 409 Conflict.
- Migracion aplicada sin romper datos existentes (usuarios actuales quedan vinculados a sede por defecto).
- Compila backend sin errores y mantiene comportamiento previo de tareas/plantillas/departamentos.

## 10. Skills a invocar
1. infraestructura-dotnet: validar estructura de solucion/proyectos antes de tocar capas.
2. modelo-aplicacion: crear entidad Sede y extender Usuario.
3. base-datos-aplicacion: actualizar DbContext y preparar migracion EF Core SQLite.
4. dtos-aplicacion: crear DTOs de Sede y ampliar contratos de Usuario.
5. validaciones-aplicacion: aplicar validaciones declarativas de entrada y reglas de guarda.
6. logica-negocio: implementar reglas de sede (existencia y bloqueo de borrado con usuarios).
7. servicios-aplicacion: crear servicio de sedes y ajustar servicio de usuarios.
8. controladores-api: exponer endpoints de sedes y adaptar endpoints de usuarios.
9. analisis-diseno: sincronizar documentacion tecnica tras implementar.
10. validador-analisis-prd: verificar consistencia final entre PRD y analisis/diseno.