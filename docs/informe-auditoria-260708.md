# Informe de Auditoria de Calidad

Fecha: 2026-07-08

## 1. Resumen ejecutivo

- Veredicto: no-go.
- Recuento de hallazgos por severidad:
	- Critico: 0
	- Mayor: 3
	- Menor: 0
	- Informativo: 2

## 2. Alcance auditado

- Tipo de auditoria: auditoria completa de la aplicacion (estado actual del repositorio).
- Revisado:
	- backend/Program.cs
	- backend/Data/ApplicationDbContext.cs
	- backend/Contracts/ApiContracts.cs
	- backend/Controllers/*.cs
	- backend/Services/*.cs
	- backend/Models/*.cs
	- backend/appsettings.json
	- backend/Data/Migrations/* (inspeccion de presencia y consistencia estructural)
- Frontend:
	- No existe frontend implementado en el repositorio auditado, segun documentacion/analisis-diseño.md#L76.

- Documentos de referencia leidos:
	- encontrado: .github/copilot-instructions.md
	- encontrado: documentacion/analisis-diseño.md
	- encontrado: documentacion/PRD.md
	- encontrado: .github/skills/guia-estilo-capas.md

## 3. Hallazgos detallados

| Severidad | Categoria | Ubicacion (archivo#Lnn) | Descripcion | Regla o estandar violado | Recomendacion |
|-----------|-----------|--------------------------|-------------|--------------------------|---------------|
| Mayor | (f) Practicas EF Core / async / datos | backend/Program.cs#L25 | Se ejecuta migracion con API sincronica (`Database.Migrate()`) durante arranque. | Regla de proyecto: no usar acceso sincronico a base de datos; convencion EF Core del repo prioriza APIs asincronas. | Sustituir por `await dbContext.Database.MigrateAsync()` y adaptar arranque asincrono. |
| Mayor | (d) Separacion de capas / arquitectura | backend/Controllers/TareasController.cs#L48 | El controlador valida existencia de usuario/tipo de tarea y construye errores de regla de negocio (`ModelState.AddModelError`) en capa HTTP. | .github/copilot-instructions.md: controladores ligeros; la logica de negocio debe vivir en servicios/dominio. .github/skills/guia-estilo-capas.md: no duplicar reglas fuera de logica de negocio. | Mover validaciones de existencia y decisiones de negocio a `TareasService`; dejar en controlador solo orquestacion HTTP y traduccion de resultado a codigo de estado. |
| Mayor | (d) Separacion de capas / arquitectura | backend/Controllers/UsuariosController.cs#L47 | El controlador valida reglas de dominio (existencia de departamento/sede/poblacion) en lugar del servicio y repite ese flujo en create/update. | .github/copilot-instructions.md y .github/skills/guia-estilo-capas.md: controladores deben mantenerse ligeros; reglas de negocio en capa de logica/servicio. | Consolidar validacion en `UsuariosService` y devolver resultado tipado para que controlador solo traduzca a HTTP. |
| Informativo | (b) Code smells | backend/Controllers/UsuariosController.cs#L47 | Duplicacion de flujo de validacion de entidades relacionadas entre `Crear` y `Actualizar` (mismo bloque con `DepartamentoId`, `SedeId`, `PoblacionId`). | Principio de simplicidad y no duplicacion (mantenibilidad). | Extraer validacion a servicio y unificar ruta de negocio para evitar divergencias futuras. |
| Informativo | (a) Violaciones de convenciones del repo | documentacion/analisis-diseño.md#L76 | El repositorio no contiene frontend React/TypeScript materializado; no se puede auditar calidad frontend ni accesibilidad real. | Directrices de arquitectura del repo definen backend + frontend React/TypeScript/Vite. | Completar estructura frontend para poder cerrar auditoria full-stack en siguiente iteracion. |
| Mayor | (h) Validacion y manejo de errores | backend/Models/Tarea.cs#L14 | Reglas de validacion de `Titulo` y recurrencia existen en modelo y tambien en DTO de entrada (`CrearActualizarTareaRequest`), generando duplicidad de regla. | .github/copilot-instructions.md: no duplicar reglas de validacion en varios sitios sin aportar valor real. | Elegir un punto canonico de validacion (contrato de entrada + reglas de negocio) y eliminar duplicidad en entidad persistente cuando no sea necesaria. |

## 4. Deuda tecnica y code smells

- Duplicacion de validaciones de negocio en capa HTTP:
	- backend/Controllers/TareasController.cs#L48, backend/Controllers/TareasController.cs#L56, backend/Controllers/TareasController.cs#L77, backend/Controllers/TareasController.cs#L85.
	- backend/Controllers/UsuariosController.cs#L47, backend/Controllers/UsuariosController.cs#L54, backend/Controllers/UsuariosController.cs#L61, backend/Controllers/UsuariosController.cs#L80, backend/Controllers/UsuariosController.cs#L87, backend/Controllers/UsuariosController.cs#L94.
	- Impacto: alto riesgo de inconsistencia futura, mas coste de mantenimiento y mayor superficie de regresion.

- Duplicacion de reglas de validacion entre DTOs y modelos:
	- backend/Models/Tarea.cs#L14, backend/Models/Tarea.cs#L54.
	- backend/Contracts/ApiContracts.cs#L48, backend/Contracts/ApiContracts.cs#L79.
	- backend/Models/PlantillaTarea.cs#L13, backend/Models/PlantillaTarea.cs#L33.
	- backend/Contracts/ApiContracts.cs#L159, backend/Contracts/ApiContracts.cs#L177.
	- Impacto: deuda de sincronizacion de reglas y comportamiento no determinista si una capa evoluciona y otra no.

## 5. Riesgos de seguridad (OWASP)

- No se detectaron vulnerabilidades explotables de OWASP Top 10 con evidencia directa en el alcance auditado.
- Observacion: la ausencia de autenticacion/autorizacion no se reporta como defecto en esta auditoria por estar fuera de alcance de v1 segun documentacion/PRD.md (seccion 5.2).

## 6. Condiciones de bloqueo

Debe corregirse antes de integrar:

1. backend/Program.cs#L25: acceso sincronico a BD en arranque (`Database.Migrate()`).
2. backend/Controllers/TareasController.cs#L48 y backend/Controllers/UsuariosController.cs#L47: reglas de negocio ubicadas en controlador, incumpliendo separacion de capas.
3. backend/Models/Tarea.cs#L14 + backend/Contracts/ApiContracts.cs#L48: duplicacion de validaciones de dominio en capas diferentes.

## 7. Veredicto final

no-go.

Justificacion: existen 3 hallazgos Mayores bloqueantes que comprometen restricciones de arquitectura y de acceso a datos del proyecto (separacion de capas y uso de acceso asincrono a BD), con evidencia trazable en backend/Program.cs#L25, backend/Controllers/TareasController.cs#L48, backend/Controllers/UsuariosController.cs#L47 y backend/Models/Tarea.cs#L14 junto a backend/Contracts/ApiContracts.cs#L48.
