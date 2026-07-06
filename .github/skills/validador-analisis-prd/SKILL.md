---
name: validador-analisis-prd
description: 'Valida consistencia entre analisis/diseno tecnico y PRD para detectar contradicciones, huecos y desincronizacion de alcance antes de implementar cambios.'
argument-hint: 'Indica las rutas de PRD y analisis/diseno, el contexto de version y si quieres solo hallazgos criticos o informe completo.'
---

# Validador Analisis-PRD

## Objetivo

Comparar dos fuentes de verdad del proyecto:
- PRD (que y por que)
- Analisis/diseno (como tecnico)

Y detectar:
- inconsistencias funcionales
- huecos de trazabilidad
- desalineacion de alcance
- riesgos de implementacion por contradicciones documentales

## Cuando usar este skill

- Antes de iniciar implementacion de una nueva version o hito.
- Antes de cerrar un sprint o preparar demo.
- Cuando se actualiza el PRD pero no esta claro si el analisis tecnico quedo sincronizado.
- Cuando hay defectos por ambiguedad entre negocio y diseno.

## Entradas esperadas

- Ruta del PRD (por defecto: documentacion/PRD.md)
- Ruta del analisis/diseno (por defecto: documentacion/analisis-diseno.md o equivalente real del repo)
- Version objetivo (por ejemplo: v1, v1.1)
- Modo de salida:
  - rapido (solo bloqueantes y altos)
  - completo (todos los hallazgos)

## Metodo de validacion

1. Extraer del PRD:
- objetivos
- alcance incluido / fuera de alcance
- requisitos funcionales (RF)
- reglas de negocio (RN)
- criterios de aceptacion
- requisitos no funcionales

2. Extraer del analisis/diseno:
- modelo de dominio
- endpoints y contratos
- flujos de negocio
- decisiones tecnicas
- pendientes y preguntas abiertas

3. Cruzar informacion con estas verificaciones:
- Cobertura RF -> diseno: cada RF del PRD tiene soporte tecnico explicito.
- Coherencia RN -> modelo/servicio: cada RN se puede implementar con campos y reglas definidas.
- Alineacion de alcance: no hay funcionalidad fuera de alcance tratada como comprometida.
- Coherencia semantica: mismos terminos para las mismas entidades/acciones.
- Coherencia de estados HTTP: criterios de aceptacion compatibles con API propuesta.
- Dependencias y riesgos: riesgos del PRD reflejados en decisiones o pendientes tecnicos.

4. Clasificar hallazgos por severidad:
- Bloqueante: contradiccion que impide validar o implementar correctamente.
- Alto: desalineacion que puede causar retrabajo importante.
- Medio: inconsistencia documental sin bloqueo inmediato.
- Bajo: mejora de claridad o nomenclatura.

5. Calcular indice de sincronizacion documental:
- Formula sugerida:
  - Sincronia (%) = (items alineados / items evaluados) * 100
- Reportar tambien cobertura por categoria (RF, RN, alcance, NFR).

## Formato de salida requerido

Generar siempre el informe con esta estructura:

1. Resumen ejecutivo:
- indice global de sincronia
- total de hallazgos por severidad
- recomendacion de salida (Apto / Apto con riesgos / No apto)

2. Matriz de trazabilidad:
- RF o RN
- evidencia en PRD
- evidencia en analisis/diseno
- estado (Alineado / Parcial / No alineado)
- severidad

3. Hallazgos:
- id (por ejemplo: H-01)
- severidad
- descripcion corta
- impacto
- accion recomendada (con documento a actualizar)

4. Plan de sincronizacion:
- cambios minimos en PRD
- cambios minimos en analisis/diseno
- orden sugerido de actualizacion

5. Criterio de cierre:
- que debe quedar corregido para considerar sincronizados ambos documentos.

## Reglas de evaluacion

- No inventar requisitos que no existan en los documentos.
- Separar claramente hechos (texto citado) de inferencias (interpretacion).
- Priorizar contradicciones funcionales sobre diferencias de estilo.
- Si hay ambiguedad, registrarla como pregunta abierta con propuesta concreta.
- Evitar reescrituras masivas: proponer cambios minimos y verificables.

## Checklist rapido

- Cada RF del PRD tiene endpoint/flujo tecnico asociado.
- Cada RN critica tiene soporte en modelo y logica.
- Incluido/Fuera de alcance es consistente en ambos documentos.
- Las entidades principales tienen nomenclatura estable.
- Los codigos HTTP esperados en PRD coinciden con el analisis tecnico.
- No hay pendientes tecnicos que contradigan compromisos de v1.

## Ejemplo de peticion

"Valida sincronizacion entre documentacion/PRD.md y documentacion/analisis-diseno.md para v1 en modo completo. Prioriza inconsistencias de alcance, reglas de negocio y endpoints."
