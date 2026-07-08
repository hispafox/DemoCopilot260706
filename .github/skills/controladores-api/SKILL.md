---
name: controladores-api
description: 'Crea o actualiza controladores de API ASP.NET Core segun README, analisis/diseno e instrucciones del proyecto, manteniendolos ligeros y coordinados con servicios, logica de negocio y persistencia cuando aplique.'
argument-hint: 'Indica si quieres crear o actualizar el controlador, que operaciones HTTP necesitas, que reglas de validacion o respuesta deben respetarse y si hay impacto en contratos, persistencia, pruebas o documentacion.'
---

# Controladores API

## Objetivo

Crear o actualizar controladores de API ASP.NET Core pequenos, claros y coherentes con las convenciones del repositorio.

## Cuando usar este skill

- Cuando se crea por primera vez un controlador para exponer una funcionalidad del backend.
- Cuando hay que anadir, corregir o quitar endpoints HTTP de un controlador existente.
- Cuando hay que alinear respuestas HTTP, validacion de entrada o contratos con el analisis/diseno y las instrucciones del proyecto.
- Cuando se necesita implementar CRUD simple sin introducir capas adicionales innecesarias.
- Preferiblemente despues de ejecutar el skill infraestructura-dotnet si no esta confirmado que el repo ya tiene proyecto ASP.NET Core compilable.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Diseno tecnico actual: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Guia comun de capas: .github/skills/guia-estilo-capas.md
- Estado real del backend: backend/Controllers, backend/Models, backend/Data, Program.cs, archivos .csproj y contratos relacionados
- Peticion del usuario con las operaciones HTTP o cambios solicitados

## Ubicaciones habituales

Los controladores se gestionan prioritariamente en:

- backend/Controllers

Elementos relacionados que pueden requerir actualizacion si el cambio tiene impacto real:

- backend/Models
- backend/Data
- backend/Data/Migrations
- backend/Program.cs
- backend/*.http (archivo de pruebas manuales HTTP)
- backend/Tests o proyecto de pruebas equivalente
- documentacion/analisis-diseno.md o documentacion/analisis-diseño.md

## Procedimiento

1. Leer README.md para entender el objetivo didactico y el alcance real del repositorio.
2. Leer .github/copilot-instructions.md para respetar las convenciones de arquitectura, nombres, API y persistencia.
3. Leer documentacion/analisis-diseno.md o documentacion/analisis-diseño.md para contrastar el comportamiento esperado y el estado tecnico documentado.
4. Verificar si existe un proyecto ASP.NET Core compilable:
	- buscar archivo .csproj del backend
	- buscar Program.cs o punto de arranque equivalente
	- comprobar si ya existe infraestructura minima para registrar controladores
5. Leer el controlador afectado o, si no existe, los modelos, contratos y contexto cercano que definan la funcionalidad a exponer.
6. Implementar solo el cambio minimo necesario en el controlador o en el conjunto minimo de endpoints afectados.
7. Si el controlador depende de persistencia o contratos que aun no existen o estan desalineados, actualizar solo los elementos relacionados estrictamente necesarios.
8. Crear o actualizar archivo `.http` de prueba manual para los endpoints tocados:
	- ubicarlo en backend junto al proyecto (por ejemplo `backend/Backend.Api.http`)
	- incluir variables base (`@host`) y ejemplos por endpoint cambiado
	- cubrir al menos caso OK y un caso de error representativo (por ejemplo 404 o validacion)
	- si se anade o cambia una propiedad en request/DTO, actualizar todos los payloads relevantes del `.http` (POST/PUT/PATCH) para reflejar el contrato vigente
	- evitar payloads parciales desactualizados: los ejemplos del `.http` deben representar el contrato completo esperado en ese endpoint
9. Verificar coherencia de puertos entre `backend/Properties/launchSettings.json` y el archivo `.http`:
	- usar como referencia `applicationUrl` del perfil activo (HTTP o HTTPS segun proceda)
	- alinear `@host` en `backend/Backend.Api.http` con el puerto real para evitar errores de envio de solicitud
10. Anadir o actualizar pruebas del comportamiento HTTP cuando el cambio no sea trivial y exista proyecto de pruebas o infraestructura razonable para anclar dichas pruebas.
11. Si cambia el diseno tecnico real o el contrato observable, alinear la documentacion tecnica afectada.

## Reglas generales de implementacion

Reglas de arquitectura:

- Mantener la API en ASP.NET Core Web API y no introducir Razor Pages, MVC con vistas, Blazor ni patrones alternativos salvo peticion explicita.
- Mantener los controladores ligeros y centrados en recibir peticiones, validar entradas, coordinar persistencia y devolver respuestas HTTP correctas.
- No introducir capas adicionales como repositorios, CQRS o mediadores salvo peticion explicita.
- No asumir que el proyecto backend ya existe o compila: comprobar primero el estado real del repo antes de cablear servicios, rutas o validaciones de compilacion.

Reglas de deteccion de proyecto:

- Si existe .csproj y punto de arranque, tratar el cambio como implementacion integrada en una API compilable.
- Si no existe .csproj o no existe punto de arranque, limitar el alcance a los archivos de controlador y a los contratos minimos que tengan sentido sin inventar bootstrap ni persistencia no pedida.
- Si falta la infraestructura para compilar, no bloquear el trabajo de controlador por ello: dejar constancia de la limitacion y validar solo lo que el estado real del repo permita.

Reglas de persistencia y acceso a datos:

- Inyectar el DbContext mediante inyeccion de dependencias solo cuando exista realmente persistencia en el proyecto y el cambio deba integrarse con ella.
- Usar APIs asincronas para el acceso a datos cuando exista acceso a datos real en el proyecto.
- Usar AsNoTracking en lecturas cuando sea apropiado y exista EF Core en la solucion.
- En actualizaciones con persistencia real, cargar la entidad, modificar solo las propiedades necesarias y guardar cambios sin sobreingenieria.

Reglas de contrato HTTP:

- Devolver codigos de estado coherentes para lectura, creacion, actualizacion y borrado.
- Usar validacion integrada del framework antes que parseos manuales.
- Devolver errores de validacion claros y predecibles.
- Devolver NotFound solo cuando corresponda semantica y funcionalmente.
- Si cambia un contrato de entrada/salida, el archivo `.http` debe quedar sincronizado en la misma tarea con ejemplos actualizados para esa propiedad.
- Si la peticion solicita una capacidad nueva para un recurso nuevo (por ejemplo, nueva entidad), no basta con exponer su identificador en otro recurso: se deben crear o actualizar endpoints propios del recurso segun el alcance pedido.

Reglas de diseno del codigo:

- Mantener clases y metodos pequenos y faciles de leer.
- Usar nombres explicitos y consistentes con las convenciones del proyecto.
- Evitar logica de negocio compleja dentro del controlador cuando no sea propia del flujo HTTP.
- No duplicar reglas de validacion ya definidas en el modelo o en contratos de entrada sin necesidad real.

Reglas de validacion final:

- Si existe proyecto compilable, ejecutar una validacion estrecha de compilacion o errores del slice tocado.
- Si no existe proyecto compilable, validar sintaxis, coherencia de referencias y alineacion documental, sin fingir una compilacion inexistente.
- Reportar explicitamente cuando no se pudo compilar por ausencia de .csproj, Program.cs o infraestructura equivalente.
- Verificar que el archivo `.http` quede actualizado y ejecutable para probar los endpoints modificados.
- Si la peticion es de implementacion punto a punto, validar que el recurso nuevo quede visible por API (al menos endpoint de lectura y de alta, salvo que el usuario limite explicitamente el alcance).

## Criterios de calidad

- El controlador queda alineado con README, analisis/diseno e instrucciones del proyecto.
- Los endpoints exponen un contrato claro, coherente y minimo.
- Si existe persistencia real, el acceso a datos es asincrono y consistente con Entity Framework Core.
- El cambio es pequeno, legible y directamente trazable a la peticion del usuario.
- Si hay impacto observable, las pruebas y la documentacion tecnica quedan actualizadas.
- Si hay cambios en endpoints, existe archivo `.http` actualizado con ejemplos minimos para validacion manual.
- Si hay cambios en endpoints, existe archivo `.http` actualizado y alineado con el puerto real configurado en `launchSettings.json`.
- Si se agregan propiedades nuevas en contratos, el `.http` incluye esas propiedades en los ejemplos OK y en los escenarios de actualizacion aplicables.
- El resultado no presupone infraestructura inexistente: compila si el proyecto existe y queda explicitamente limitado si el proyecto aun no existe.
- Cuando el alcance incluye nuevo recurso API, el resultado no se considera completo sin controlador/endpoints del recurso y sin ejemplos en `.http` para probarlos.

## Que evitar

- No introducir nombres, rutas o contratos especulativos no respaldados por el contexto del repo o la peticion.
- No crear capas extra por defecto para operaciones CRUD simples.
- No mezclar responsabilidades de interfaz web dentro del controlador de API.
- No hacer refactors amplios fuera del alcance del endpoint o controlador afectado.
- No actualizar documentacion, migraciones o modelos por inercia: solo cuando el cambio lo exige.
- No inventar .csproj, Program.cs, DbContext o cableado de arranque salvo que el usuario pida crear esa infraestructura.
- No cerrar una tarea de controladores con cambios HTTP sin dejar pruebas manuales reproducibles en `.http`.

## Resultado esperado

- Controlador nuevo o actualizado en backend/Controllers.
- Endpoints y respuestas HTTP coherentes con el proyecto.
- Archivo `.http` creado o actualizado para probar manualmente los endpoints tocados.
- Si existe infraestructura de proyecto, controlador integrado y validado dentro de esa infraestructura.
- Si no existe infraestructura de proyecto, controlador preparado al nivel maximo posible sin introducir bootstrap o persistencia no solicitados.
- Si hay impacto real, pruebas y documentacion tecnica actualizadas de forma consistente.