---
name: base-datos-aplicacion
description: 'Crea o actualiza la capa de base de datos segun README, analisis/diseno e instrucciones con Entity Framework Core y SQLite: AppDbContext, Fluent API, registro en Program.cs y migraciones, sin acoplar el skill a nombres concretos de entidad.'
argument-hint: 'Indica si quieres crear o actualizar persistencia, que cambios de modelo deben reflejarse en EF Core y si hay que generar migracion nueva o ajustar una existente.'
---

# Base de Datos de Aplicacion

## Objetivo

Crear o actualizar la capa de base de datos con Entity Framework Core y SQLite, asegurando:

- AppDbContext (como nombre por defecto cuando no exista convencion previa).
- Configuracion de entidades con Fluent API.
- Registro de DbContext en Program.cs.
- Migraciones alineadas con el modelo real.

Este skill se define sin fijar nombres concretos de entidades o recursos.

## Cuando usar este skill

- Cuando el proyecto necesita crear por primera vez la persistencia EF Core + SQLite.
- Cuando cambia el modelo y hay impacto de esquema.
- Cuando falta el DbContext o su registro en DI.
- Cuando hay que consolidar configuraciones Fluent API en lugar de reglas dispersas.
- Cuando se requiere crear o actualizar migraciones.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Analisis/diseno tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Guia comun de capas: .github/skills/guia-estilo-capas.md
- Modelo actual: backend/Models
- Estado de persistencia: backend/Data, backend/Program.cs, backend/*.csproj
- Peticion del usuario con alcance del cambio

## Ubicaciones habituales

- backend/Data/AppDbContext.cs
- backend/Data/Configurations
- backend/Data/Migrations
- backend/Program.cs
- backend/appsettings.json
- backend/Backend.Api.csproj

Respetar convenciones existentes del repo si ya hay rutas o nombres diferentes.

## Procedimiento

1. Leer README.md y .github/copilot-instructions.md para respetar stack, alcance y convenciones.
2. Leer analisis/diseno para identificar operaciones, relaciones y restricciones que impactan persistencia.
3. Leer backend/Models para derivar entidades, propiedades, nullability y relaciones.
4. Verificar infraestructura .NET real (proyecto, Program.cs y paquetes EF Core/SQLite).
5. Crear o actualizar AppDbContext con DbSet y OnModelCreating segun modelo real.
6. Implementar configuraciones Fluent API por entidad (preferiblemente separadas por tipo).
7. Configurar cadena de conexion SQLite en appsettings.json si falta.
8. Registrar DbContext en Program.cs con UseSqlite y connection string.
9. Crear migracion nueva o ajustar flujo de migraciones segun el cambio de esquema.
10. Verificar que las migraciones representan fielmente el modelo actual y no incluyen ruido.
11. Si hay impacto observable o estructural, alinear analisis/diseno tecnico.

## Reglas de implementacion

Reglas de diseno de persistencia:

- Usar un unico DbContext de aplicacion.
- Preferir configuracion Fluent API para reglas de mapeo, claves e indices.
- Evitar duplicar validaciones de dominio en varias capas sin necesidad.
- Mantener compatibilidad con SQLite al definir tipos y restricciones.

Reglas de nombres y alcance:

- AppDbContext es el nombre por defecto cuando no existe convencion previa.
- Si ya existe un DbContext funcional con otro nombre, respetar la convencion existente.
- No fijar nombres concretos de entidades dentro del skill.
- Derivar el mapeo desde el modelo y el analisis/diseno, no desde supuestos.

Reglas de migraciones:

- Toda modificacion de esquema debe ir acompaniada de migracion.
- No editar manualmente la base SQLite para sustituir migraciones.
- Revisar que Up y Down sean consistentes con el cambio solicitado.

Reglas de Program.cs:

- Registrar el DbContext una sola vez en DI.
- No duplicar registros de DbContext ni connection strings equivalentes.

## Gates recomendados

Gate 1 (modelo -> mapeo):
- El AppDbContext y la Fluent API reflejan el modelo real y sus relaciones.

Gate 2 (bootstrap persistencia):
- Program.cs registra DbContext con SQLite y appsettings contiene la cadena de conexion requerida.

Gate 3 (migraciones):
- Existe migracion coherente con el cambio de esquema y sin operaciones ajenas al alcance.

Gate 4 (no acoplamiento):
- El skill y la implementacion no dependen de nombres concretos de entidad en su definicion.

## Criterios de calidad

- Persistencia EF Core + SQLite integrada y consistente.
- AppDbContext claro, pequeno y mantenible.
- Configuracion Fluent API legible y trazable al modelo.
- Migraciones reproducibles y alineadas al cambio real.
- Cambio minimo, sin sobreingenieria.

## Que evitar

- No crear multiples DbContext sin necesidad funcional real.
- No dejar mapeos implicitos ambiguos si el modelo requiere configuracion explicita.
- No duplicar reglas entre data annotations y Fluent API sin motivo.
- No modificar o borrar la base de datos como reemplazo de migraciones.
- No introducir nombres fijos de entidad en la definicion del skill.

## Resultado esperado

- Capa de base de datos creada o actualizada en backend/Data.
- AppDbContext disponible y registrado en Program.cs.
- Configuraciones Fluent API coherentes con el modelo.
- Migraciones actualizadas para sincronizar esquema.
- Documentacion tecnica alineada si hay impacto estructural.
