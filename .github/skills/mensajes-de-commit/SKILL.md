---
name: mensajes-de-commit
description: 'Redacta mensajes de commit en castellano para este proyecto. Úsalo al pedir crear, proponer, revisar o mejorar un mensaje de commit, antes de commitear cambios, o cuando se necesite resumir un diff con asunto específico y cuerpo opcional.'
argument-hint: 'Describe los cambios realizados o indica que revise el diff para proponer el mensaje de commit.'
---

# Mensajes de Commit

## Objetivo

Generar mensajes de commit en castellano, específicos y útiles para este proyecto.

## Cuándo usar este skill

- Cuando se vaya a commitear un cambio y haga falta proponer el mensaje.
- Cuando se quiera revisar si un mensaje de commit es demasiado vago.
- Cuando haya que resumir un diff o un conjunto de cambios en una o varias líneas.

## Reglas

- El mensaje de commit siempre debe estar en castellano.
- La primera línea debe incluir siempre un tipo permitido.
- La primera línea es el resumen: breve, específico y centrado en el cambio real.
- Sé escueto, pero no vago. Un mensaje corto sirve solo si deja claro qué se tocó.
- No uses resúmenes genéricos como fix, cambios, update, wip, actualizar fichero o actualizar instrucciones.
- Describe exactamente qué cambió: archivo, comportamiento, validación, configuración, texto o flujo afectado.
- Redacta el asunto en imperativo y describiendo la acción principal del cambio.
- Cada commit debe representar un cambio lógico y coherente. El mensaje no debe mezclar refactors, cambios funcionales y ajustes de formato no relacionados.
- El cuerpo es opcional, pero valioso cuando aporta contexto real.
- Si el cambio es trivial, una sola línea basta.
- Si el commit incluye pruebas, validación o migraciones, menciónalo solo cuando sea relevante para entender el alcance.

## Formato esperado

Usa este formato por defecto:

```text
<tipo>[ámbito opcional]: <resumen corto en imperativo>

[cuerpo opcional — explica el porqué si el cambio no es obvio]

[footer(s) opcionales]
```

## Tipos permitidos

| Tipo | Cuándo usarlo |
|------|--------------|
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de bug |
| `docs` | Solo documentación |
| `refactor` | Reestructuración sin cambio de comportamiento |
| `test` | Añadir o corregir tests |
| `chore` | Tareas de mantenimiento (deps, config, build) |
| `style` | Formato, sangría, sin cambio lógico |
| `perf` | Mejora de rendimiento |
| `ci` | Cambios en pipelines / GitHub Actions |
| `revert` | Revierte un commit anterior |

El tipo es obligatorio en todos los commits.

## Procedimiento

1. Revisa qué cambió realmente, no solo qué archivos se tocaron.
2. Identifica la acción principal del commit.
3. Redacta una primera línea breve con tipo obligatorio, ámbito opcional y descripción concreta en imperativo.
4. Añade cuerpo solo si hace falta explicar alcance, decisión, validación o efectos colaterales.
5. Comprueba que otra persona pueda entender el cambio sin leer el diff completo.

## Criterios de calidad

- Bueno: docs: cambia ejemplos de código de inglés a castellano
- Malo: docs: actualizar instrucciones
- Bueno: fix: corrige la validación del título al crear tareas
- Malo: fix: arreglos varios
- Bueno: test: añade cobertura del filtro de tareas completadas
- Malo: test: más tests

## Qué evitar

- Nombrar el contenedor en lugar del cambio, por ejemplo actualizar fichero.
- Describir el proceso en vez del resultado.
- Usar resúmenes demasiado amplios cuando el cambio fue concreto.
- Añadir cuerpo de relleno que repita la primera línea con otras palabras.