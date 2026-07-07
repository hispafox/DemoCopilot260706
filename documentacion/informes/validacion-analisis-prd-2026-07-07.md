# Informe de Validacion Analisis-PRD

- Fecha: 2026-07-07
- Version objetivo: v1
- Modo: completo
- Documentos evaluados:
  - PRD: documentacion/PRD.md
  - Analisis/Diseno: documentacion/analisis-diseno.md (archivo real: documentacion/analisis-diseño.md)

## 1. Resumen ejecutivo

### Indice global de sincronia

- Formula aplicada: Sincronia (%) = (items alineados / items evaluados) * 100
- Items evaluados: 32
- Items alineados: 23
- **Sincronia global: 71.9%**

### Cobertura por categoria

- RF (requisitos funcionales): 4/4 alineados (100.0%)
- RN (reglas de negocio): 7/12 alineados (58.3%)
- Alcance (incluido/fuera de alcance): 9/11 alineados (81.8%)
- NFR (no funcionales): 3/5 alineados (60.0%)

### Hallazgos por severidad

- Bloqueante: 0
- Alto: 3
- Medio: 3
- Bajo: 1

### Recomendacion de salida

**Apto con riesgos**

La cobertura funcional base (RF) esta alineada, pero hay brechas en RN criticas de comportamiento (idempotencia al completar y preservacion de tareas al eliminar plantilla) que deben cerrarse antes de considerar sincronizacion documental completa de v1.

---

## 2. Matriz de trazabilidad

| Item | Evidencia en PRD | Evidencia en analisis/diseno | Estado | Severidad |
|---|---|---|---|---|
| RF-01 CRUD tareas | "crear, listar, obtener por id, actualizar y eliminar tareas" | Endpoints `/api/tareas` GET/GET{id}/POST/PUT/DELETE | Alineado | - |
| RF-02 Completar tarea | "accion explicita" y generar siguiente ocurrencia si es repetitiva | `POST /api/tareas/{id}/completar` con generacion de siguiente ocurrencia | Alineado | - |
| RF-03 CRUD plantillas | "crear, listar, actualizar y eliminar plantillas" | Endpoints `/api/plantillas` GET/GET{id}/POST/PUT/DELETE | Alineado | - |
| RF-04 Instanciar plantilla | "crear una tarea nueva desde una plantilla existente" | `POST /api/plantillas/{id}/instanciar` | Alineado | - |
| RN-01 Titulo obligatorio con trim | "titulo ... obligatorio ... tras aplicar trim" | Modelo `TodoItem.Title` requerido, sin regla explicita de trim | Parcial | Medio |
| RN-02 Nueva tarea no completada por defecto | "se crea en estado no completada por defecto" | `IsCompleted` por defecto `false` | Alineado | - |
| RN-03 Solo repetitivas con recurrencia | "Solo las tareas marcadas como repetitivas pueden tener tipo de recurrencia" | `Recurrencia` nullable, sin regla explicita de validacion cruzada | Parcial | Medio |
| RN-04 No repetitiva sin recurrencia/proxima fecha | "deben permanecer sin valor" | Campos nullable (`Recurrencia`, `ProximaFecha`), sin enforcement explicito | Parcial | Medio |
| RN-05 Completar no repetitiva no genera extra | "no se genera ninguna tarea adicional" | Endpoint describe generacion solo si es repetitiva | Alineado | - |
| RN-06 Completar repetitiva genera 1 ocurrencia | "exactamente una siguiente ocurrencia" | Decision tecnica: logica en `TodoService.Completar()` | Alineado | - |
| RN-07 Idempotencia al completar | "accion de completar es idempotente" | No aparece regla/estrategia explicita anti-duplicados | No alineado | Alto |
| RN-08 Plantilla con titulo obligatorio | "plantilla debe tener titulo obligatorio" | `PlantillaTarea.Titulo` requerido | Alineado | - |
| RN-09 Instancia independiente de plantilla | "cambios posteriores ... no alteran tareas" | Pendiente indica que tareas creadas no se actualizan automaticamente | Alineado | - |
| RN-10 Asociacion tarea-plantilla opcional | "asociacion ... opcional" | `PlantillaId` nullable | Alineado | - |
| RN-11 Eliminar plantilla conserva tareas | "tareas ya creadas ... deben conservarse" | No se explicita comportamiento de borrado/relacion en DB/servicio | No alineado | Alto |
| RN-12 Recurrencia solo al completar | "no existe generacion automatica por tiempo" | Pendiente confirma que no hay job automatico | Alineado | - |
| Alcance incluido v1 | CRUD tareas, plantillas, recurrencia diaria/semanal/mensual, completar explicito | Endpoints y modelo cubren esos flujos | Alineado | - |
| Alcance fuera de v1 | sin auth, sin paginacion avanzada, sin busqueda/filtros complejos, sin notificaciones, sin jobs automaticos, sin multiusuario | Se explicita sin auth/sin paginacion/sin filtros complejos/sin jobs automaticos; no explicito para notificaciones y multiusuario | Parcial | Bajo |
| NFR claridad y mantenibilidad | "claridad ... sobre sofisticacion" y separacion responsabilidades | Se declara arquitectura plana y reglas de diseno simples | Alineado | - |
| NFR consistencia HTTP/validaciones | "consistencia de respuestas HTTP y validaciones basicas" | Tabla HTTP completa; validaciones de negocio no siempre explicitas | Parcial | Medio |
| NFR SQLite local | "persistencia local en SQLite" | Decision tecnica explicita de SQLite | Alineado | - |
| NFR cobertura pruebas no triviales | "cobertura de pruebas en cambios no triviales" | Se menciona xUnit+Moq, sin criterio de cobertura/estrategia de aceptacion | Parcial | Medio |

---

## 3. Hallazgos

### H-01
- Severidad: Alto
- Descripcion corta: RN-07 (idempotencia al completar) no esta aterrizada en diseno tecnico.
- Hecho (PRD): "La accion de completar es idempotente ... no debe generar ocurrencias duplicadas".
- Hecho (Analisis): no hay estrategia explicita para evitar duplicados por doble llamada a `/completar`.
- Impacto: riesgo de generar ocurrencias duplicadas y defectos funcionales en flujos repetitivos.
- Accion recomendada: actualizar analisis/diseno con mecanismo concreto (guardas de estado/versionado/transaccion) y casos de prueba de idempotencia. Documento a actualizar: analisis/diseno.

### H-02
- Severidad: Alto
- Descripcion corta: RN-11 (conservacion de tareas al eliminar plantilla) no esta especificada tecnicamente.
- Hecho (PRD): "Al eliminar una plantilla, las tareas ya creadas ... deben conservarse".
- Hecho (Analisis): no se define politica de FK ni comportamiento de borrado en servicio/DB.
- Impacto: riesgo de borrado accidental, errores de integridad o comportamiento ambiguo en produccion/demo.
- Accion recomendada: definir explicitamente politica de relacion (`SetNull`/restriccion) y flujo en servicio/controlador, con test de integridad. Documento a actualizar: analisis/diseno.

### H-03
- Severidad: Alto
- Descripcion corta: Desalineacion semantica de entidad principal y nomenclatura de campos.
- Hecho (PRD): el dominio habla de "tarea" con reglas en castellano (titulo, completada, etc.).
- Hecho (Analisis): modelo principal propuesto como `TodoItem` con campos `Title`, `IsCompleted`, `CreatedAt`.
- Impacto: riesgo de traduccion inconsistente en API/codigo/tests y mayor friccion didactica.
- Accion recomendada: estabilizar nomenclatura canonica en analisis/diseno (idealmente alineada a PRD) o documentar tabla de mapeo obligatoria PRD->modelo. Documento a actualizar: analisis/diseno (y PRD solo si se decide cambiar canon).

### H-04
- Severidad: Medio
- Descripcion corta: RN-01 exige trim en titulo y no se refleja regla tecnica explicita.
- Hecho (PRD): "titulo ... obligatorio ... tras aplicar trim".
- Hecho (Analisis): se marca campo requerido, pero no se define validacion de trim.
- Impacto: entradas con espacios pueden pasar en algunos flujos.
- Accion recomendada: agregar validacion tecnica explicita en analisis/diseno y pruebas de borde. Documento: analisis/diseno.

### H-05
- Severidad: Medio
- Descripcion corta: RN-03 y RN-04 no tienen enforcement tecnico explicito.
- Hecho (PRD): reglas cruzadas entre `EsRepetitiva`, `Recurrencia` y `ProximaFecha`.
- Hecho (Analisis): los campos son nullable, pero no se define validacion de consistencia.
- Impacto: datos inconsistentes en persistencia y comportamiento inesperado en completar.
- Accion recomendada: documentar reglas de validacion en servicio y casos de error HTTP (`400`) asociados. Documento: analisis/diseno.

### H-06
- Severidad: Medio
- Descripcion corta: NFR de pruebas no triviales queda declarativo, sin criterio operativo.
- Hecho (PRD): exige cobertura de pruebas en cambios no triviales.
- Hecho (Analisis): solo menciona stack `xUnit + Moq`.
- Impacto: riesgo de cobertura insuficiente en flujos de recurrencia e integridad de plantillas.
- Accion recomendada: definir matriz minima de pruebas (CRUD, completar repetitiva/no repetitiva, idempotencia, borrar plantilla con tareas asociadas). Documento: analisis/diseno.

### H-07
- Severidad: Bajo
- Descripcion corta: Alcance fuera de v1 no esta totalmente reflejado (notificaciones y multiusuario).
- Hecho (PRD): ambos figuran fuera de alcance.
- Hecho (Analisis): no se contradicen, pero no quedan explicitamente listados.
- Impacto: leve riesgo de interpretaciones distintas en planning.
- Accion recomendada: incluir esos dos puntos en seccion de fuera de alcance del analisis/diseno. Documento: analisis/diseno.

---

## 4. Plan de sincronizacion

### Cambios minimos en PRD

1. (Opcional) Solo si se decide mantener nombres tecnicos en ingles, agregar glosario corto PRD->modelo tecnico para evitar ambiguedad.

### Cambios minimos en analisis/diseno

1. Especificar idempotencia de `/completar` (RN-07) con estrategia y prueba.
2. Especificar politica de borrado de plantilla y comportamiento de `PlantillaId` (RN-11).
3. Documentar validaciones de trim y consistencia `EsRepetitiva/Recurrencia/ProximaFecha` (RN-01, RN-03, RN-04).
4. Completar fuera de alcance explicito para notificaciones y multiusuario.
5. Definir set minimo de pruebas de aceptacion/servicio para RN criticas.
6. Cerrar pendiente de nomenclatura canonica (Tarea vs TodoItem) o incorporar mapeo formal.

### Orden sugerido de actualizacion

1. Cerrar RN criticas (H-01 y H-02).
2. Cerrar coherencia semantica/nomenclatura (H-03).
3. Cerrar validaciones de entrada y consistencia de modelo (H-04 y H-05).
4. Cerrar NFR de pruebas y explicitar fuera de alcance restante (H-06 y H-07).

---

## 5. Criterio de cierre

Se consideran sincronizados ambos documentos para v1 cuando:

1. RN-07 y RN-11 queden especificadas en analisis/diseno con estrategia tecnica verificable y pruebas asociadas.
2. La nomenclatura de dominio quede estable y sin ambiguedad (o con mapeo oficial documentado).
3. Las reglas RN-01, RN-03 y RN-04 tengan validaciones tecnicas explicitas y codigos HTTP de error consistentes.
4. El alcance fuera de v1 este explicitamente reflejado en ambos documentos sin omisiones relevantes.
5. Exista una matriz minima de pruebas vinculada a RF/RN de mayor riesgo.
