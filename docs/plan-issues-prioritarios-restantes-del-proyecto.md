# Plan: Issues prioritarios restantes del proyecto

**Issue:** pendiente
**Rama:** pendiente

## 1. Resumen
Este plan corrige el alcance para centrarse en issues reales del proyecto (incidencias y funcionalidad pendiente), no en codigos ISO. A partir del estado actual del repositorio, se priorizan primero los riesgos de mayor impacto operativo: seguridad de dependencias, verificabilidad de la API y brecha funcional frente al PRD.

Priorizacion propuesta:
- P1 (Critico): vulnerabilidad alta en dependencia SQLite reportada por diagnosticos del proyecto.
- P2 (Alta): cobertura incompleta de escenarios HTTP en Backend.Api.http respecto a endpoints existentes.
- P3 (Alta): ausencia de frontend React + TypeScript + Vite, definido como arquitectura objetivo en instrucciones del proyecto y esperado para demo web end-to-end.

## 2. Requisitos
- No incluir alcance de codigos ISO salvo que exista issue explicito y priorizado en backlog (no es el caso de esta correccion).
- Basar la priorizacion en evidencia del repo:
  - Diagnosticos de compilacion/dependencias.
  - PRD y analisis/diseno tecnico.
  - Cobertura real del archivo backend/Backend.Api.http frente a controladores activos.
- Resolver primero lo que reduce riesgo inmediato:
  - Seguridad (supply chain/dependencias).
  - Capacidad de validacion manual reproducible de endpoints clave.
  - Cierre del gap funcional principal de producto (frontend ausente).
- Mantener arquitectura por capas y convenciones vigentes del proyecto (ASP.NET Core Web API + EF Core/SQLite + frontend React/TypeScript/Vite).

## 3. Cambios en el modelo
- Issue P1 (vulnerabilidad dependencia): sin cambios de entidades de dominio.
- Issue P2 (cobertura HTTP): sin cambios de entidades de dominio.
- Issue P3 (frontend ausente): sin cambios obligatorios de entidades en esta fase; se reutiliza el modelo actual de API.

## 4. DTOs
- Issue P1: no requiere cambios en DTOs.
- Issue P2: no requiere cambios de contrato; requiere validar que los payloads de backend/Backend.Api.http reflejan exactamente los DTOs vigentes.
- Issue P3: no requiere crear nuevos DTOs en backend en la primera iteracion; el frontend debe consumir los contratos ya expuestos por la API.

## 5. Endpoints
Issue P2 prioriza cerrar la brecha entre endpoints implementados y ejemplos de verificacion manual en backend/Backend.Api.http.

Endpoints nuevos/modificados a reflejar en backend/Backend.Api.http (con caso OK y caso error por endpoint):
- PUT /api/departamentos/{id}
  - OK: actualizar nombre de departamento existente (200).
  - Error: departamento inexistente (404) y nombre invalido (400).
- DELETE /api/departamentos/{id}
  - OK: eliminar sin usuarios asociados (204).
  - Error: con usuarios asociados (409) y no encontrado (404).
- GET /api/sedes
  - OK: listado (200).
  - Error complementario: GET /api/sedes/{id} inexistente (404).
- POST /api/sedes
  - OK: crear sede valida (201).
  - Error: nombre invalido (400).
- PUT /api/sedes/{id}
  - OK: actualizar sede existente (200).
  - Error: sede inexistente (404) y nombre invalido (400).
- DELETE /api/sedes/{id}
  - OK: eliminar sin usuarios asociados (204).
  - Error: con usuarios asociados (409) y no encontrado (404).
- PUT /api/tipos-tarea/{id}
  - OK: actualizar tipo existente (200).
  - Error: tipo inexistente (404) y nombre invalido (400).
- DELETE /api/tipos-tarea/{id}
  - OK: eliminar tipo sin tareas asociadas (204).
  - Error: con tareas asociadas (409) y no encontrado (404).
- PUT /api/plantillas/{id}
  - OK: actualizar plantilla existente (200).
  - Error: plantilla inexistente (404) y payload invalido (400).
- DELETE /api/plantillas/{id}
  - OK: eliminar plantilla existente (204).
  - Error: plantilla inexistente (404).
- POST /api/tareas/{id}/completar
  - OK: completar tarea existente (200).
  - Error: tarea inexistente (404).
- POST /api/tareas/desde-plantilla/{plantillaId}
  - OK: crear tarea desde plantilla valida (201).
  - Error: plantilla inexistente (404).

Notas operativas para P2:
- Mantener @host alineado con backend/Properties/launchSettings.json.
- Evitar ejemplos obsoletos de payload que no coincidan con los requests actuales.

## 6. Lógica de negocio
- Issue P1 (Critico - seguridad dependencias):
  - Actualizar dependencias vulnerables de SQLite a una version no afectada por el advisory reportado.
  - Verificar restauracion y build sin alertas de seguridad de alta severidad para ese paquete.
- Issue P2 (Alta - verificabilidad API):
  - No introducir nuevas reglas de negocio; asegurar que cada endpoint principal tenga trazabilidad manual en .http con escenarios OK/error.
  - Confirmar mensajes y codigos de respuesta esperados (400/404/409) segun reglas ya implementadas.
- Issue P3 (Alta - brecha funcional PRD):
  - Implementar frontend minimo viable en React + TypeScript + Vite para los flujos del PRD:
    - Listar/crear/editar/eliminar tareas.
    - Marcar tarea como completada.
    - CRUD de plantillas e instanciacion desde plantilla.
    - Gestion de usuarios y departamentos (incluyendo sede/poblacion si forman parte del flujo actual).
  - Mantener llamadas HTTP centralizadas en servicios de frontend y formularios con validacion visible.
  - Usar HTTP plano en frontend (localhost:5173) y backend HTTPS segun configuracion local.

## 7. Capas afectadas
- Issue P1:
  - Migraciones: no aplica.
  - Models: no aplica.
  - Dtos: no aplica.
  - LogicaNegocio: no aplica.
  - Services: no aplica.
  - Controllers: no aplica.
  - Infraestructura dependencias: backend/Backend.Api.csproj y mcp/Backend.Mcp.csproj.
- Issue P2:
  - Controllers: validacion de cobertura de contratos HTTP existentes.
  - Services/LogicaNegocio: no cambios funcionales previstos.
  - Artefacto de prueba manual: backend/Backend.Api.http.
- Issue P3:
  - Frontend: nueva capa frontend/ con React + TypeScript + Vite.
  - Services de frontend: consumo de endpoints API.
  - Tipos frontend: mapeo de DTOs existentes.
  - Controllers/Services backend: solo ajustes puntuales si aparece gap contractual durante integracion.

## 8. Tests a implementar
Estado actual segun instrucciones del proyecto: excepcion temporal activa para no crear/actualizar tests salvo peticion expresa.

Por tanto, en esta iteracion:
- Pendiente/no aplicable para nuevas pruebas automatizadas.
- Se mantiene validacion manual a traves de backend/Backend.Api.http para endpoints afectados por P2.

## 9. Criterios de aceptación
- Issue P1 (Critico - seguridad dependencia):
  - Los diagnosticos del workspace dejan de reportar la vulnerabilidad alta de SQLitePCLRaw.lib.e_sqlite3 en backend/Backend.Api.csproj y mcp/Backend.Mcp.csproj.
  - Restauracion de paquetes y build completan sin errores derivados del cambio de version.

- Issue P2 (Alta - cobertura de verificacion API):
  - backend/Backend.Api.http contiene ejemplos OK y de error para cada endpoint listado en la seccion 5.
  - Todos los ejemplos usan payloads coherentes con los DTOs actuales y rutas vigentes en controladores.
  - El archivo `backend/Backend.Api.http` incluye ejemplos OK y de error para cada endpoint nuevo o modificado, alineado con el puerto real de `backend/Properties/launchSettings.json`.

- Issue P3 (Alta - cierre de brecha funcional de producto):
  - Existe una app frontend React + TypeScript + Vite operativa en frontend/.
  - La UI permite ejecutar al menos los flujos principales del PRD (CRUD tareas, completar tarea, CRUD plantillas, instanciar desde plantilla, CRUD usuarios/departamentos).
  - Las llamadas de frontend a backend quedan centralizadas en una capa de servicios y no dispersas en componentes.

## 10. Skills a invocar
Para ejecutar los issues por orden de prioridad y con gates de control:

1. orquestador-skills (coordinacion obligatoria de secuencia y cobertura extremo a extremo)
2. infraestructura-dotnet (para P1: inspeccion de proyectos, paquetes y compilabilidad)
3. controladores-api (para P2: asegurar alineacion de endpoints y contrato HTTP)
4. servicios-aplicacion (si durante P2/P3 emerge ajuste de orquestacion backend)
5. dtos-aplicacion (solo si P3 descubre desajuste de contrato)
6. validaciones-aplicacion (solo si P3 requiere endurecer validaciones visibles en formularios/API)
7. base-datos-aplicacion (solo si P3 o ajustes backend exigen cambios persistentes)

Gate final obligatorio:
- Confirmar cierre de P1 en diagnosticos.
- Confirmar cobertura completa de P2 en backend/Backend.Api.http.
- Confirmar recorrido funcional basico de P3 contra la API existente.