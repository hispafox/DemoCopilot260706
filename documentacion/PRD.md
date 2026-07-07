# PRD - Lista de Tareas (Demo Curso GitHub Copilot)

## 1. Resumen ejecutivo

Este PRD define el alcance de producto para una aplicacion web de gestion de tareas personales orientada a uso didactico en un curso de GitHub Copilot.

La solucion debe permitir crear, listar, actualizar y eliminar tareas, e incluir dos capacidades diferenciales para el curso:
- plantillas reutilizables
- tareas repetitivas con generacion automatica de la siguiente ocurrencia al completar
- priorizacion funcional de tareas (Baja, Normal, Alta, Urgente)

Este documento define el que y el por que del producto.
El como tecnico se detalla en el anexo de analisis y diseno.

## 2. Problema a resolver

Las personas usuarias necesitan una forma simple de organizar tareas recurrentes y no recurrentes sin friccion. En contexto formativo, ademas se necesita una base funcional clara para demostrar buenas practicas de desarrollo asistido por Copilot.

Problemas concretos:
- registrar tareas rapidamente
- evitar recrear manualmente tareas similares (plantillas)
- no perder continuidad en tareas periodicas (recurrencia)

## 3. Objetivos

### 3.1 Objetivos de producto
- Permitir gestion completa de tareas (CRUD) de forma simple.
- Permitir crear tareas desde plantillas predefinidas.
- Permitir completar tareas repetitivas y generar automaticamente la siguiente ocurrencia.

### 3.2 Objetivos de negocio/formacion
- Servir como demo didactica legible y mantenible para el curso.
- Mostrar decisiones de diseno sencillas y justificadas, evitando sobrearquitectura.

## 4. Personas usuarias objetivo

### 4.1 Persona principal
- Alumno/a del curso de Copilot.
- Necesita entender flujo end-to-end (API + datos + logica) con codigo claro.

### 4.2 Persona secundaria
- Instructor/a o revisor tecnico.
- Necesita una base estable para explicar decisiones de producto y de implementacion.

## 5. Alcance

### 5.1 Incluido en v1
- Gestion de tareas: alta, consulta, edicion y borrado.
- Estado completada/no completada.
- Priorizacion de tareas con niveles Baja, Normal, Alta y Urgente.
- Plantillas: CRUD e instanciacion de tarea desde plantilla.
- Recurrencia basica: diaria, semanal y mensual.
- Endpoint explicito para completar tareas con efecto secundario de generacion de siguiente ocurrencia.

### 5.2 Fuera de alcance en v1
- Autenticacion/autorizacion.
- Paginacion avanzada.
- Busqueda y filtros complejos.
- Notificaciones.
- Generacion automatica por jobs en background sin accion de usuario.
- Sincronizacion multiusuario.

## 6. Requisitos funcionales

### RF-01 Gestion de tareas
El sistema debe permitir crear, listar, obtener por id, actualizar y eliminar tareas.

Criterios de aceptacion:
- Crear tarea valida devuelve confirmacion de creacion.
- Consultar tarea inexistente devuelve no encontrado.
- Eliminar tarea existente elimina el recurso.

### RF-02 Completar tarea
El sistema debe permitir marcar una tarea como completada mediante una accion explicita.

Criterios de aceptacion:
- Al completar una tarea no repetitiva, su estado queda completado.
- Al completar una tarea repetitiva, se genera la siguiente ocurrencia con la periodicidad configurada.

### RF-03 Gestion de plantillas
El sistema debe permitir crear, listar, actualizar y eliminar plantillas de tarea.

Criterios de aceptacion:
- Una plantilla valida puede guardarse y consultarse.
- Eliminar plantilla inexistente responde no encontrado.

### RF-04 Instanciar desde plantilla
El sistema debe permitir crear una tarea nueva desde una plantilla existente.

Criterios de aceptacion:
- Instanciar una plantilla existente crea una nueva tarea con valores derivados.
- Instanciar una plantilla inexistente responde no encontrado.

### RF-05 Prioridades de tarea
El sistema debe permitir asignar y actualizar la prioridad de una tarea.

Criterios de aceptacion:
- Una tarea admite unicamente los valores de prioridad: Baja, Normal, Alta y Urgente.
- Si no se informa prioridad en una creacion valida, se aplica el valor por defecto Normal.
- La prioridad se conserva en lectura, actualizacion y operaciones derivadas de recurrencia.

## 7. Reglas de negocio

- RN-01: El titulo de una tarea es obligatorio. No se permite crear ni actualizar tareas con titulo vacio tras aplicar trim de espacios en extremos.
- RN-02: Una tarea nueva se crea en estado no completada por defecto, salvo indicacion explicita valida en el flujo de negocio.
- RN-03: Solo las tareas marcadas como repetitivas pueden tener tipo de recurrencia.
- RN-04: Si una tarea no es repetitiva, su recurrencia y su proxima fecha deben permanecer sin valor.
- RN-05: Al completar una tarea no repetitiva, no se genera ninguna tarea adicional.
- RN-06: Al completar una tarea repetitiva, se debe generar exactamente una siguiente ocurrencia segun su tipo de recurrencia (diaria, semanal o mensual).
- RN-07: La accion de completar es idempotente a nivel funcional: no debe generar ocurrencias duplicadas por una misma finalizacion efectiva.
- RN-08: Una plantilla debe tener titulo obligatorio para poder guardarse.
- RN-09: Instanciar una plantilla genera una nueva tarea independiente; cambios posteriores en la plantilla no alteran tareas ya creadas.
- RN-10: La asociacion entre tarea y plantilla es opcional; una tarea puede existir sin plantilla origen.
- RN-11: Al eliminar una plantilla, las tareas ya creadas desde ella deben conservarse.
- RN-12: La recurrencia solo se materializa al completar la tarea; no existe generacion automatica por tiempo en v1.
- RN-13: La prioridad valida de una tarea solo puede ser una de estas opciones: Baja, Normal, Alta o Urgente.
- RN-14: Si no se especifica prioridad al crear una tarea, el sistema asigna Normal por defecto.

## 8. Requisitos no funcionales

- Claridad de codigo sobre sofisticacion arquitectonica.
- Consistencia de respuestas HTTP y validaciones basicas.
- Persistencia local en SQLite para facilitar ejecucion sin infraestructura externa.
- Cobertura de pruebas en cambios no triviales.
- Mantenibilidad: separacion clara de responsabilidades (controladores, servicios, datos, modelos).

## 9. Metricas de exito (KPIs)

KPIs de uso/funcionalidad para validar v1:
- Tasa de exito CRUD >= 95% en pruebas de aceptacion.
- Tasa de exito al completar tareas repetitivas >= 95% en pruebas de aceptacion.
- Tasa de errores 5xx en flujos principales < 1% en entorno de demo.

KPIs didacticos:
- Flujo principal explicable de extremo a extremo en <= 20 minutos de clase.
- Tiempo de onboarding tecnico para entender estructura base <= 30 minutos.

## 10. Supuestos, dependencias y riesgos

Supuestos:
- Uso principal en entorno local de curso.
- Volumen de datos bajo/medio.

Dependencias:
- API backend operativa.
- Persistencia SQLite.
- Conjunto de pruebas automatizadas para regresion.

Riesgos:
- Ambiguedad funcional entre documento tecnico y alcance de producto.
- Crecimiento de alcance (scope creep) por peticiones no priorizadas.
- Diferencias entre comportamiento esperado y definicion de recurrencia en casos borde.

Mitigaciones:
- Mantener este PRD como fuente de verdad de alcance.
- Registrar cambios de alcance por version.
- Añadir pruebas de aceptacion para recurrencia.

## 11. Hitos propuestos

- Hito 1: CRUD de tareas operativo.
- Hito 2: CRUD de plantillas operativo.
- Hito 3: completado con recurrencia y pruebas de aceptacion.
- Hito 4: estabilizacion y documentacion final.

## 12. Criterio de salida de v1

Se considera v1 completada cuando:
- Todos los requisitos funcionales RF-01 a RF-05 cumplen sus criterios de aceptacion.
- No hay defectos criticos abiertos en flujos principales.
- La demo puede ejecutarse y explicarse de forma consistente durante una sesion de curso.

## 13. Anexos

- Anexo tecnico de referencia: documentacion/analisis-diseño.md
