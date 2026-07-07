---
name: logica-negocio
description: 'Crea o actualiza la capa de logica de negocio segun README, analisis/diseno e instrucciones, derivando reglas desde modelo y operaciones sin fijar nombres concretos de recurso en el skill.'
argument-hint: 'Indica el cambio funcional esperado, si quieres crear o actualizar reglas de negocio y que operaciones del analisis deben quedar cubiertas.'
---

# Logica de Negocio

## Objetivo

Crear o actualizar la capa de logica de negocio con reglas y decisiones del dominio, derivadas del modelo y de las operaciones documentadas en el analisis/diseno.

Este skill evita acoplarse a nombres concretos de recurso dentro de su definicion.

## Cuando usar este skill

- Cuando una regla de negocio no debe vivir en controladores ni en la capa de servicios.
- Cuando hay decisiones de dominio reutilizables por varias operaciones.
- Cuando se incorporan reglas que dependen de estados, fechas, transiciones o validaciones del modelo.
- Cuando se requiere alinear comportamiento con las operaciones definidas en el analisis/diseno.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Analisis/diseno tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Guia comun de capas: .github/skills/guia-estilo-capas.md
- Modelo actual: backend/Models
- Capa de servicios actual: backend/Services
- Capa API actual: backend/Controllers
- Peticion del usuario con el alcance funcional

## Ubicaciones habituales

- backend/Business o backend/Domain (segun convencion existente)
- backend/Services (solo para invocacion y orquestacion)
- backend/Controllers (solo cableado minimo cuando aplique)

Si no existe carpeta para logica de negocio, crear una opcion convencional minima y consistente con el repo.

## Procedimiento

1. Leer README.md y .github/copilot-instructions.md para respetar alcance y convenciones.
2. Leer el analisis/diseno para identificar operaciones funcionales y reglas implicitas o explicitas.
3. Leer el modelo en backend/Models para detectar invariantes, estados y relaciones relevantes.
4. Derivar casos de negocio desde modelo + operaciones (no desde nombres fijos de recurso).
5. Verificar si ya existe logica equivalente para evitar duplicados.
6. Si no existe, crear componentes de logica de negocio pequenos y enfocados.
7. Mantener la capa de servicios como orquestacion: invoca logica de negocio, no la reemplaza.
8. Ajustar controladores solo si hace falta para cableado minimo.
9. Si el comportamiento observable cambia, alinear analisis/diseno tecnico.

## Reglas de implementacion

Reglas de separacion de capas:

- Modelo: define estructura y validaciones declarativas del dominio.
- Logica de negocio: concentra reglas, decisiones y transiciones del dominio.
- Servicios: orquestan flujos de aplicacion e integraciones, delegando reglas de negocio.
- Controladores: gestionan HTTP y delegan al servicio.

Reglas de alcance:

- Aplicar el cambio minimo necesario para cubrir las operaciones del analisis/diseno impactadas.
- No crear abstracciones especulativas ni reglas no pedidas.
- No acoplar el skill a nombres concretos de entidades, endpoints o recursos.

Reglas de consistencia:

- Las reglas deben poder rastrearse al modelo y a operaciones documentadas.
- Evitar duplicar reglas entre logica de negocio, servicios y controladores.
- Priorizar componentes pequenos, legibles y testeables.

## Gates recomendados

Gate 1 (trazabilidad):
- Cada regla nueva o modificada se puede mapear a modelo + operacion del analisis/diseno.

Gate 2 (separacion):
- Las reglas de negocio no quedan embebidas en controladores ni en servicios de forma principal.

Gate 3 (consumo):
- La capa de servicios invoca la capa de logica de negocio para los casos impactados.

## Criterios de calidad

- Capa de logica de negocio creada o actualizada con responsabilidad clara.
- Servicios ajustados solo para orquestar e invocar reglas.
- Controladores ligeros y enfocados en contrato HTTP.
- Cambio pequeno, legible y trazable a la peticion.

## Que evitar

- No fijar nombres concretos de recurso dentro del skill.
- No mover logica de negocio compleja a controladores.
- No duplicar la misma regla en varias capas.
- No introducir capas extra no solicitadas.

## Resultado esperado

- Capa de logica de negocio implementada o ajustada segun modelo + operaciones documentadas.
- Capa de servicios alineada para delegar reglas de negocio.
- Capa API mantenida ligera cuando aplique.
- Documentacion tecnica alineada si hay impacto observable.
