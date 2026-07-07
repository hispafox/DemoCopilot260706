# Guia de estilo comun para skills de capas

## Objetivo

Alinear redaccion y decisiones de arquitectura entre skills de modelo, base de datos, logica de negocio, servicios y controladores.

## Principios de redaccion

- Usar alcance explicito: README, analisis/diseno e instrucciones del proyecto.
- Mantener lenguaje operativo, corto y verificable.
- Evitar terminos ambiguos y promesas fuera de alcance.
- Evitar acoplar la definicion del skill a nombres concretos de recurso.

## Frontera por capas

- Modelo: estructura de entidades, propiedades y validaciones declarativas.
- Base de datos: DbContext, mapeo Fluent API, registro de persistencia y migraciones.
- Logica de negocio: reglas, decisiones y transiciones de dominio.
- Servicios: orquestacion de casos de uso y coordinacion de dependencias.
- Controladores: entrada/salida HTTP, validacion de contrato y codigos de respuesta.

## Orden recomendado de ejecucion

1. infraestructura-dotnet
2. analisis-diseno
3. validador-analisis-prd (opcional recomendado)
4. modelo-aplicacion
5. base-datos-aplicacion (si hay impacto de persistencia)
6. logica-negocio (si hay reglas de dominio)
7. dtos-aplicacion (si hay contratos HTTP)
8. servicios-aplicacion (si hay orquestacion)
9. controladores-api (si hay endpoints)

## Regla de no duplicacion

- Una regla de negocio debe vivir en logica de negocio, no repetirse en servicios y controladores.
- Reglas de mapeo/persistencia deben vivir en base de datos (Fluent API), no dispersarse en capas HTTP.
- La capa de servicios no reemplaza la capa de logica de negocio.

## Checklist minimo por skill

- Se leyo README, analisis/diseno e instrucciones.
- El cambio es minimo y trazable a la peticion.
- Se respeta separacion de responsabilidades por capa.
- Se evitan nombres fijos de recurso en la definicion del skill.
