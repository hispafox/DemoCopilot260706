# Veredicto del consejo (LLM Council): evaluación del agente `auditor-calidad`

- **Fecha:** 2026-07-09
- **Objeto evaluado:** `.github/agents/auditor-calidad.agent.md`
- **Método:** LLM Council (5 asesores independientes → revisión entre pares anónima → síntesis del presidente)
- **Nota:** Este documento es un informe de decisión. El consejo **aconseja una dirección; no reescribe el agente**. No se ha modificado ningún archivo del repositorio para producirlo.

---

## 0. Pregunta encuadrada (lo que recibió cada asesor)

> Evaluar la calidad de diseño y la eficacia del agente `auditor-calidad`, un agente custom de VS Code para el repo formativo DemoCopilot (app de tareas .NET 10 + React/TS, entidad principal `Tarea`). Se define como auditor senior full-stack en modo abogado del diablo y **solo lectura**, que emite veredicto go/no-go con evidencia `archivo#Lnn`. Conviven con él un sistema multiagente (`planificador` → `desarrollador` → `verificador`, orquestado, máx. 3 vueltas, donde `verificador-democopilot` también es solo-lectura, compila con `dotnet build` y emite APROBADO/REVISAR) y agentes globales `code-reviewer` y `security-reviewer`. ¿Está bien diseñado, es robusto y no redundante? ¿Qué fallos, puntos ciegos o incoherencias tiene y qué debería cambiar? Veredicto: mantener / ajustar / replantear. En juego: es material didáctico que enseña el rol de "puerta de calidad" y además se usará para auditar código real.

**Ponderación de lentes (tipo de decisión: "elegir herramienta / tooling + QA de diseño"):** mandan **de Fuera, Ejecutor y Contrario**; **Expansivo** habla más corto. Todas contribuyen.

**Dato verificado antes de sintetizar** (para no debatir sobre una especulación): los nombres de herramienta del frontmatter son reales y consistentes en el repo:

| Agente | Rol | `tools` declarado |
|---|---|---|
| `desarrollador-democopilot` | Mutante (implementa) | `[read, search, edit, execute]` |
| `verificador-democopilot` | Solo lectura, **sí compila** | `[read, search, execute]` |
| `auditor-calidad` | Solo lectura, **dice no compilar** | `[execute, read, edit, search, web]` |

El auditor recibió el mismo `edit` que el agente mutante y el `execute` del verificador, pese a que su carta prohíbe ambos. La incoherencia es un hecho, no una hipótesis.

---

## 1. Respuestas de los asesores (íntegras, des-anonimizadas)

### El Contrario  *(anon: Respuesta C)*

> El fallo fatal está en el frontmatter, y contradice todo lo demás: `tools: [execute, read, edit, search, web]`. Un agente cuya única razón de ser es SER SOLO LECTURA declara `edit` y `execute` entre sus herramientas. La garantía de no-mutación vive solo en la prosa del cuerpo, no en la configuración. Eso no es un auditor read-only: es un editor con una promesa. Como material didáctico que enseña "puerta de calidad", enseña justo lo peor: que solo-lectura es una sugerencia, no una restricción. Quita `edit` y `execute`. Si no puedes hacerlo cumplir por configuración, no lo estás diseñando, lo estás deseando.
>
> Segundo: redundancia sin routing. Tienes cuatro puertas —`auditor-calidad`, `verificador-democopilot`, `code-reviewer`, `security-reviewer`— y cero reglas de cuándo llamar a cuál ni de quién gana. ¿Qué pasa cuando `verificador` dice APROBADO y `auditor` dice no-go sobre el mismo commit? Indefinido. La dimensión OWASP pisa a `security-reviewer`; convenciones y smells pisan a `code-reviewer`. Estás pagando cuatro veces por el mismo trabajo y produciendo veredictos que pueden colisionar.
>
> Tercero, incoherencia de raíz: el auditor prohíbe "builds que escriban artefactos", así que no puede compilar. Entonces no detecta errores de tipo, ni N+1 real, ni fallos que solo aparecen al construir. Su go/no-go es *más débil* que el de `verificador` (que sí buildea) pero no se subordina a nadie. Un juez que no puede abrir el expediente.
>
> Cuarto: la evidencia `archivo#Lnn` se pudre. En un repo formativo editado constantemente, las líneas se desplazan y las citas quedan mintiendo. Y `web` en un auditor local es superficie de inyección gratuita.
>
> Veredicto: **replantear**, no ajustar. La premisa central no está implementada.

### El de Primeros Principios  *(anon: Respuesta A)*

> La pregunta que traes —"¿está bien diseñado este agente?"— es la equivocada. La pregunta real es: **¿por qué existe un tercer solo-lectura que solo diagnostica?**
>
> Reconstruyamos desde cero. ¿Qué resuelve de verdad una puerta de calidad? Convertir "el código está mal" en "el código no pasa". Un veredicto go/no-go solo tiene valor si **bloquea** algo. Pero `auditor-calidad` no bloquea nada: emite un informe de 7 secciones que un humano puede ignorar. Ya tienes `verificador-democopilot`, que sí está enganchado a un flujo orquestado con gate real (máx 3 vueltas, APROBADO/REVISAR) y ejecuta `dotnet build`. Eso es una puerta. Lo tuyo es un ensayo.
>
> Segunda suposición que quito: que "senior full-stack abogado del diablo solo-lectura" sea una identidad. No lo es, es un solapamiento. `code-reviewer` + `security-reviewer` + `verificador` ya cubren convenciones, OWASP y build. ¿Qué queda que sea *exclusivamente* de este agente? Nada estructural: solo el tono escéptico. Un tono no justifica un agente.
>
> Tercera, la más grave: `tools: [execute, read, edit, search, web]`. Declaras SOLO LECTURA en prosa pero concedes `edit` y `execute` en el contrato de la máquina. El sistema no lee tu manifiesto de identidad; lee el frontmatter. **Tu restricción fundamental es decorativa.** Un abogado del diablo con permiso de escritura es exactamente el fallo que este agente diría cazar en otros.
>
> Replantear. No pulir. La pregunta didáctica correcta no es "cómo audito", sino "qué hace que un veredicto sea vinculante". O fusionas su escepticismo dentro del `verificador` (donde el gate existe), o lo conviertes en un `code-reviewer` especializado en capas .NET/EF. Como pieza autónoma, enseña una puerta que no cierra.

### El Expansivo  *(anon: Respuesta E)*

> Este agente está infravalorado como pieza suelta. Su verdadero potencial no es "auditar", es convertirse en la **puerta de calidad canónica de todo el ecosistema**. Ahí hay oro didáctico sin explotar.
>
> Primero: `verificador-democopilot`, `code-reviewer`, `security-reviewer` y `auditor-calidad` no son redundancia, son un **currículo de niveles de rigor**. Enséñalos como tal. El verificador es el semáforo rápido dentro del loop (build + APROBADO/REVISAR); el auditor es el forense externo que entra cuando el semáforo pasa pero algo huele mal. Eso es una lección enorme sobre "calidad barata vs. calidad cara" que ningún curso enseña bien. Documéntalo y multiplicas el valor formativo por tres.
>
> Segundo, la oportunidad adyacente: el veredicto `archivo#Lnn` es **material estructurado, no prosa**. Si el informe de 7 secciones sale como tabla parseable, tienes gratis: métricas de deuda técnica en el tiempo, un dashboard de evolución, incluso alimentar sub-issues a Copilot. El agente puede pasar de "escribe un informe" a "genera el backlog priorizado de calidad". Ese salto es casi gratis y transforma el rol de puerta en **motor de mejora continua**.
>
> Tercero, lo más grande: el modo "abogado del diablo escéptico basado en evidencia" es un **patrón reutilizable de red-team** más allá de este repo. Extráelo. Un auditor que exige evidencia `archivo#Lnn` y prohíbe adulación es plantilla para auditar cualquier stack.
>
> ¿Qué si sale mejor de lo esperado? Que los alumnos no aprenden "un agente", aprenden a **componer una malla de garantía de calidad** con roles diferenciados. Mantener y expandir: convierte las cuatro piezas en un pipeline explícito y saca el informe a formato estructurado.

### El de Fuera  *(anon: Respuesta B)*

> Leo esto por primera vez y hay cosas que se dan por hechas.
>
> **El frontmatter contradice al cuerpo.** `tools: [execute, read, edit, search, web]` incluye `edit` y `execute`, pero el agente dice ser SOLO LECTURA y "no ejecuta comandos que muten". ¿Por qué tiene permiso de escritura y ejecución un auditor que jura no usarlos? O el editor lo ignora, o es un accidente esperando a pasar. Desde fuera, esto huele a copia-pega de otra plantilla. Quítalo: si es solo lectura, dale solo `read`, `search`, `web`.
>
> **"archivo#Lnn" no se explica.** Sé que significa ruta+línea porque me lo cuentas en la pregunta, pero el agente asume que quien lo lee ya entiende el formato. ¿Línea de qué versión? Si el código cambia, `foo.cs#L42` apunta a otra cosa. ¿Rango de líneas? ¿Commit fijado? Ambiguo.
>
> **Los "4 documentos fuente de verdad" son fantasmas.** No se nombran aquí. El agente "declara cuáles encontró", pero ¿qué pasa si no encuentra ninguno? ¿Aborta? ¿Audita a ciegas? Un forastero no sabe si el agente sirve sin ellos.
>
> **El "modelo canónico Tarea" hardcodeado me chirría.** ¿Por qué un auditor "full-stack genérico" conoce una entidad concreta con reglas de negocio (1-200 caracteres)? Eso acopla un rol supuestamente reutilizable a un proyecto específico. Confuso: ¿es genérico o es para *esta* app?
>
> **Solapamiento sin fronteras.** Coexiste con verificador, code-reviewer y security-reviewer. Menciona "riesgos OWASP" y "calidad React"... que es literalmente lo que hacen los otros dos. ¿Cuándo llamo a cuál? Nadie lo dice.
>
> **Cambiaría:** alinea tools con SOLO LECTURA, define `archivo#Lnn` con commit, nombra los 4 documentos y su fallback, y declara explícitamente qué NO cubre este agente frente a los otros tres.

### El Ejecutor  *(anon: Respuesta D)*

> El lunes por la mañana el problema no es la filosofía del "abogado del diablo", es que el agente se contradice a sí mismo en la primera línea ejecutable. Declaras SOLO LECTURA pero le das `tools: [execute, read, edit, search, web]`. `edit` sobra: quítalo ya. No es un matiz teórico —un agente que puede editar terminará editando en cuanto una vuelta se ponga difícil. La herramienta define la capacidad, no el párrafo del cuerpo.
>
> Segundo, el nudo real: prohíbes "builds que escriban artefactos" pero dejas `execute`. ¿Para qué? Un `dotnet build` normal escribe bin/obj. Entonces tu auditor NO puede compilar. Y si no compila, ¿cómo verifica "sin N+1", "AsNoTracking", nulabilidad, async? Solo por lectura estática. Vale, pero entonces `execute` no sirve para nada útil y es puro riesgo. Decisión práctica: o le quitas `execute` (auditoría 100% estática, honesta con su nombre) o le permites `dotnet build` contra una carpeta temporal / `--no-incremental` en sandbox. No dejes el estado intermedio actual, que es "tengo la llave pero tengo prohibido usarla".
>
> Tercero, redundancia: ya tienes `verificador-democopilot` que SÍ compila y da APROBADO/REVISAR. Este auditor da go/no-go sin compilar. Solapamiento de veredictos, distinto criterio, misma puerta. En la práctica el alumno no sabrá cuál manda. Define quién decide: verificador = compila (gate técnico), auditor = revisión estática de reglas (gate de calidad). Roles disjuntos o uno sobra.
>
> Primer paso concreto: (1) borrar `edit`, (2) resolver `execute` —yo lo quitaría—, (3) escribir una frase que diga "este agente NO compila; delega la compilación en el verificador". Con eso es coherente y usable el lunes. Veredicto: **ajustar**, no replantear. La estructura sirve; el frontmatter miente.

---

## 2. Revisión entre pares (íntegra)

> Mapa de anonimización usado en esta ronda: **A** = Primeros Principios · **B** = de Fuera · **C** = Contrario · **D** = Ejecutor · **E** = Expansivo. Los revisores no conocían este mapa.

### Revisor 1
> **1. Respuesta más fuerte: D.** Es la única que convierte el diagnóstico en decisión operativa con un razonamiento cerrado: `dotnet build` escribe en `bin/obj`, luego con la prohibición de artefactos `execute` es una llave inutilizable → o auditoría 100% estática o build en sandbox. No hay ambigüedad ni falsa épica. Su veredicto "ajustar, no replantear" está justificado, a diferencia de C y A que saltan a "replantear" sin descartar el arreglo barato.
>
> **2. Mayor punto ciego: E.** Romantiza el potencial (dashboard, backlog, red-team) e ignora el fallo bloqueante que las otras cuatro ven: el frontmatter `edit`/`execute` contradice SOLO LECTURA. Construye una catedral sobre un cimiento roto; le falta reconocer que sin resolver la contradicción de capacidades y el routing entre las cuatro puertas, "expandir" amplifica el riesgo.
>
> **3. Lo que todas escaparon:** nadie cuestiona que el sistema realmente **lea y aplique** ese frontmatter en agentes custom de VS Code, ni verifica que `execute`/`edit` sean nombres de herramienta válidos. Si el runtime ignora `tools` mal formado, el debate sobre capacidades es especulativo. Además, ninguna define el criterio de conflicto: verificador APROBADO vs auditor no-go sobre el mismo commit —quién gana— queda sin resolver por todas.

### Revisor 2
> **1. Más fuerte: C.** Es la única que nombra el problema de *routing* con un caso concreto (verificador=APROBADO vs auditor=no-go → indefinido) y además marca `web` como superficie de inyección, algo que solo ella ve. Combina el fallo de `tools` con la ausencia de subordinación jerárquica. A razona parecido pero se queda en lo retórico; C aterriza el conflicto operativo.
>
> **2. Mayor punto ciego: E.** Romantiza el agente ("puerta canónica", "dashboard", "backlog") sin tocar la contradicción `edit`/`execute` vs SOLO LECTURA que las otras cuatro consideran descalificante. Construye una catedral sobre unos cimientos que ni inspecciona: si el frontmatter miente, no hay currículo de rigor, hay teatro. Le falta lo mínimo: honestidad sobre la config actual.
>
> **3. Lo que se les escapó a las cinco:** nadie cuestiona si el formato de evidencia `archivo#Lnn` es el correcto — B y C solo dicen que "se pudre". Lo real: en un repo Git la evidencia debería anclarse a `commit:archivo#Lnn` o a un permalink, y eso es un requisito *verificable* del agente, no un detalle. Tampoco nadie pregunta **quién consume el veredicto**: si ningún flujo lo lee, el debate mantener/ajustar/replantear es ocioso. La existencia se justifica por el consumidor, no por el diseño.

### Revisor 3
> **1. Más fuerte: B.** Es la única que cataloga *todos* los defectos accionables sin exagerar ni caer en dramatismo: contradicción frontmatter/cuerpo, `archivo#Lnn` sin commit se pudre, 4 docs "fuente de verdad" sin nombrar ni fallback, y el acople del "modelo canónico Tarea" a un rol genérico. Ese último punto —hardcodear dominio en un agente reutilizable— solo lo ve B y es el que más cambia la decisión: convierte "ajustar tools" en "reconsiderar el rol". C y D aciertan en `edit`/`execute` pero se quedan ahí.
>
> **2. Mayor punto ciego: E.** Ignora que el frontmatter contradice la premisa solo-lectura; construye un dashboard sobre cimientos rotos. Le falta lo básico: un agente que jura solo-lectura con `edit`/`execute` no es "puerta canónica", es riesgo. Vende expansión antes de arreglar la fundación.
>
> **3. Se les escapó a las cinco:** nadie define el *routing ni la resolución de conflictos* entre los 4 agentes (C lo roza pero no lo resuelve). Si `auditor` da no-go y `verificador` da APROBADO, ¿quién manda en el gate? Sin esa regla, cualquier veredicto —mantener, ajustar o replantear— deja el sistema indeterminado. Esa es la decisión real pendiente.

### Revisor 4
> **1. Más fuerte: B.** Es la única que combina precisión técnica y accionabilidad sin dramatizar. Detecta la contradicción `tools`↔cuerpo, la podredumbre de `archivo#Lnn` sin commit, los 4 docs sin nombrar ni fallback, y el acoplamiento del "modelo canónico Tarea" que rompe la genericidad del rol. Cada hallazgo trae fix verificable. D acierta en lo quirúrgico (borrar `edit`, quitar `execute`) pero cubre menos superficie.
>
> **2. Mayor punto ciego: E.** Premia retórica ("puerta canónica", "dashboard", "red-team reutilizable") sin tocar el fallo real: un agente solo-lectura con `edit`/`execute` no es infravalorado, está roto. Ignora la contradicción de permisos, el solapamiento y el conflicto de veredictos. Vende potencial futuro en lugar de auditar el diseño actual.
>
> **3. Lo que se les escapó a las cinco:** nadie define el **contrato de resolución de conflictos** cuando `auditor` dice no-go pero `verificador` da APROBADO. Todas señalan el solapamiento, pero ninguna especifica quién gana ni cómo se orquesta el gate real. Tampoco cuestionan si un agente que *no compila* puede legítimamente emitir go/no-go: eso es opinión sin evidencia ejecutable, no auditoría. **Veredicto: ajustar.**

### Revisor 5
> **1. Más fuerte: B.** Es la única que combina precisión técnica (contradicción frontmatter/cuerpo, `archivo#Lnn` sin commit se pudre) con el problema de acoplamiento —"Tarea hardcodeado"— y exige declarar fronteras. Diagnostica sin caer en el falso dilema "replantear o mantener": propone arreglos concretos y accionables.
>
> **2. Mayor punto ciego: A.** Reduce todo a solapamiento y concluye "solo queda el tono", ignorando que en un repo FORMATIVO el tono/rol *es* la función: enseñar la diferencia entre semáforo rápido y forense externo tiene valor pedagógico propio. También descarta el valor de un diagnóstico no bloqueante sin considerar que enseñar "puerta que informa vs. puerta que bloquea" es currículo.
>
> **3. Lo que se les escapó a las cinco:** ninguna resuelve la tensión central del doble propósito. C y D tratan `edit`/`execute` como fallo de seguridad puro; E lo idealiza como currículo. Pero nadie señala que *en material didáctico* un frontmatter que se contradice con el cuerpo enseña activamente la práctica errónea a los alumnos —el bug no es solo técnico, es que el ejemplo se copiará. El consejo debería priorizar coherencia frontmatter↔cuerpo como requisito pedagógico, no solo operativo.

---

## 3. VEREDICTO DEL CONSEJO

### Dónde coincide el consejo
- **Incoherencia frontmatter↔carta (4 de 5, y verificada).** Contrario, Primeros Principios, de Fuera y Ejecutor convergen de forma independiente: `tools: [execute, read, edit, search, web]` concede `edit` y `execute` a un agente cuya razón de ser es **solo lectura**. La restricción vive en la prosa, no en la configuración que el runtime aplica. Verificado contra el repo: es exactamente el mismo `edit` del agente **mutante** `desarrollador-democopilot`. Señal de altísima confianza; probable copia-pega de plantilla.
- **Solapamiento con las otras puertas.** Auditor, `verificador`, `code-reviewer` y `security-reviewer` cubren terreno común (convenciones, OWASP, calidad React) sin ninguna regla de cuándo invocar cada uno.
- **El auditor no compila y eso debilita su go/no-go.** Al prohibirse los builds que escriben artefactos, no puede verificar por ejecución (N+1 real, tipos, async); su veredicto es más débil que el del `verificador`, que sí compila.
- **Quitar `edit` es no-negociable.** Ningún asesor lo defiende; cuatro lo exigen explícitamente.

### Dónde choca el consejo
- **Ajustar vs. replantear.** *Ejecutor* y *de Fuera*: la estructura del agente es sólida (severidades, dimensiones, formato de informe, antifalsos positivos); "el frontmatter miente" → **ajustar**. *Contrario* y *Primeros Principios*: la premisa central (una puerta que **bloquea**) no está implementada y el rol se solapa con otros tres → **replantear/fusionar**. Discrepan razonablemente porque miden cosas distintas: la calidad del *artefacto* (buena) frente a la justificación de su *existencia autónoma* (dudosa).
- **Qué hacer con `execute`.** Ejecutor: quitarlo (auditoría 100% estática, honesta) **o** permitir build en sandbox/temp; no dejar el estado intermedio. Contrario: fuera. Nadie defiende dejarlo como está.
- **El valor del rol.** *Expansivo* (en solitario): no es redundancia sino un "currículo de niveles de rigor" con potencial de motor de mejora continua (informe parseable → métricas → backlog). El resto lo ve como solapamiento no resuelto.

### Puntos ciegos que cazó el consejo (solo salieron en la revisión)
- **No hay contrato de resolución de conflictos** (Revisores 1, 3 y 4). Si `verificador` da APROBADO y `auditor` da no-go sobre el mismo commit, **quién gana está indefinido**. Los revisores lo elevan a "la decisión real pendiente": sin esta regla, cualquier veredicto deja el sistema indeterminado.
- **La evidencia debe anclarse a commit** (Revisor 2). `archivo#Lnn` "se pudre" al desplazarse las líneas; el requisito verificable correcto es `commit:archivo#Lnn` o permalink. No es un detalle: es parte del contrato de salida.
- **El dominio hardcodeado rompe la genericidad** (Revisor 3). El "modelo canónico `Tarea`" incrustado convierte "ajustar tools" en "reconsiderar el rol": un auditor que se anuncia full-stack genérico está acoplado a una entidad concreta —e, irónicamente, **duplica** reglas que ya viven en `copilot-instructions.md`, justo lo que el agente prohíbe.
- **El fallo es didáctico, no solo técnico** (Revisor 5). En material formativo, un frontmatter que contradice al cuerpo **enseña la práctica errónea**: los alumnos copiarán el ejemplo. La coherencia frontmatter↔cuerpo es un requisito pedagógico, no solo operativo.
- **¿Quién consume el veredicto?** (Revisores 1 y 2). No está confirmado que el runtime aplique estrictamente `tools`, ni que exista un flujo que lea el informe del auditor. La existencia se justifica por el consumidor, no por el diseño.

### La recomendación
**Ajustar de forma decidida ahora; dejar el replanteamiento del rol como decisión explícita a continuación.** No es "mantener y expandir" (Expansivo) ni "fusionar ya" (Primeros Principios): es corregir la incoherencia que hoy hace peligroso y confuso al agente, y *después* decidir si sigue como pieza autónoma. En concreto, y por orden:

1. **Alinear `tools` con la carta:** dejar `tools: [read, search]`. Quitar `edit` y `execute` (contradicen la premisa) y `web` (superficie de inyección innecesaria para una auditoría local; si se quiere para consultar OWASP, dejarlo **solo** con justificación escrita). Esto es lo único no-negociable e inmediato.
2. **Declarar que el auditor NO compila** y que la verificación por ejecución (build, tipos) se **delega en `verificador-democopilot`**. El auditor hace análisis estático de reglas; el verificador es el gate técnico.
3. **Definir el routing y la precedencia entre las 4 puertas:** cuándo se invoca cada una y quién manda en conflicto. Propuesta del consejo: `verificador` = gate técnico (build); `auditor` = gate de calidad estático; ante `auditor` no-go con `verificador` APROBADO, **prevalece no-go** (o se escala a humano), nunca se ignora en silencio.
4. **Desacoplar el dominio:** sacar el "modelo canónico `Tarea`" del agente y hacer que lo **lea** de `copilot-instructions.md` en el arranque. Elimina la duplicación (coherente con su propia regla anti-duplicación) y devuelve genericidad al rol.
5. **Anclar la evidencia a commit:** cambiar el contrato de `archivo#Lnn` a `commit:archivo#Lnn` o permalink, para que las citas no mientan cuando el código se mueva.

El presidente se aparta parcialmente de la mayoría "ajustar" incorporando el hallazgo de la revisión (routing/precedencia): sin el punto 3, el arreglo del frontmatter deja un sistema coherente pero **indeterminado**. Con los cinco puntos, el agente pasa de "editor con una promesa" a "auditor estático honesto y subordinado", que es defendible tanto como herramienta como material didáctico.

### Confianza
**78%** — Sube porque la incoherencia frontmatter↔carta es un hecho verificado contra los otros agentes del repo y su arreglo es barato y de alto impacto. Bajaría a ~60% (inclinándose a replantear/fusionar) si se confirma que **ningún flujo consume** el informe del auditor. Subiría a ~88% con dos comprobaciones: (a) que el runtime de agentes custom aplica `tools` de forma estricta, y (b) una regla de routing escrita entre las cuatro puertas.

### Voto particular — El de Primeros Principios
> "Aplaudís el arreglo barato y os quedáis tranquilos, pero el arreglo barato es precisamente la trampa. Podéis quitar `edit`, anclar la evidencia al commit y escribir un routing bonito, y aun así no habréis contestado la única pregunta que importa: **¿qué hace este agente que `verificador` + `code-reviewer` + `security-reviewer` no hagan ya?** Un veredicto que no bloquea es un ensayo con formato de sentencia. Si le ponéis precedencia sobre el `verificador` (punto 3), entonces habéis creado un *segundo* gate técnico sin build que puede vetar al que sí compila —peor, no mejor. Lo honesto no es pulir el auditor: es fundir su escepticismo dentro del `verificador`, que es donde el gate ya cierra, y quedaros con tres puertas nítidas en vez de cuatro que se pisan. Coherente no es lo mismo que necesario."

### Lo primero que hay que hacer
Editar el frontmatter de `.github/agents/auditor-calidad.agent.md` y dejar **`tools: [read, search]`** (quitar `edit`, `execute` y `web`). Un solo cambio que alinea la capacidad real del agente con su carta de solo-lectura y elimina hoy el riesgo y la lección errónea; todo lo demás (routing, desacoplar dominio, anclar a commit) se construye sobre esa base.
