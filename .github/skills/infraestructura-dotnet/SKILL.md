---
name: infraestructura-dotnet
description: 'Comprueba la infraestructura real de proyectos .NET antes de implementar cambios: detecta .csproj, .sln o .slnx, Program.cs, proyectos de pruebas, referencias NuGet y nivel de compilabilidad del repositorio. Usalo antes de crear controladores, persistencia, tests o cualquier cambio que dependa del esqueleto de solucion.'
argument-hint: 'Indica que parte quieres verificar: proyecto backend, solucion completa, tests, paquetes NuGet, punto de arranque o capacidad real de compilacion.'
---

# Infraestructura .NET

## Objetivo

Comprobar el estado real de la infraestructura .NET del repositorio antes de implementar cambios que dependan de proyectos, solucion, paquetes o arranque de aplicacion.

Si la peticion del usuario incluye crear proyecto, bootstrap o esqueleto .NET, este skill tambien debe crear la infraestructura minima necesaria para poder compilar.

## Cuando usar este skill

- Antes de crear o actualizar controladores, persistencia, servicios o tests en backend .NET.
- Cuando no esta claro si el repositorio ya contiene un proyecto compilable.
- Cuando hay dudas sobre si existen .csproj, .sln, .slnx, Program.cs o referencias NuGet.
- Cuando un skill posterior necesita saber si puede compilar, restaurar paquetes o integrarse en una solucion existente.
- Cuando el usuario pide explicitamente crear el proyecto o levantar infraestructura minima faltante.

## Entradas esperadas

- Instrucciones del proyecto: .github/copilot-instructions.md
- Contexto funcional: README.md
- Estructura real del repositorio
- Peticion del usuario con el alcance de la comprobacion

Modo de ejecucion:
- diagnostico (por defecto)
- diagnostico-y-bootstrap (cuando el usuario pide crear proyecto o infraestructura)

## Artefactos a comprobar

- Archivos .csproj del backend y de tests
- Archivos .sln o .slnx
- Program.cs o punto de arranque equivalente
- Referencias NuGet en PackageReference o Directory.Packages.props
- Carpetas backend, tests u otras capas .NET relevantes
- Evidencia de que la compilacion es posible o no con el estado actual del repo

## Procedimiento

1. Leer README.md y .github/copilot-instructions.md para entender el alcance del proyecto y sus restricciones.
2. Localizar archivos .csproj, .sln y .slnx en el workspace.
3. Verificar si existe Program.cs o bootstrap equivalente para el backend.
4. Revisar si existen proyectos de pruebas y si estan conectados a una solucion o proyecto principal.
5. Revisar referencias NuGet relevantes si existen archivos de proyecto.
6. Clasificar el estado de infraestructura en uno de estos niveles:
   - Solo codigo fuente suelto sin proyecto
   - Proyecto parcial sin solucion o sin arranque
   - Proyecto compilable aislado
   - Solucion compilable con varios proyectos
7. Devolver restricciones claras para los skills siguientes:
   - que se puede implementar ya
   - que no debe asumirse todavia
   - que validacion tecnica es posible en este momento
8. Si el modo es diagnostico-y-bootstrap y falta infraestructura minima, crear bootstrap .NET:
  - crear solucion .sln en la raiz
  - crear proyecto backend .csproj (ASP.NET Core Web API)
  - crear Program.cs minimo con AddControllers y MapControllers
  - crear appsettings.json basico si no existe
  - enlazar el proyecto a la solucion
9. Verificar compilacion minima con dotnet build cuando exista .sln o .csproj.
10. Reportar resultado final con:
  - diagnostico inicial
  - acciones de bootstrap realizadas
  - resultado de build (si aplica)

## Reglas de evaluacion

- No asumir que existe solucion si solo hay archivos .cs.
- No asumir que existe compilacion posible si falta .csproj o punto de arranque.
- No asumir persistencia, EF Core o paquetes NuGet si no hay evidencia en archivos de proyecto.
- No crear infraestructura nueva por defecto: primero describir el estado real y las limitaciones.
- Si el usuario no ha pedido crear bootstrap o solucion, limitarse a comprobar y reportar.
- Si el usuario si ha pedido crear proyecto/infraestructura, ejecutar bootstrap minimo tras el diagnostico sin pedir una segunda confirmacion.
- En bootstrap minimo, no introducir capas extra ni dependencias no solicitadas.

## Criterios de calidad

- El resultado distingue claramente entre codigo existente e infraestructura inexistente.
- El skill deja una conclusion operativa para los siguientes pasos.
- La salida evita que otros skills inventen compilacion, restauracion o arranque no presentes.
- Si se ejecuta bootstrap, la solucion y el backend deben quedar compilables (al menos build exitoso del esqueleto creado).

## Formato de salida sugerido

- Estado de infraestructura: <nivel detectado>
- Proyectos detectados:
  - <ruta o ausencia>
- Solucion detectada:
  - <ruta o ausencia>
- Arranque detectado:
  - <ruta o ausencia>
- Paquetes NuGet:
  - <evidencia o ausencia>
- Validacion tecnica posible ahora:
  - <compilar | validar sintaxis | solo revisar estructura>
- Restricciones para skills siguientes:
  - <restriccion concreta>
- Bootstrap ejecutado:
  - <si/no y acciones>
- Build:
  - <resultado o no aplica>

## Que evitar

- No crear .csproj, .sln, .slnx, Program.cs o referencias NuGet salvo peticion explicita.
- No bloquear implementaciones de codigo si el usuario ha pedido avanzar sin infraestructura completa; en ese caso, solo dejar claras las limitaciones.
- No confundir ausencia de solucion con error del repositorio: puede ser un estado deliberado del ejercicio.
- No sobreescribir archivos existentes de infraestructura sin justificacion y evidencia del cambio requerido.

## Resultado esperado

- Diagnostico claro del nivel real de infraestructura .NET del repo.
- Restricciones concretas para modelo, controladores, persistencia y tests.
- Base objetiva para decidir si los siguientes skills deben compilar, validar sintaxis o limitarse a cambios preparatorios.
- Cuando aplique modo diagnostico-y-bootstrap, esqueleto .NET minimo creado y enlazado a solucion con evidencia de build.