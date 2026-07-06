# Instrucciones de Copilot para una app de tareas con ASP.NET Core 10, API, React, TypeScript y Vite

## Resumen del proyecto

Este proyecto es una aplicación sencilla de lista de tareas construida con:

- ASP.NET Core 10
- ASP.NET Core Web API
- React
- TypeScript
- Vite
- Entity Framework Core
- SQLite

Prioriza código claro y mantenible frente a diseños con abstracciones innecesarias.

## Directrices de arquitectura

- Usa ASP.NET Core Web API para el backend y React con TypeScript y Vite para el frontend. No introduzcas Razor Pages, vistas MVC, Blazor ni patrones alternativos salvo que se pidan expresamente.
- Mantén separadas las responsabilidades de backend y frontend: la API expone datos y operaciones, y React resuelve la interfaz de usuario.
- Centraliza la persistencia en Entity Framework Core usando un único DbContext de aplicación.
- Cuando tenga sentido, organiza por funcionalidad, pero manteniendo una estructura convencional para una API ASP.NET Core y un frontend React.
- Evita añadir capas extra como repositorios o CQRS salvo que el usuario lo pida explícitamente.
- No usar patrones complejos (CQRS, mediator) — el objetivo es claridad didáctica.
- Mantener las clases pequeñas y fáciles de leer en pantalla.

## Directrices de dominio y datos

- Modela una tarea solo con los campos que necesite la funcionalidad.
- Prefiere nombres de propiedades explícitos y legibles como Id, Titulo, EstaCompletada, FechaVencimiento, FechaCreacion y Notas cuando sean necesarios.
- Usa anotaciones de datos o configuración fluida solo cuando aporten valor real. No dupliques reglas de validación en varios sitios.
- Ten en cuenta la compatibilidad con SQLite al elegir tipos y consultas.
- Crea migraciones de Entity Framework Core para los cambios de esquema. No edites la base de datos SQLite manualmente como sustituto de las migraciones.

## Modelo base

- La entidad principal del proyecto debe ser Tarea.
- El nombre de la entidad y de sus miembros debe mantenerse estable salvo que una necesidad funcional real obligue a cambiarlo.
- Las propiedades mínimas obligatorias del modelo son Id, Titulo, EstaCompletada y FechaCreacion.
- Las propiedades opcionales recomendadas son FechaVencimiento y Notas.
- Titulo debe ser obligatorio y tener una longitud razonable validada en el modelo o en la capa de presentación, sin duplicar reglas innecesariamente.
- FechaCreacion debe almacenarse en UTC cuando se use.
- FechaVencimiento debe admitir nulos para permitir tareas sin fecha límite.
- Notas debe ser opcional.

Los nombres canónicos del modelo son:

- Tarea para la entidad principal.
- Id para la clave primaria.
- Titulo para el título de la tarea.
- EstaCompletada para indicar si la tarea está completada.
- FechaCreacion para la fecha de creación.
- FechaVencimiento para la fecha límite opcional.
- Notas para notas opcionales.

Un modelo de referencia puede ser:

```csharp
public class Tarea
{
    public int Id { get; set; }
	public string Titulo { get; set; } = string.Empty;
	public bool EstaCompletada { get; set; }
	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaVencimiento { get; set; }
	public string? Notas { get; set; }
}
```

## Convenciones de nombres

- Usa el estilo con inicial mayúscula por palabra para clases, propiedades, métodos públicos, componentes de React, tipos y constantes.
- Usa el estilo con inicial minúscula y cambios de mayúscula entre palabras para parámetros, variables locales y campos privados.
- Si se usan campos privados inyectados por constructor, nómbralos con prefijo de guion bajo, por ejemplo _dbContext.
- Usa nombres completos y descriptivos. Evita abreviaturas ambiguas como dto, obj, data o tmp salvo cuando sean convenciones ampliamente aceptadas y el contexto sea obvio.
- Nombra las entidades en singular, por ejemplo Tarea, y las colecciones en plural, por ejemplo tareas.
- Nombra los DbSet en plural cuando representen colecciones de entidades, por ejemplo Tareas.
- Nombra los controladores de API con el sufijo Controller cuando se use ese patrón, por ejemplo TareasController.
- Nombra los componentes de React con el mismo nombre que su archivo, por ejemplo TareasPage.tsx, TareaForm.tsx o TareaLista.tsx.
- Nombra los hooks personalizados de React con el prefijo use, por ejemplo useTareas o useFiltroTareas.
- Nombra los tipos e interfaces de TypeScript de forma explícita y orientada al dominio, por ejemplo TareaDto, CrearTareaRequest o TareaFormData.
- Usa nombres booleanos que se lean como una condición, por ejemplo EstaCompletada, TieneFechaVencimiento o PuedeEliminarse.
- Usa nombres de fechas y marcas temporales que indiquen claramente su significado, por ejemplo FechaCreacion, FechaActualizacion o FechaVencimiento.

## Organización de carpetas

- Usa una estructura convencional y separada para backend y frontend para que la navegación del proyecto sea predecible.
- Coloca la API ASP.NET Core en una carpeta propia, con Data, Models, Controllers y otros elementos de backend claramente delimitados.
- Coloca el acceso a datos en una carpeta como Data, incluyendo el DbContext y la configuración relacionada con Entity Framework Core.
- Coloca las entidades de dominio en una carpeta como Models cuando el proyecto necesite separarlas claramente de la lógica de transporte o de la interfaz.
- Coloca el frontend React con TypeScript y Vite en una carpeta propia, con src y subcarpetas orientadas a componentes, páginas, servicios, tipos y estilos.
- Coloca configuraciones transversales y archivos de arranque en ubicaciones claras y convencionales, evitando crear jerarquías profundas sin necesidad.
- Si aparecen modelos de entrada o respuesta específicos de la API, mantenlos cerca de la funcionalidad que los usa o en una carpeta dedicada cuando la reutilización lo justifique.
- Si aparecen componentes reutilizables de React, agrúpalos en una carpeta compartida del frontend en lugar de repartirlos sin criterio.

Una estructura típica puede ser:

```text
/backend
	/Controllers
		TareasController.cs
	/Data
		ApplicationDbContext.cs
		Migrations/
	/Models
		Tarea.cs
	Program.cs
	appsettings.json
/frontend
	/package.json
	/tsconfig.json
	/vite.config.ts
	/src
		/components
			TareaForm.tsx
			TareaLista.tsx
		/pages
			TareasPage.tsx
		/services
			api.ts
		/types
			tarea.ts
		/styles
		App.tsx
		main.tsx
```

## Convenciones de API

- Coloca la lógica HTTP específica de cada recurso en controladores o endpoints claramente agrupados por funcionalidad.
- Mantén las clases de API ligeras y centradas en recibir peticiones, validar entradas, orquestar persistencia y devolver respuestas HTTP correctas.
- Usa modelos de entrada y salida cuando hagan falta, pero no crees capas de transporte innecesarias para operaciones triviales.
- Prefiere la validación estándar del marco de trabajo antes que los parseos manuales.
- Devuelve códigos de estado coherentes y respuestas predecibles para creación, edición, lectura y borrado.

## Convenciones de React y TypeScript

- Mantén los componentes pequeños y centrados en una sola responsabilidad visible.
- Prefiere estado local antes que soluciones globales de estado cuando el flujo siga siendo simple.
- Centraliza las llamadas HTTP en una capa pequeña del frontend, por ejemplo en services, para no duplicar acceso a la API en muchos componentes.
- Mantén formularios accesibles con etiquetas, mensajes de validación y marcado HTML semántico.
- Evita lógica de negocio compleja dentro del JSX; muévela a funciones auxiliares, hooks o servicios cuando haga falta.
- Usa TypeScript en todo el frontend. No añadas archivos JavaScript al frontend salvo que exista una necesidad técnica real.
- Declara tipos e interfaces compartidos en archivos dedicados cuando se reutilicen entre varios componentes o servicios.
- Aprovecha Vite como herramienta de desarrollo y construcción del frontend. No sustituyas Vite por otra herramienta salvo petición expresa.

## Convenciones de Entity Framework Core

- Usa APIs asíncronas para el acceso a datos en el código de aplicación.
- Inyecta el DbContext mediante inyección de dependencias.
- Usa AsNoTracking en consultas de solo lectura cuando sea apropiado.
- Evita patrones de consultas N+1.
- Mantén las consultas simples y legibles antes de intentar optimizarlas.
- Para actualizaciones, carga la entidad, modifica las propiedades necesarias y guarda cambios. No sobreingenierices la lógica de parcheo.

## Convenciones de SQLite

- Asume que el desarrollo local usa una base de datos SQLite basada en archivo.
- Mantén la configuración de la cadena de conexión simple y convencional.
- Ten cuidado con las limitaciones específicas del proveedor al escribir migraciones y consultas.
- No borres ni recrees la base de datos para corregir desajustes de esquema salvo que el usuario pida ese enfoque explícitamente.

## Estilo de código

- Sigue las convenciones estándar de C# y el formato existente del proyecto.
- Prefiere clases y métodos pequeños y enfocados.
- Usa nombres claros en lugar de abreviaturas.
- Evita generalizaciones especulativas.
- Añade comentarios solo cuando la intención no sea obvia en el código.
- Usa correctamente los tipos por referencia anulables.

## Buenas prácticas para mensajes de commit

- Usa el skill de proyecto en .github/skills/mensajes-de-commit cuando haya que redactar o revisar mensajes de commit.
- Los mensajes de commit de este proyecto deben ir siempre en castellano.

## Validación y manejo de errores

- Usa primero la validación integrada del marco de trabajo.
- Devuelve errores de validación claros desde la API y muéstralos en el frontend React de forma comprensible para la persona usuaria.
- Devuelve NotFound solo cuando sea el comportamiento HTTP correcto.
- No ocultes excepciones. Manéjalas solo cuando el código pueda responder de forma útil.

## Expectativas de testing

- Todo cambio no trivial debe incluir pruebas.
- Prefiere pruebas unitarias para la lógica de aplicación, componentes de React y transformaciones de datos cuando tengan comportamiento relevante.
- Prefiere pruebas unitarias para la lógica de aplicación, componentes de React, hooks y transformaciones de datos cuando tengan comportamiento relevante.
- Añade pruebas de integración cuando el cambio dependa de comportamiento HTTP, enrutamiento, serialización, validación o base de datos.
- Mantén las pruebas enfocadas en el comportamiento observable.
- No añadas pruebas de relleno.

## Qué evitar

- No introduzcas Razor Pages, vistas MVC ni Blazor salvo petición expresa.
- No introduzcas una capa de repositorio sin una necesidad clara.
- No dupliques lógica de negocio entre la API y el frontend.
- No añadas JavaScript plano al frontend cuando el proyecto ya usa TypeScript.
- No uses acceso síncrono a base de datos.
- No añadas gestión de estado global compleja en React sin una necesidad real.
- No hagas refactors amplios fuera del alcance de la tarea solicitada.

## Preferencias típicas de implementación

- Para flujos de creación y edición, usa endpoints simples en la API y formularios de React con manejo claro de estado y validación.
- Para vistas de listado, mantén el filtrado y la ordenación sencillos.
- Para marcar tareas como completadas, prefiere una operación HTTP simple y explícita frente a soluciones cargadas de cliente.
- Para borrado, prefiere flujos explícitos de confirmación cuando la interfaz ya los soporte.
- Cuando introduzcas marcas temporales, almacénalas en UTC.

## Habilidades

## Agentes

## Guía de tareas para Copilot

Al generar código para este proyecto:

- Conserva la separación existente entre API y frontend React con TypeScript y Vite.
- Haz el cambio más pequeño que resuelva completamente la petición.
- Actualiza o añade migraciones de EF Core cuando cambie el modelo.
- Actualiza las pruebas junto con la implementación.
- Si una petición introduce complejidad arquitectónica innecesaria, propone primero la opción más simple.