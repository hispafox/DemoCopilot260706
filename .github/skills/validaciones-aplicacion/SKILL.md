---
name: validaciones-aplicacion
description: 'Aplica validaciones declarativas en DTOs de entrada y reglas de guarda en la logica de negocio siguiendo solo las restricciones explicitadas en el README, las instrucciones del proyecto y las secciones 4 y 5 del analisis/diseno.'
argument-hint: 'Indica las rutas de README, copilot instructions y analisis/diseno, junto con los DTOs o servicios afectados. El skill extrae las restricciones de la seccion 4 para DataAnnotations y las de la seccion 5 para guards y transiciones de estado.'
---

# Validaciones de Aplicacion

## Objetivo

Recorrer los contratos de entrada y la logica de negocio afectada para aplicar validaciones solo cuando esten respaldadas por el analisis/diseno tecnico vigente.

Este skill no inventa reglas nuevas. Si una validacion no aparece en el analisis, se deja sin tocar o se reporta como hueco documental.

## Cuando usar este skill

- Cuando hay que ajustar DTOs de entrada con DataAnnotations derivadas del modelo y del analisis.
- Cuando hay que introducir o revisar reglas de guarda en servicios o logica de negocio.
- Cuando un cambio afecta a restricciones de campo, obligatoriedad, longitud, nulabilidad o coherencia entre propiedades.
- Cuando un cambio afecta a comprobaciones de existencia, estados permitidos o transiciones de estado.

## Entradas esperadas

- README.md
- .github/copilot-instructions.md
- documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- DTOs de entrada reales en backend/Contracts o la carpeta equivalente del repo
- Logica de negocio o servicios afectados en backend/Services o la carpeta equivalente del repo
- Modelo de dominio en backend/Models cuando una restriccion dependa de una propiedad o estado real

## Fuente de verdad para este skill

- Seccion 4 del analisis/diseno: define las restricciones de datos que deben trasladarse a DTOs de entrada mediante DataAnnotations.
- Seccion 5 del analisis/diseno: define las operaciones de API y el comportamiento esperado que debe materializarse en reglas de guarda y transiciones de estado.

## Procedimiento

1. Leer README.md y .github/copilot-instructions.md para respetar el alcance general y las convenciones del proyecto.
2. Leer el analisis/diseno y extraer de forma literal las restricciones de la seccion 4 y las reglas operativas de la seccion 5.
3. Inspeccionar los DTOs de entrada existentes para identificar que propiedades reciben datos desde la API.
4. Aplicar DataAnnotations solo para restricciones explicitadas en el analisis:
   - Required cuando el campo sea obligatorio.
   - StringLength, MinLength o equivalentes cuando el analisis indique longitudes o vacio no permitido.
   - validaciones cruzadas con IValidatableObject solo cuando la regla dependa de mas de una propiedad.
5. Inspeccionar la logica de negocio o los servicios afectados para ubicar donde deben vivir los guards.
6. Aplicar reglas de guarda solo para lo que la seccion 5 describa:
   - comprobacion de existencia antes de operar
   - transiciones de estado permitidas
   - coherencia entre estados, fechas y banderas
   - comportamiento esperado cuando la entidad ya esta en el estado final previsto
7. Mantener las validaciones en la capa adecuada:
   - contratos de entrada para restricciones de forma y consistencia de datos
   - logica de negocio para guards, existencia y transiciones
8. Si el analisis no define una validacion concreta, no crearla por intuicion.
9. Si una regla parece necesaria pero no esta en el analisis, registrar el hueco documental antes de implementarla.
10. Si el cambio afecta comportamiento observable, actualizar la documentacion tecnica correspondiente solo para reflejar lo ya decidido.

## Reglas de implementacion

- No duplicar la misma validacion en varias capas salvo que el analisis lo exija de forma explicita.
- No mover guards de dominio a controladores.
- No convertir una restriccion de negocio en una validacion de DTO si el analisis la presenta como una decision de estado.- Cuando el cambio requiere comprobar existencia de entidades relacionadas o estados permitidos, implementarlo en servicios o lógica de negocio y no en el controlador.
- Si un endpoint necesita devolver un error de validación por una regla vinculada al dominio, el servicio debe lanzar o devolver un resultado de error claro y el controlador debe traducirlo a la respuesta HTTP adecuada.- No inventar mensajes de error, campos obligatorios o transiciones no documentadas.
- No introducir validaciones adicionales para "endurecer" el sistema si no estan respaldadas por el analisis.

## Gates recomendados

Gate 1 (extraccion de reglas):
- Cada validacion a tocar se puede trazar a una frase o tabla de la seccion 4 o 5 del analisis.

Gate 2 (contratos de entrada):
- Los DTOs de entrada solo reciben DataAnnotations derivadas del analisis, sin ampliaciones especulativas.

Gate 3 (logica de negocio):
- Los guards y transiciones implementadas coinciden con las operaciones y estados descritos en la seccion 5.

Gate 4 (no invencion):
- Toda regla no documentada queda fuera del cambio o se reporta como hueco documental.

## Criterios de calidad

- Las restricciones de entrada quedan expresadas en los DTOs adecuados.
- Las comprobaciones de existencia y de estado quedan en la logica de negocio o servicios, no en la capa HTTP.
- El cambio es minimo, trazable y directamente respaldado por el analisis.
- No aparecen validaciones nuevas sin evidencia documental.

## Que evitar

- No usar este skill para redefinir el dominio.
- No mezclar validacion de forma con reglas de negocio si el analisis las separa.
- No añadir mensajes o reglas que no existan en el analisis.
- No ampliar el alcance a persistencia, controladores o frontend salvo impacto directo y justificado.

## Resultado esperado

- DTOs de entrada alineados con la seccion 4 del analisis.
- Logica de negocio con guards alineados con la seccion 5 del analisis.
- Documentacion tecnica sin contradicciones nuevas.
- Si el analisis no cubre una regla necesaria, queda reportado como hueco y no como validacion inventada.