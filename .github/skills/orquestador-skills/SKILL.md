---
name: orquestador-skills
description: 'Coordina y fuerza secuencias de ejecucion entre skills, con gates de control para evitar desalineaciones y aplicar cambios en el orden correcto.'
argument-hint: 'Indica el cambio a realizar. El orquestador usa modo unico del curso (secuencia base completa) y devuelve estado por pasos.'
---

# Orquestador de Skills

## Modo de uso del curso

Este skill se usa en modo unico.

- No usar variantes de plantilla (basica, intermedia o avanzada).
- Ejecutar por defecto la secuencia base completa del repositorio.
- Devolver siempre cierre con estado por pasos.

## Objetivo

Coordinar secuencias de trabajo entre skills con orden explicito, validaciones intermedias y criterios de bloqueo antes de avanzar.

Secuencia base recomendada en este repo:

1. analisis-diseno
2. validador-analisis-prd (opcional recomendado)
3. modelo-aplicacion

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
- PRD del repo: documentacion/PRD.md (si existe).
- Analisis tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md (segun el nombre real en el repo).
- Estado real del codigo en backend/Models y elementos relacionados.
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

2. Definicion de secuencia
- Confirmar la cadena de skills a ejecutar en orden.
- Definir que condiciones bloquean el avance entre pasos.

3. Ejecucion del skill inicial
- Ejecutar el primer skill de la cadena.
- Verificar evidencia de salida antes de continuar.

4. Gate 1 (obligatorio)
- Verificar que el resultado del skill inicial cumple criterio minimo.
- Si no se pudo actualizar, detener flujo y reportar bloqueo.

5. Ejecucion de skills intermedios
- Ejecutar cada skill intermedio en el orden definido.
- Aplicar su gate correspondiente antes de pasar al siguiente.

6. Gate final (condicional)
- Si hay hallazgos Bloqueante, detener flujo y proponer correcciones minimas.
- Si hay hallazgos Alto, continuar solo si usuario acepta riesgo o si se corrigen antes.
- Si no hay bloqueantes, continuar.

7. Ejecucion del skill de cierre
- Ejecutar el skill final de la secuencia.
- Actualizar solo elementos relacionados cuando haya impacto real.

8. Cierre y reporte
- Entregar resumen de ejecucion con:
  - estado por paso (OK, Omitido, Bloqueado)
  - archivos actualizados
  - riesgos pendientes
  - siguiente accion recomendada

## Mapa de decisiones rapido

- Si no se especifica secuencia, usar la secuencia base del repo.
- Si hay validacion recomendada, intentar ejecutarla; si falla por causa tecnica, continuar con nota de riesgo.
- Si hay validacion obligatoria, no continuar al siguiente skill sin validacion completada.

## Formato de salida sugerido

Usar una salida breve y trazable:

- Paso 1 <skill>: OK | Omitido | Bloqueado
- Paso 2 <skill>: OK | Omitido | Bloqueado
- Paso N <skill>: OK | Omitido | Bloqueado
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

## Que evitar

- No ejecutar skills por inercia sin validar prerequisitos.
- No omitir validacion PRD sin dejar justificacion explicita.
- No hacer refactors fuera del alcance funcional solicitado.
- No marcar como OK un paso sin evidencia en repositorio.

## Ejemplo de peticion

'Aplica orquestador-skills con secuencia analisis-diseno -> validador-analisis-prd -> modelo-aplicacion para agregar prioridad a Tarea, con validacion PRD en modo obligatorio y reporte final por pasos.'
