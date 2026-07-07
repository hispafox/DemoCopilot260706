---
name: dtos-aplicacion
description: 'Crea o actualiza DTOs de la aplicacion a partir del estado real del modelo y de los endpoints documentados en el analisis/diseno, sin acoplarse a nombres fijos de recursos o campos.'
argument-hint: 'Indica si quieres crear o actualizar DTOs, que endpoints o contratos cambian y si hay impacto en validaciones, mapeos o documentacion tecnica.'
---

# DTOs de Aplicacion

## Objetivo

Crear o actualizar DTOs de entrada y salida para la API, manteniendo contratos HTTP claros y minimos, alineados con README, instrucciones del proyecto y analisis/diseno tecnico vigente.

Este skill debe ser generico: no fija nombres de recursos ni campos concretos por defecto.

## Cuando usar este skill

- Cuando se crean endpoints nuevos y falta definir contratos de entrada/salida.
- Cuando cambia un endpoint existente y su contrato HTTP debe actualizarse.
- Cuando el modelo de dominio cambia y hay que ajustar DTOs relacionados.
- Cuando se necesita separar entidad de dominio y contrato de API sin sobrearquitectura.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Analisis/diseno tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Estado real del codigo en backend/Models y backend/Controllers
- Peticion del usuario con alcance de los contratos a crear o ajustar

## Principio rector de este skill

- No hardcodear nombres de recurso ni de campos en la guia del skill.
- Derivar siempre los DTOs desde:
  - el modelo real detectado en backend/Models
  - los endpoints y respuestas documentados en analisis/diseno
- Si el analisis/diseno no cubre algun contrato necesario, registrar hueco documental antes de inventar contratos.

## Ubicaciones habituales

Los DTOs se gestionan preferentemente en una carpeta dedicada del backend, por ejemplo:

- backend/Contracts
- backend/Dto
- backend/DTOs

Usar la convencion real ya existente en el repositorio si existe; si no existe, crear una sola convencion y mantenerla consistente.

## Procedimiento

1. Leer README.md para entender el alcance funcional real.
2. Leer .github/copilot-instructions.md para respetar convenciones de arquitectura, nombres y API.
3. Leer documentacion/analisis-diseno.md o documentacion/analisis-diseño.md para identificar modelo y endpoints vigentes.
4. Inspeccionar backend/Models y backend/Controllers para confirmar estado real de entidades y contratos activos.
5. Determinar por endpoint si se requiere:
   - DTO de entrada
   - DTO de salida
   - reutilizacion de DTO existente
6. Crear o actualizar solo los DTOs minimos necesarios para el cambio solicitado.
7. Alinear validaciones de entrada en DTOs sin duplicar innecesariamente reglas ya cubiertas en otros puntos.
8. Si hay cambio de contrato HTTP, ajustar mapeos minimos en controladores o capa equivalente.
9. Si el contrato observable cambia, actualizar analisis/diseno en la seccion de endpoints y contratos.
10. Validar coherencia final:
   - nombres consistentes
   - nullability correcta
   - compatibilidad con codigos HTTP esperados

## Reglas de implementacion

Reglas de alcance:

- Aplicar el cambio minimo necesario.
- No introducir capas extra (repositorios, CQRS, mediadores) salvo peticion explicita.
- No crear DTOs especulativos para endpoints que no existen.

Reglas de contratos:

- Los DTOs deben reflejar el contrato API, no la persistencia interna.
- Evitar exponer propiedades internas no necesarias en respuestas.
- Para entrada, incluir solo datos requeridos por la operacion.
- Para salida, priorizar claridad y estabilidad de contrato.

Reglas de consistencia:

- Mantener convenciones de nombres del proyecto.
- Mantener tipos y nullability consistentes con el comportamiento real.
- Evitar duplicidad de DTOs con semantica equivalente.

Reglas de validacion:

- Usar validacion declarativa cuando aporte valor real.
- No duplicar la misma validacion en multiples capas sin necesidad.

## Gates recomendados

Gate 1 (modelo):
- Existe correspondencia trazable entre entidades reales y DTOs tocados.

Gate 2 (contrato HTTP):
- Cada endpoint afectado tiene contrato de entrada/salida definido o justificacion de no aplicar.

Gate 3 (documentacion):
- Si cambia el contrato observable, analisis/diseno queda alineado.

## Criterios de calidad

- DTOs derivados de modelo y endpoints reales, no inventados.
- Cambio pequeno, legible y trazable a la peticion.
- Contratos HTTP claros y coherentes con respuestas esperadas.
- Sin sobreingenieria ni acoplamientos innecesarios.

## Que evitar

- No fijar ejemplos con nombres de recursos o campos concretos como regla universal.
- No usar entidades de dominio como contrato API por inercia cuando el contrato requiere desacople.
- No reestructurar carpetas del backend fuera del alcance.
- No actualizar documentacion o controladores por inercia si no hay impacto real.

## Resultado esperado

- DTOs nuevos o actualizados en la carpeta de contratos del backend.
- Contratos de endpoints afectados coherentes con analisis/diseno y codigo real.
- Si hay impacto observable, documentacion tecnica alineada.
