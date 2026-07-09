---
name: orquestador-skills
description: 'Coordina y fuerza secuencias de ejecucion entre skills con gates de control para aplicar cambios en el orden correcto, incluyendo modelo, base de datos, logica de negocio, servicios y controladores.'
argument-hint: 'Indica el cambio a realizar. El orquestador usa modo unico del curso, aplica la secuencia base segun alcance real y devuelve estado por pasos.'
---

# Orquestador de Skills

## Modo de uso del curso

Este skill se usa en modo unico.

- No usar variantes de plantilla (basica, intermedia o avanzada).
- Ejecutar por defecto la secuencia base del repositorio, omitiendo solo los pasos cuyo alcance no aplique al cambio solicitado.
- Devolver siempre cierre con estado por pasos.

## Objetivo

Coordinar secuencias de trabajo entre skills con orden explicito, validaciones intermedias y criterios de bloqueo antes de avanzar.

Secuencia base recomendada en este repo:

0. github-ops (Fase 1 — Apertura): crear Issue en GitHub + crear rama `feat/<N>-<slug>` vinculada. Captura el numero de issue `#N` y el nombre de rama; ambos deben propagarse al planificador y al resto del flujo.
1. infraestructura-dotnet
2. analisis-diseno
3. validador-analisis-prd (opcional recomendado)
4. modelo-aplicacion
5. base-datos-aplicacion (cuando el cambio alcanza persistencia, DbContext o migraciones)
6. logica-negocio (cuando el cambio alcanza reglas y decisiones de dominio)
7. dtos-aplicacion (cuando el cambio alcanza contratos HTTP)
8. validaciones-aplicacion (cuando el cambio alcanza restricciones de entrada o guards de dominio explicitados en el analisis)
9. servicios-aplicacion (cuando el cambio alcanza logica de aplicacion y orquestacion)
10. controladores-api (cuando el cambio alcanza endpoints HTTP)
11. github-ops (Fase 2 — Cierre): ejecutar solo cuando verificador-democopilot emite APROBADO. Hace git add, git commit (usando mensajes-de-commit con `Refs #N` en el footer), git push y crea el Pull Request vinculado al issue (`Closes #N`, base `main`).

El objetivo es reducir errores por saltarse pasos, mantener trazabilidad y permitir orquestar tambien otras cadenas de skills cuando el curso lo requiera.

## Cuando usar este skill

- Siempre que se vaya a ejecutar una cadena de varios skills dependientes entre si.
- Siempre que se arranque una nueva practica, modulo o hito del curso.
- Cuando haya dudas sobre el orden correcto de ejecucion entre skills.
- Cuando diferentes personas o agentes participen en la misma cadena de cambios.

## Entradas esperadas

- Peticion del usuario y resultado esperado.
- Secuencia objetivo de skills (al menos skill inicial y skill final).
- Gates o validaciones obligatorias antes de avanzar de fase.
- Guia comun de capas: .github/skills/guia-estilo-capas.md.
- PRD del repo: documentacion/PRD.md (si existe).
- Analisis tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md (segun el nombre real en el repo).
- Estado de infraestructura .NET: .csproj, .sln o .slnx, Program.cs, proyectos de pruebas y referencias NuGet si existen.
- Estado real del codigo en backend/Models, backend/Services, backend/Controllers y elementos relacionados.
- Modo de validacion PRD:
  - recomendado (por defecto)
  - obligatorio
  - omitido (solo con justificativo explicito)

## Politica de secuencia (regla dura)

Nunca ejecutar un skill de implementacion antes de completar el skill previo definido como prerequisito.

Si un gate es obligatorio, nunca ejecutar el siguiente skill hasta completar validacion y resolver bloqueantes.

## Flujo operativo

1. Preparacion de contexto
- Leer .github/copilot-instructions.md.
- Leer README.md y peticion del usuario.
- Identificar alcance y artefactos impactados.

2. Apertura GitHub (Step 0 — obligatorio)
- Invocar github-ops Fase 1: crear Issue en GitHub y crear rama `feat/<N>-<slug>`.
- Capturar numero de issue `#N` y nombre de rama exacto.
- Propagar `#N` y nombre de rama al planificador (para que los incluya en el encabezado del plan).
- Si github-ops Fase 1 falla, detener flujo y reportar bloqueo.

3. Comprobacion de infraestructura
- Ejecutar infraestructura-dotnet para determinar si existe proyecto compilable o solo codigo fuente.
- Fijar el nivel real de validacion posible para el resto de la cadena.
- Si la peticion incluye crear proyecto o infraestructura, ejecutar infraestructura-dotnet en modo diagnostico-y-bootstrap.

3. Definicion de secuencia
- Confirmar la cadena de skills a ejecutar en orden.
- Definir que condiciones bloquean el avance entre pasos.

4. Ejecucion del skill inicial
- Ejecutar el primer skill de la cadena.
- Verificar evidencia de salida antes de continuar.

5. Gate 1 (obligatorio)
- Verificar que el resultado del skill inicial cumple criterio minimo.
- Si no se pudo actualizar, detener flujo y reportar bloqueo.

6. Ejecucion de skills intermedios
- Ejecutar cada skill intermedio en el orden definido.
- Aplicar su gate correspondiente antes de pasar al siguiente.

7. Gate final (condicional)
- Si hay hallazgos Bloqueante, detener flujo y proponer correcciones minimas.
- Si hay hallazgos Alto, continuar solo si usuario acepta riesgo o si se corrigen antes.
- Si no hay bloqueantes, continuar.

8. Gate de cobertura extremo a extremo (obligatorio cuando se pide nueva funcionalidad)
- Antes de cerrar, verificar cobertura completa por capas para la funcionalidad solicitada.
- Si falta una capa requerida, no cerrar como OK global: marcar Bloqueado o Parcial con causa concreta.
- Checklist minimo por capacidad nueva:
  - Modelo y relaciones de dominio.
  - Persistencia (DbContext/Fluent API y migracion si hay impacto de esquema).
  - Contratos HTTP (DTOs request/response) cuando hay endpoints.
  - Servicios de aplicacion (interfaz, implementacion y registro DI en Program.cs) cuando hay orquestacion.
  - Controlador/endpoints para el recurso nuevo cuando el alcance incluye exponerlo por API.
  - Archivo .http actualizado para validar manualmente casos OK y error.
  - Compilacion del backend o validacion tecnica equivalente si no hay infraestructura compilable.
  - Documentacion tecnica alineada cuando cambia el comportamiento observable.

9. Ejecucion del skill de cierre
- Ejecutar el skill final de la secuencia.
- Actualizar solo elementos relacionados cuando haya impacto real.

10. Cierre GitHub (Step Final — obligatorio cuando verificador emite APROBADO)
- Invocar github-ops Fase 2: git add, git commit (mensajes-de-commit con `Refs #N`), git push y crear PR (`Closes #N`, base `main`).
- No ejecutar este paso si el veredicto del verificador es REVISAR.
- Si github-ops Fase 2 falla en cualquier sub-paso, reportar bloqueo con causa exacta y accion correctiva.

11. Cierre y reporte
- Entregar resumen de ejecucion con:
  - estado por paso (OK, Omitido, Bloqueado)
  - cobertura extremo a extremo (Completa, Parcial o No aplica)
  - archivos actualizados
  - riesgos pendientes
  - siguiente accion recomendada

## Mapa de decisiones rapido

- Si no se especifica secuencia, usar la secuencia base del repo.
- Si no se ha comprobado antes la infraestructura .NET, ejecutar infraestructura-dotnet como primer paso.
- Si el usuario pide crear proyecto/infraestructura y faltan .sln/.csproj/Program.cs, infraestructura-dotnet debe crear bootstrap minimo y validar build antes de continuar.
- Si hay validacion recomendada, intentar ejecutarla; si falla por causa tecnica, continuar con nota de riesgo.
- Si hay validacion obligatoria, no continuar al siguiente skill sin validacion completada.
- Si un paso de la secuencia no aplica por alcance real, marcarlo como Omitido en el cierre y justificarlo brevemente.
- Si el cambio alcanza persistencia o esquema, insertar base-datos-aplicacion despues de modelo-aplicacion y antes de logica-negocio.
- Si el cambio alcanza contratos HTTP, no cerrar la cadena en modelo-aplicacion: continuar con dtos-aplicacion y despues controladores-api si hay impacto en endpoints.
- Si el cambio alcanza restricciones de entrada o guards derivados del analisis, insertar validaciones-aplicacion despues de dtos-aplicacion y antes de servicios-aplicacion.
- Si el cambio alcanza reglas de dominio, insertar logica-negocio despues de modelo-aplicacion y antes de servicios-aplicacion.
- Si el cambio alcanza logica de aplicacion, insertar servicios-aplicacion despues de logica-negocio y antes de controladores-api.
- Si no existen servicios requeridos por controladores, priorizar servicios-aplicacion para crearlos y registrar su inyeccion en Program.cs antes de tocar endpoints, consumiendo la logica-negocio cuando aplique.
- Si el usuario pide una nueva funcionalidad de punto a punto, no cerrar el flujo al completar solo modelo o persistencia: ejecutar todos los pasos aplicables hasta exponer la capacidad final solicitada.
- Si el usuario pide una nueva entidad con relacion a otra existente y espera gestionarla por API, crear tambien su servicio y controlador; no limitarse a agregar la FK en la entidad existente.
- Si una nueva capacidad queda parcialmente implementada por bloqueo tecnico, reportar Bloqueado con el checklist de capas faltantes y la siguiente accion exacta.

## Formato de salida sugerido

Usar una salida breve y trazable:

- Paso 0 github-ops (Apertura): OK | Bloqueado — Issue #N / Rama: feat/<N>-<slug>
- Paso 1 <skill>: OK | Omitido | Bloqueado
- Paso 2 <skill>: OK | Omitido | Bloqueado
- Paso N <skill>: OK | Omitido | Bloqueado
- Paso Final github-ops (Cierre): OK | Bloqueado — PR #<N-PR> / <URL>
- Cobertura E2E: Completa | Parcial | No aplica
- Archivos tocados:
  - <ruta>
- Riesgos abiertos:
  - <riesgo>
- Proxima accion:
  - <accion concreta>

## Criterios de calidad

- Nunca se rompe el orden de la secuencia declarada.
- Cada paso deja evidencia en archivos del repo.
- Los bloqueos se reportan con causa concreta y accion correctiva.
- El resultado final queda alineado con el flujo declarado y sus gates.
- Cuando el usuario pide crear infraestructura, el paso infraestructura-dotnet no puede marcarse OK sin evidencia de bootstrap minimo o justificacion tecnica de bloqueo.
- Una peticion de nueva funcionalidad no se considera completada si falta cualquier capa aplicable del checklist E2E.

## Que evitar

- No ejecutar skills por inercia sin validar prerequisitos.
- No omitir validacion PRD sin dejar justificacion explicita.
- No hacer refactors fuera del alcance funcional solicitado.
- No marcar como OK un paso sin evidencia en repositorio.

## Ejemplo de peticion

'Aplica orquestador-skills con secuencia infraestructura-dotnet -> analisis-diseno -> validador-analisis-prd -> modelo-aplicacion -> base-datos-aplicacion -> logica-negocio -> dtos-aplicacion -> servicios-aplicacion -> controladores-api para agregar un cambio con impacto en persistencia, reglas de negocio, contratos, servicios y endpoints API, con validacion PRD en modo obligatorio y reporte final por pasos.'
