# Plan: Soporte ISO para población de usuario

**Issue:** pendiente
**Rama:** pendiente

## 1. Resumen
Se prioriza la parte "ISO" como mejora incremental de localización en usuarios. Dado que el término es ambiguo en el repositorio actual, la interpretación operativa mínima para esta iteración es: incorporar código de país ISO 3166-1 alpha-2 en la población asociada al usuario.

Este alcance permite avanzar sin bloquear el flujo: no crea una capa nueva de catálogos de países, solo añade el dato mínimo verificable para clasificar poblaciones por estándar ISO.

## 2. Requisitos
- Interpretación operativa mínima de "ISO": código de país ISO 3166-1 alpha-2 (ejemplos: ES, FR, PT).
- Mantener arquitectura existente por capas: Models, Contracts, Services, Controllers, Data.
- Añadir el código ISO en la entidad Poblacion y exponerlo en DTOs y endpoints de poblaciones.
- Propagar el código ISO en lectura de usuarios a través de los campos de población ya relacionados.
- Validar formato del código ISO: obligatorio, longitud exacta 2, normalizado en mayúsculas y trim.
- Mantener EF Core con SQLite y migración de esquema; no editar la base manualmente.
- Mantener APIs asíncronas y AsNoTracking en lecturas.

## 3. Cambios en el modelo
- Modificar backend/Models/Poblacion.cs:
  - Nuevo campo CodigoIsoPais (string, obligatorio, longitud 2).
- Mantener Usuario sin nuevos campos físicos (ya referencia PoblacionId).
- Modificar backend/Data/ApplicationDbContext.cs:
  - Configurar CodigoIsoPais como requerido y HasMaxLength(2).
  - Índice no único sugerido para consultas por país (opcional según volumen).
- Crear migración EF Core:
  - Añadir columna CodigoIsoPais en tabla Poblaciones.
  - Backfill para datos existentes con valor temporal "ES" como default técnico inicial de esta iteración (ajustable en datos reales más adelante).

## 4. DTOs
- Modificar backend/Contracts/ApiContracts.cs:
  - PoblacionDto: añadir CodigoIsoPais.
  - CrearActualizarPoblacionRequest: añadir CodigoIsoPais con validaciones declarativas y normalización (trim + uppercase).
  - UsuarioDto: añadir PoblacionCodigoIsoPais para evitar llamadas extra en frontend.

Campos propuestos:
- PoblacionDto:
  - Id
  - Nombre
  - CodigoIsoPais
- CrearActualizarPoblacionRequest:
  - Nombre
  - CodigoIsoPais
- UsuarioDto:
  - PoblacionId
  - PoblacionNombre
  - PoblacionCodigoIsoPais

## 5. Endpoints
Endpoints modificados y ejemplos requeridos en backend/Backend.Api.http:

- GET /api/poblaciones
  - OK: devuelve lista con Nombre y CodigoIsoPais.
  - Error: no aplica en flujo normal (mantener ejemplo de consulta de id inexistente en GET por id).
- GET /api/poblaciones/{id}
  - OK: devuelve PoblacionDto con CodigoIsoPais.
  - Error: 404 Not Found para id inexistente.
- POST /api/poblaciones
  - OK: 201 Created con CodigoIsoPais válido (por ejemplo "ES").
  - Error: 400 ValidationProblem cuando CodigoIsoPais sea vacío, longitud distinta de 2 o espacios.
- PUT /api/poblaciones/{id}
  - OK: 200 OK actualizando Nombre/CodigoIsoPais.
  - Error: 404 si no existe; 400 si CodigoIsoPais inválido.
- GET /api/usuarios
  - OK: incluye PoblacionCodigoIsoPais en cada UsuarioDto.
  - Error: no aplica directamente.
- GET /api/usuarios/{id}
  - OK: incluye PoblacionCodigoIsoPais.
  - Error: 404 Not Found para id inexistente.

Ejemplos mínimos a reflejar en backend/Backend.Api.http:
- POST /api/poblaciones con { "nombre": "Barcelona", "codigoIsoPais": "ES" } (OK).
- POST /api/poblaciones con { "nombre": "Barcelona", "codigoIsoPais": "E" } (400).
- GET /api/poblaciones/999999 (404).
- GET /api/usuarios y GET /api/usuarios/{id} verificando presencia de poblacionCodigoIsoPais.

## 6. Lógica de negocio
- Regla ISO mínima:
  - CodigoIsoPais siempre informado, con 2 caracteres útiles tras trim y almacenado en mayúsculas.
- Normalización de entrada:
  - Convertir CodigoIsoPais a uppercase en el request para evitar variantes "es/Es/eS".
- Coherencia de lectura:
  - UsuariosService debe proyectar PoblacionCodigoIsoPais desde navegación Poblacion.
- Persistencia:
  - Usar EF Core async y AsNoTracking en lecturas.
  - Aplicar migración para cambio de esquema en SQLite.
- Compatibilidad incremental:
  - No introducir nueva entidad Pais en esta fase; se deja para una iteración posterior si negocio lo requiere.

## 7. Capas afectadas
- Models:
  - backend/Models/Poblacion.cs (nuevo CodigoIsoPais).
- Dtos:
  - backend/Contracts/ApiContracts.cs (PoblacionDto, CrearActualizarPoblacionRequest, UsuarioDto).
- LogicaNegocio:
  - Validación y normalización de CodigoIsoPais en reglas de entrada y mapeos.
- Services:
  - backend/Services/PoblacionesService.cs (crear/actualizar/mapear CodigoIsoPais).
  - backend/Services/UsuariosService.cs (proyección de PoblacionCodigoIsoPais).
- Controllers:
  - backend/Controllers/PoblacionesController.cs (sin lógica extra, solo contrato actualizado).
  - backend/Controllers/UsuariosController.cs (respuesta con DTO actualizado).
- Migraciones:
  - backend/Data/Migrations/* (nueva migración de CodigoIsoPais en Poblaciones).
- HTTP de prueba manual:
  - backend/Backend.Api.http (casos OK y error de endpoints modificados).

## 8. Tests a implementar
Según la excepción temporal activa del proyecto, no se deben crear ni actualizar tests salvo petición expresa del usuario.
Estado de esta sección en esta iteración: pendiente/no aplicable.

## 9. Criterios de aceptación
- La entidad Poblacion persiste y devuelve CodigoIsoPais en formato normalizado (2 letras mayúsculas).
- POST y PUT de poblaciones rechazan CodigoIsoPais inválido con 400 ValidationProblem.
- GET de usuarios (lista y detalle) incluye PoblacionCodigoIsoPais en el DTO de salida.
- La migración se aplica en SQLite sin romper el resto de módulos existentes.
- El archivo backend/Backend.Api.http incluye ejemplos OK y de error para cada endpoint nuevo o modificado, alineado con el puerto real de backend/Properties/launchSettings.json.

## 10. Skills a invocar
Aplicar coordinación obligatoria con orquestador-skills por tratarse de un cambio transversal (modelo, persistencia, contratos, servicios y endpoints):

1. infraestructura-dotnet
2. modelo-aplicacion
3. base-datos-aplicacion
4. dtos-aplicacion
5. validaciones-aplicacion
6. logica-negocio
7. servicios-aplicacion
8. controladores-api

Gate final de cobertura extremo a extremo:
- Verificar migración aplicada, compilación backend y actualización de backend/Backend.Api.http con casos OK/error de todos los endpoints modificados.