---
name: servicios-aplicacion
description: 'Crea o actualiza servicios de aplicacion en backend y su registro en Program.cs, manteniendo separacion entre controlador, orquestacion de servicio y logica de negocio.'
argument-hint: 'Indica el caso de uso, si quieres crear o actualizar servicios, y como deben coordinarse con controladores y capa de logica de negocio.'
---

# Servicios de Aplicacion

## Objetivo

Verificar si existen los servicios necesarios para la funcionalidad solicitada y, cuando falten, crearlos con el alcance minimo necesario e inyectarlos en Program.cs.

Este skill se centra en la capa de servicio, no en crear endpoints HTTP completos ni en absorber la logica de negocio.

## Cuando usar este skill

- Cuando un controlador necesita delegar logica de orquestacion y no existe un servicio adecuado.
- Cuando hay que crear un servicio nuevo para encapsular casos de uso.
- Cuando hay que registrar servicios en DI dentro de Program.cs.
- Cuando se quiere evitar que el controlador hable directo con persistencia o reglas de negocio complejas.
- Cuando hay capa de logica de negocio y el servicio debe actuar como orquestador que la consume.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Analisis/diseno tecnico: documentacion/analisis-diseno.md o documentacion/analisis-diseño.md
- Guia comun de capas: .github/skills/guia-estilo-capas.md
- Estado real del backend: backend/Services, backend/Controllers, backend/Models, backend/Program.cs
- Peticion del usuario con alcance del caso de uso

## Ubicaciones habituales

- backend/Services
- backend/Program.cs
- backend/Controllers (solo para cableado minimo hacia el servicio)

Usar convencion existente del repo si ya hay una carpeta o patron de servicios.

## Procedimiento

1. Leer README.md y .github/copilot-instructions.md para respetar alcance y convenciones.
2. Revisar analisis/diseno para identificar casos de uso y responsabilidades de servicio.
3. Inspeccionar backend/Services y backend/Controllers para detectar servicios existentes y evitar duplicados.
4. Si existe capa de logica de negocio (por ejemplo backend/Business o backend/Domain), verificar su uso desde servicios.
5. Determinar si el servicio ya existe:
   - Si existe y cubre el caso: reutilizar y ajustar solo lo minimo.
   - Si no existe: crear interfaz e implementacion minimas con nombres del dominio.
6. Ubicar la logica de orquestacion en el servicio y delegar reglas de negocio en la capa de logica de negocio cuando exista.
7. Registrar el servicio en DI en Program.cs con el lifetime adecuado (por defecto Scoped).
8. Si el controlador afectado aun no usa servicio, ajustar su constructor y llamadas minimas.
9. Si cambia comportamiento observable, alinear analisis/diseno tecnico.
10. Validar que no se introducen capas innecesarias ni sobreingenieria.

## Reglas de implementacion

Reglas de arquitectura:

- Mantener separacion de responsabilidades: controlador (HTTP), servicio (orquestacion), logica de negocio (reglas), dominio/persistencia (modelo y datos).
- No mover logica de negocio compleja al controlador.
- No concentrar reglas de negocio en servicios cuando exista capa de logica de negocio.
- No introducir repositorios, CQRS o mediadores salvo peticion explicita.

Reglas de DI:

- Registrar en Program.cs todos los servicios nuevos requeridos.
- Evitar registros duplicados para el mismo contrato.
- Usar AddScoped como valor por defecto salvo justificacion tecnica.

Reglas de alcance:

- Aplicar el cambio minimo necesario para el caso pedido.
- No crear servicios especulativos para funcionalidades no pedidas.
- No reestructurar carpetas fuera del alcance.

## Gates recomendados

Gate 1 (existencia):
- Se comprobo si el servicio ya existia antes de crear uno nuevo.

Gate 2 (inyeccion):
- Program.cs contiene el registro DI del servicio requerido.

Gate 3 (consumo):
- El controlador afectado usa el servicio en lugar de acoplarse a logica que no le corresponde.

Gate 4 (frontera con negocio):
- El servicio invoca la capa de logica de negocio para reglas y decisiones de dominio cuando aplique.

## Criterios de calidad

- Servicio creado o actualizado con responsabilidad clara.
- Frontera explicita entre orquestacion (servicio) y reglas (logica de negocio).
- Program.cs actualizado con DI coherente y sin duplicados.
- Controlador mas ligero y enfocado en HTTP.
- Cambio pequeno, legible y trazable a la peticion.

## Que evitar

- No crear servicios si el caso ya esta cubierto por uno existente.
- No dejar servicios sin registrar en Program.cs.
- No duplicar reglas de negocio en controlador, servicio y capa de negocio.
- No mezclar en el mismo cambio refactors amplios no solicitados.

## Resultado esperado

- Servicio existente verificado o servicio nuevo creado en backend/Services.
- Servicios alineados para orquestar e invocar logica de negocio cuando aplique.
- Registro de inyeccion actualizado en backend/Program.cs.
- Controlador afectado cableado al servicio cuando aplique.
- Documentacion tecnica alineada si hay impacto observable.
