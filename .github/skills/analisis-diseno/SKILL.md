---
name: analisis-diseno
description: 'Genera o actualiza el documento de análisis y diseño técnico del proyecto en documentacion/analisis-diseño.md. Úsalo cuando quieras crear desde cero el análisis técnico, actualizarlo tras un cambio de diseño, o sincronizarlo con el estado real del código.'
argument-hint: 'Indica si quieres crear desde cero o actualizar el documento existente, y qué secciones han cambiado o deben revisarse.'
---

# Análisis y Diseño Técnico

## Objetivo

Generar o actualizar `documentacion/analisis-diseño.md` con el análisis técnico completo y actualizado del proyecto, partiendo del estado real del código y de los documentos de referencia del repositorio.

## Cuándo usar este skill

- Al iniciar el proyecto, para crear el documento de análisis desde cero.
- Cuando se añade una entidad, endpoint, servicio o capa nueva al backend.
- Cuando cambia el modelo de datos y hay que reflejar las nuevas propiedades o relaciones.
- Cuando se toma una decisión de diseño relevante (cambio de patrón, nueva dependencia, estrategia de persistencia).
- Cuando el código y el documento han divergido y hay que resincronizarlos.
- Antes de validar con el skill `validador-analisis-prd`.

## Entradas esperadas

- Estado actual del código fuente (carpetas `backend/`, `frontend/` si existe).
- PRD del proyecto: `documentacion/PRD.md` (si existe).
- Documento existente: `documentacion/analisis-diseño.md` (si ya existe, se actualiza en lugar de recrearse).
- Instrucciones del proyecto: `.github/copilot-instructions.md`.
- Indicación del usuario: crear desde cero o actualizar sección concreta.

## Procedimiento

1. Leer `.github/copilot-instructions.md` para respetar convenciones de nombres, estructura de carpetas y reglas del proyecto.
2. Leer `documentacion/PRD.md` si existe, para entender el alcance funcional comprometido.
3. Explorar el código fuente real:
   - Entidades en `backend/Models/`.
   - DbContext en `backend/Data/`.
   - Controladores en `backend/Controllers/`.
   - Servicios en `backend/Services/` si existen.
   - Componentes y tipos de frontend en `frontend/src/` si existen.
4. Si el documento existe, leerlo para identificar secciones desactualizadas antes de modificar.
5. Generar o actualizar el documento siguiendo la estructura estándar definida abajo.
6. Guardar el resultado en `documentacion/analisis-diseño.md` (sobrescribir).

## Estructura estándar del documento

El documento debe contener siempre estas secciones, en este orden:

```
# Análisis y Diseño — <Nombre del proyecto>

## 1. Objetivo del proyecto
## 2. Stack tecnológico
## 3. Arquitectura de capas
## 4. Modelo de datos
## 5. Endpoints API REST
## 6. Decisiones de diseño
## 7. Pendientes / Preguntas abiertas
```

### Sección 1 — Objetivo del proyecto

Párrafo breve (2-4 líneas) que describe qué hace la aplicación, para quién y con qué propósito didáctico o funcional.

### Sección 2 — Stack tecnológico

Tabla con columnas: Tecnología | Versión | Rol.
Incluir solo las tecnologías realmente usadas en el proyecto (no especulativas).

### Sección 3 — Arquitectura de capas

Tabla con columnas: Capa | Carpeta | Responsabilidad.
Árbol de carpetas del proyecto con los archivos clave.
Reglas de diseño aplicadas (inyección de dependencias, async/await, separación de responsabilidades).

### Sección 4 — Modelo de datos

Por cada entidad del dominio:
- Tabla con columnas: Campo | Tipo | Descripción.
- Bloque de código C# con la clase real.

Por cada enum relevante:
- Bloque de código C# con el enum.

### Sección 5 — Endpoints API REST

Por cada recurso (agrupado por controlador):
- Tabla con columnas: Verbo | Ruta | Descripción | Respuesta OK | Error.

### Sección 6 — Decisiones de diseño

Lista con viñetas. Cada entrada con formato:
- **Nombre de la decisión**: explicación del porqué.

Incluir solo decisiones reales tomadas, no especulativas.

### Sección 7 — Pendientes / Preguntas abiertas

Lista de elementos pendientes, con formato:
- **Nombre**: descripción y opciones si las hay.

## Convenciones obligatorias

- Respetar los nombres canónicos del proyecto definidos en `.github/copilot-instructions.md`: `Tarea`, `Id`, `Titulo`, `EstaCompletada`, `FechaCreacion`, `FechaVencimiento`, `Notas`.
- Si el código real usa nombres distintos a los canónicos, documentar los nombres reales del código y añadir una nota de pendiente en la sección 7.
- Tablas en Markdown estándar (pipes `|`).
- Bloques de código con el lenguaje indicado (` ```csharp `, ` ```text `).
- Redactar en castellano.
- No inventar funcionalidades que no existan en el código.
- No omitir campos ni endpoints existentes.

## Criterios de calidad

- El documento refleja el estado real del código, no el estado deseado.
- Cada entidad del modelo tiene su tabla y su bloque de código.
- Cada controlador tiene su tabla de endpoints.
- Las decisiones de diseño explican el porqué, no solo el qué.
- Los pendientes son concretos y accionables.
- El documento puede ser leído por alguien nuevo al proyecto sin necesitar abrir el código.

## Qué evitar

- No documentar capas o servicios que no existan todavía en el código.
- No añadir endpoints hipotéticos.
- No mezclar decisiones tomadas con deseos futuros en la sección 6 (los deseos van en la sección 7).
- No duplicar información entre secciones.
- No usar nombres en inglés si el proyecto usa castellano y viceversa.
