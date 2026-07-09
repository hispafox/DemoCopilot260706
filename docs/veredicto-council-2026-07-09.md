# Veredicto del consejo - revisión del hallazgo de auditoría

Fecha: 2026-07-09

## Resumen ejecutivo

El hallazgo relativo a la supuesta duplicación de la validación de entidades relacionadas entre la creación y la actualización de usuarios no es correcto en el estado actual del repositorio. La revisión del código muestra que la validación de referencias a departamento, sede y población ya está centralizada en la capa de servicio y se aplica de forma consistente en ambos flujos operativos.

## Contexto del hallazgo

El informe de auditoría original identificó como problema que existía un bloque repetido de validación en la ruta de creación y en la ruta de actualización para los campos `DepartamentoId`, `SedeId` y `PoblacionId`. Ese diagnóstico se basaba en una lectura superficial del flujo, pero la implementación real ya ha evolucionado hacia un modelo más ordenado.

## Evidencia técnica

Al revisar el código se observa que:

- El servicio de usuarios concentra la lógica de validación en el método `ValidarReferenciasAsync` dentro de [backend/Services/UsuariosService.cs](../backend/Services/UsuariosService.cs).
- Tanto `CrearAsync` como `ActualizarAsync` llaman a ese método antes de persistir los cambios.
- El controlador [backend/Controllers/UsuariosController.cs](../backend/Controllers/UsuariosController.cs) no contiene la regla de negocio de comprobar la existencia de entidades relacionadas; su papel es únicamente coordinar la petición HTTP y traducir los resultados a respuestas adecuadas.
- La validación de entrada básica sigue siendo responsabilidad de los contratos de petición, como se ve en [backend/Contracts/ApiContracts.cs](../backend/Contracts/ApiContracts.cs), pero la comprobación de existencia de las entidades referenciadas ya está unificada en el servicio.

## Valoración del hallazgo

En este punto, el problema no es una duplicación funcional activa, sino más bien una interpretación equivocada del diseño. La arquitectura actual evita que la validación de referencias se repita de forma separada en cada endpoint y, por tanto, reduce el riesgo de divergencia futura.

## Respuestas de los asesores

### El Contrario

La preocupación inicial tiene sentido en teoría, pero la implementación actual no muestra una duplicación real del problema. La validación no está dispersa entre create y update, sino centralizada en un único punto del servicio. Si hubiera una regresión futura, el riesgo no sería que el mismo bloque se repita en dos lugares, sino que alguien lo desplace otra vez a la capa HTTP. El mayor fallo del hallazgo es que lo presenta como defecto activo cuando lo que existe es una mejora de diseño ya incorporada.

### El de Primeros Principios

La pregunta correcta no es si hay dos bloques iguales, sino si la regla de negocio está en el lugar adecuado. En este caso, la respuesta es sí: la comprobación de existencia de departamentos, sedes y poblaciones está en la capa de servicio, donde corresponde. El control del flujo HTTP no debería decidir si una entidad relacionada existe; eso es una decisión de negocio y de persistencia. El hallazgo original mezcla una observación de forma con un problema de arquitectura real.

### El Expansivo

Este resultado es un ejemplo de cómo un informe puede quedarse corto al mirar solo la superficie de un patrón. Lo importante aquí es que el sistema ya ha evolucionado hacia una estructura más robusta: una única ruta de validación para crear y actualizar usuarios. Eso mejora la mantenibilidad y deja espacio para escalar la lógica con más reglas en el futuro. El valor real no está en corregir un supuesto duplicado, sino en preservar esta estructura y evitar que vuelva a dispersarse.

### El de Fuera

Desde una mirada externa, lo que más importa es que el problema se describa con claridad. Si un auditor o un desarrollador lee el informe, debería entender que la lógica de validación está centralizada y que no hay un fallo operativo visible. El hallazgo original parece haber tomado una impresión visual del código y la ha convertido en un problema de diseño, cuando en realidad la implementación actual ya resuelve esa preocupación.

### El Ejecutor

El camino más pragmático es mantener el diseño actual y corregir el informe. No hay necesidad de reescribir la lógica ni de introducir cambios de arquitectura para una situación que ya está resuelta. La tarea útil es documentar que la validación está en el servicio, que el controlador solo orquesta la petición y que la solución es estable. Si algo se va a cambiar, debería ser la redacción del informe, no el código.

## Conclusión

No corresponde mantener este hallazgo como un defecto vigente. Lo más correcto es corregir la lectura del informe y dejar constancia de que la validación de referencias ya está unificada en la capa de servicio, con una única ruta de negocio para crear y actualizar usuarios.

## Recomendación

Actualizar el informe de auditoría para reflejar el estado real del código, reconociendo que la validación de entidades relacionadas está centralizada y que la observación original debe reformularse como una nota de contexto histórico o de seguimiento, no como un problema activo.

## Nota de seguimiento

Si en futuras iteraciones se introdujera lógica adicional de validación específica por entidad o por operación, conviene revisar si esa lógica sigue siendo compartida y no se desplaza de nuevo a los controladores o a múltiples rutas paralelas.
