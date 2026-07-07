---
name: modelo-aplicacion
description: 'Crea o actualiza clases de modelo de dominio. Si hay cambios con impacto, actualiza tambien elementos relacionados; si no los hay, solo modifica el modelo.'
argument-hint: 'Indica si quieres crear o actualizar el modelo, que campos o reglas cambian y si esperas impacto en elementos relacionados (DbContext, migraciones, contratos o documentacion).'
---

# Modelo de Aplicacion

## Objetivo

Crear o actualizar clases de modelo de dominio con cambios pequenos, claros y coherentes con las convenciones del repositorio.

## Cuando usar este skill

- Cuando se crea por primera vez una entidad de dominio.
- Cuando se anade, elimina o ajusta una propiedad de una clase de modelo.
- Cuando cambian validaciones del modelo (por ejemplo, obligatoriedad, longitud o formato).
- Cuando hay que alinear el modelo con el analisis/diseno o con las instrucciones del proyecto.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Diseno tecnico actual: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Modelo actual: una o varias clases en backend/Models
- Peticion del usuario con el cambio solicitado

## Ubicaciones habituales

Las clases de modelo se gestionan prioritariamente en:

- backend/Models

Elementos relacionados que pueden requerir actualizacion si el cambio impacta contrato o persistencia:

- backend/Data (DbContext y configuracion EF Core)
- backend/Data/Migrations (si cambia el esquema)
- backend/Controllers (si cambia el contrato HTTP)
- documentacion/analisis-diseno.md o documentacion/analisis-diseño.md (si cambia el diseno tecnico real)

## Procedimiento

1. Leer README.md para entender el objetivo didactico y alcance del repositorio.
2. Leer .github/copilot-instructions.md para respetar convenciones del proyecto.
3. Leer documentacion/analisis-diseno.md o documentacion/analisis-diseño.md para contrastar el estado esperado del modelo.
4. Leer las clases de backend/Models afectadas por la peticion y tomarlas como fuente de verdad del estado actual.
5. Aplicar solo el cambio minimo necesario en el modelo, sin refactors fuera de alcance.
6. Evaluar si hay elementos relacionados impactados por el cambio:
	- Si no hay impacto real, detenerse en el modelo.
	- Si hay impacto real, actualizar tambien los elementos relacionados minimos necesarios.
7. Verificar que el resultado conserva nombres canonicos, tipos correctos y consistencia entre archivos tocados.
8. Si se toco contrato funcional o persistencia, alinear la documentacion tecnica afectada.

## Reglas generales de modelo

Reglas de validacion:

- Aplicar validaciones declarativas cuando aporten valor y sean coherentes con el proyecto.
- Evitar duplicar la misma regla en varias capas sin necesidad.

Reglas de consistencia:

- Respetar convenciones de nombres definidas en las instrucciones del proyecto.
- Mantener nullability explicita para campos opcionales.
- No introducir propiedades especulativas no pedidas.
- Conservar o ajustar tipos de datos segun el uso real del dominio.

## Criterios de calidad

- Cambio pequeno y directamente trazable a la peticion.
- Modelo compilable y legible.
- Si no hay impacto relacionado, solo se modifica el modelo.
- Si hay impacto relacionado, se actualizan solo los elementos estrictamente necesarios.
- Si el cambio modifica contrato funcional o estructura real, dejar el analisis/diseno alineado.

## Que evitar

- No renombrar entidades o propiedades sin necesidad funcional real.
- No anadir capas de arquitectura (repositorios, CQRS, etc.) para un cambio de modelo.
- No inventar propiedades para requisitos no confirmados.
- No tocar frontend ni otras capas sin impacto real derivado del cambio de modelo.
- No actualizar migraciones, controladores o documentacion por inercia: solo cuando el cambio lo exige.

## Resultado esperado

- Clase o clases de modelo actualizadas en backend/Models.
- Validaciones declarativas coherentes con el proyecto y con el modelo afectado.
- Si hay impacto, elementos relacionados actualizados de forma minima y consistente.
- Si no hay impacto, solo se actualiza el modelo.
