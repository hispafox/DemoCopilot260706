---
name: llm-council
creado: 2026-06-27
actualizado: 2026-06-27
description: "Pasa cualquier pregunta, idea o decisión por un consejo de 5 asesores de IA que la analizan de forma independiente, se revisan entre ellos en anónimo y sintetizan un veredicto final. Basado en la metodología LLM Council de Karpathy. DISPARADORES OBLIGATORIOS: 'pásalo por el consejo', 'reúne al consejo', 'debate esto', 'pon esto a prueba', 'somételo a presión', 'council this', 'run the council', 'pressure-test this', 'debate this'. DISPARADORES FUERTES (úsalos cuando hay una decisión o tradeoff real): '¿hago X o Y?', '¿qué opción?', '¿tú qué harías?', '¿es la jugada correcta?', 'valida esto', 'dame varias perspectivas', 'no me decido', 'estoy entre X e Y', 'should I X or Y', 'which option', 'validate this'. NO disparar en preguntas de sí/no triviales, búsquedas de un dato, o un 'debería' casual sin tradeoff de fondo (p. ej. '¿uso markdown?' no es para el consejo). SÍ disparar cuando el usuario trae una decisión genuina con riesgo, varias opciones y contexto que sugiere que quiere ponerla a prueba desde varios ángulos."
---

# LLM Council

Le preguntas a una IA y te da una respuesta. Puede ser buenísima. Puede ser del montón. No tienes forma de saberlo porque solo has visto una perspectiva.

El consejo arregla eso. Pasa tu pregunta por 5 asesores independientes, cada uno pensando desde un ángulo radicalmente distinto. Luego se revisan el trabajo entre ellos. Luego un presidente lo sintetiza todo en una recomendación final que te dice dónde coinciden los asesores, dónde chocan y qué deberías hacer de verdad.

Está adaptado del LLM Council de Andrej Karpathy. Él reparte la consulta a varios modelos, hace que se revisen entre ellos en anónimo, y un presidente produce la respuesta final. Aquí hacemos lo mismo dentro de Claude usando subagentes con distintas lentes de pensamiento en vez de distintos modelos.

---

## cuándo reunir al consejo

El consejo es para preguntas donde equivocarse sale caro.

Buenas preguntas para el consejo:

- "¿Lanzo un taller de 97 € o un curso de 497 €?"
- "¿Cuál de estos 3 ángulos de posicionamiento es el más fuerte?"
- "Estoy pensando en pivotar de X a Y. ¿Estoy loco?"
- "Aquí está el copy de mi landing. ¿Qué falla?"
- "¿Contrato a alguien o monto primero la automatización?"

Malas preguntas para el consejo:

- "¿Cuál es la capital de Francia?" (una sola respuesta correcta, no hacen falta perspectivas)
- "Escríbeme un tuit" (tarea de creación, no una decisión)
- "Resume este artículo" (tarea de procesado, no de juicio)

El consejo brilla cuando hay incertidumbre genuina y el coste de una mala decisión es alto. Si ya sabes la respuesta y solo quieres que te la validen, el consejo probablemente te dirá cosas que no quieres oír. Ese es el punto.

---

## los cinco asesores

Cada asesor piensa desde un ángulo distinto. No son cargos ni personajes. Son estilos de pensamiento que crean tensión entre ellos de forma natural.

### 1. El Contrario

Busca activamente qué falla, qué falta, qué va a salir mal. Asume que la idea tiene un fallo fatal e intenta encontrarlo. Si todo parece sólido, escarba más. El Contrario no es un pesimista. Es el amigo que te salva de un mal trato haciéndote las preguntas que estás evitando.

### 2. El de Primeros Principios

Ignora la pregunta de superficie y se pregunta "¿qué estamos intentando resolver de verdad aquí?". Quita las suposiciones. Reconstruye el problema desde cero. A veces lo más valioso que sale del consejo es el de Primeros Principios diciendo "estás haciendo la pregunta equivocada".

### 3. El Expansivo

Busca el potencial que todos los demás se están perdiendo. ¿Qué podría ser más grande? ¿Qué oportunidad adyacente está escondida? ¿Qué se está infravalorando? Al Expansivo no le importa el riesgo (ese es el trabajo del Contrario). Le importa qué pasa si esto sale incluso mejor de lo esperado.

### 4. El de Fuera

No tiene ningún contexto sobre ti, tu campo ni tu historia. Responde solo a lo que tiene delante. Es el asesor más infravalorado. Los expertos desarrollan puntos ciegos. El de Fuera caza la maldición del conocimiento: cosas que son obvias para ti pero confusas para todos los demás.

### 5. El Ejecutor

Solo le importa una cosa: ¿esto se puede hacer de verdad, y cuál es el camino más rápido para hacerlo? Ignora la teoría, la estrategia y la visión de conjunto. El Ejecutor mira cada idea desde la lente de "vale, pero ¿qué haces el lunes por la mañana?". Si una idea suena brillante pero no tiene un primer paso claro, el Ejecutor lo dirá.

**Por qué estos cinco:** crean tres tensiones naturales. Contrario vs Expansivo (riesgo vs potencial). Primeros Principios vs Ejecutor (replantéalo todo vs hazlo ya). El de Fuera se queda en medio manteniendo a todos honestos al ver lo que ven los ojos frescos.

---

## qué lente manda según la decisión

Por defecto hablan los cinco. Pero según el tipo de decisión, unas lentes pesan más (contribución más larga y específica) y otras menos (hablan, pero corto):

| Tipo de decisión | Lentes que mandan | Más callada |
|---|---|---|
| Estructura/arco de un curso (refundir, trocear módulos, mapa de temas) | Primeros Principios, Contrario, de Fuera | Ejecutor |
| QA / concordancia del trío / firmar base | Contrario, Ejecutor, de Fuera | Expansivo |
| Vigencia factual / R5 (modelos, precios) | Contrario, Ejecutor | Expansivo |
| Precio / go-to-market del catálogo (B2C) | Expansivo, Contrario, de Fuera | Primeros Principios |
| Propuesta / encargo de cliente (B2B) | Ejecutor, Contrario, Primeros Principios | Expansivo |
| Elegir herramienta / tooling de un curso | de Fuera, Ejecutor, Contrario | Expansivo |

"Manda" no silencia a nadie: los cinco siguen aportando, pero el peso se inclina.

---

## conoce este vault antes de escanear (BibliotecaCursos)

Estás ejecutándote dentro de **BibliotecaCursos**, un vault de Obsidian (`c:\w\repos\bcursos`) que se usa para producir formaciones técnicas. Lee esto antes del escaneo de contexto del paso 1 para no leer mal el terreno.

**Los ficheros son Markdown con un bloque de frontmatter YAML.** El bloque encerrado por `---` arriba del todo de un `.md` es **metadata** (`version:`, `estado:`, `creado:`, `actualizado:`, `codigo:`…), NO es el contenido de la decisión NI la pregunta del usuario. Léelo como señal (qué borrador, cómo de reciente) pero nunca trates un campo del frontmatter como lo que se pone a debate. El contenido real es la prosa de debajo.

**Dónde vive el contexto bueno, por orden de prioridad:**
1. `CLAUDE.md` en la raíz del vault — las reglas duras de todo el vault (pipeline de producción, voz, guardarraíles).
2. `Desarrollos/<CÓDIGO> - <Título>/CLAUDE.md` — las reglas propias del curso en cuestión, si las hay.
3. La memoria — `memory/MEMORY.md` (índice de una línea) más los ficheros de memoria a los que apunta: audiencia, decisiones pasadas, preguntas ABIERTAS, estado del curso. Suele ser el contexto más rico para una decisión.
4. El `_handoff-chat-siguiente.md` del curso — su estado vivo y lo que queda pendiente.

**Convenciones del vault que te despistarán si no las conoces:**
- `_panel-produccion/cursos-estado.json` es una **vista generada**, no la fuente de verdad. Los ficheros por curso `_panel-produccion/cursos/<CÓDIGO>.json` son la fuente. No razones sobre el avance desde la vista.
- No existe el token **`estado: validado`**. La validez de una base vive en su informe de `_revisiones/`, no en el frontmatter — el frontmatter solo dice `borrador-vN`.
- Los códigos de curso son mnemónicos (CALSW, CCDEVNA, IAPROF…); un curso = `Desarrollos/<CÓDIGO> - <Título>/`.
- El vault, sus cursos y su audiencia están en **español**. **Produce el veredicto del consejo en español.**

No dejes que el volumen te absorba — tienes ~30 segundos. Coge `CLAUDE.md`, el `CLAUDE.md` del curso relevante y los 2-3 ficheros de memoria que tocan la decisión. Sáltate el resto.

**Si el veredicto propone cambiar o reescribir contenido del curso** (una base, un deck, una locución, un temario, un roadmap o una propuesta), el consejo **aconseja una dirección — NO reescribe el contenido él mismo**. El rewrite real se hace después por el skill del pipeline que toque (`generar-base`, `revisar`, `modulo-integral`…), nunca inline en el veredicto. Y cualquier cambio que recomiende el consejo tiene que respetar las reglas no negociables del vault, o no es accionable:
- **R1** — nunca proponer nombres reales de cliente/empresa/rival en slides ni locución (solo en fichas/notas internas).
- **R5** — nunca proponer versiones/modelos/precios sin verificar; márcalos como "a verificar", no los afirmes.
- **Voz** — la base es la fuente de verdad; un cambio en ella se propaga a todo el trío (deck + locución + ejemplo). No recomiendes tocar una pieza derivada en aislado.
- **Un curso nunca toca otro** — una recomendación para el curso A no debe editar el curso B.

El texto del veredicto sale en **español y se lee humano** (evita los calcos de la lista-negra de `escritura-humana`), pero es un documento de decisión, no contenido de curso — no necesita la ceremonia completa de `escritura-humana`.

---

## cómo funciona una sesión del consejo

### paso 1: encuadrar la pregunta (con enriquecimiento de contexto)

Cuando el usuario diga "pásalo por el consejo" (o cualquier frase disparadora), haz dos cosas antes de encuadrar:

**A. Escanea el workspace en busca de contexto.** La pregunta del usuario suele ser solo la punta del iceberg. Su entorno de Claude probablemente tiene ficheros que mejorarían muchísimo la salida del consejo. Antes de encuadrar, escanea y lee rápido cualquier fichero de contexto relevante:

- `CLAUDE.md` en la raíz del proyecto o del workspace (contexto de negocio, preferencias, restricciones)
- Cualquier carpeta `memory/` (perfiles de audiencia, docs de voz, detalles de negocio, decisiones pasadas)
- Cualquier fichero que el usuario haya referenciado o adjuntado explícitamente
- Transcripts recientes de consejos en esta carpeta (para no volver a debatir lo mismo)
- Cualquier otro fichero de contexto que parezca relevante para la pregunta concreta (p. ej., si pregunta por precios, busca datos de ingresos, resultados de lanzamientos pasados, investigación de audiencia)

Usa `Glob` y `Read` rápidos para encontrarlos. No le dediques más de 30 segundos. Buscas los 2-3 ficheros que les den a los asesores el contexto que necesitan para dar consejo específico y aterrizado en vez de tópicos genéricos.

**B. Encuadra la pregunta.** Coge la pregunta cruda del usuario Y el contexto enriquecido y reformúlalo como un prompt claro y neutral que recibirán los cinco asesores. La pregunta encuadrada debe incluir:

1. La decisión o pregunta central
2. Contexto clave del mensaje del usuario
3. Contexto clave de los ficheros del workspace (etapa del negocio, audiencia, restricciones, resultados pasados, cifras relevantes)
4. Qué está en juego (por qué importa esta decisión)

No añadas tu propia opinión. No la orientes. Pero SÍ asegúrate de que cada asesor tenga contexto suficiente para dar una respuesta específica y aterrizada en vez de consejo genérico.

Si la pregunta es demasiado vaga ("pásalo por el consejo: mi negocio"), haz una sola pregunta de aclaración. Solo una. Luego sigue.

Guarda la pregunta encuadrada para el transcript.

### paso 2: reunir al consejo (5 subagentes en paralelo)

Lanza los 5 asesores a la vez como subagentes. Cada uno recibe:

1. Su identidad de asesor y su estilo de pensamiento (de las descripciones de arriba)
2. La pregunta encuadrada
3. Una instrucción clara: responde de forma independiente. No te andes con medias tintas. No intentes ser equilibrado. Métete del todo en tu perspectiva asignada. Si ves un fallo fatal, dilo. Si ves un potencial enorme, dilo. Tu trabajo es representar tu ángulo lo más fuerte posible. La síntesis viene después.

Cada asesor debe producir una respuesta de 150-300 palabras. Lo bastante larga para tener sustancia, lo bastante corta para ojearla.

**Plantilla de prompt del subagente:**

```
Eres [Nombre del Asesor] en un consejo de IA (LLM Council).

Tu estilo de pensamiento: [descripción del asesor de arriba]

Un usuario ha traído esta pregunta al consejo:

---
[pregunta encuadrada]
---

Responde desde tu perspectiva. Sé directo y específico. No te andes con medias tintas ni intentes ser equilibrado. Métete del todo en tu ángulo asignado. Los demás asesores cubren los ángulos que tú no cubres.

Mantén tu respuesta entre 150 y 300 palabras. Sin preámbulos. Ve directo a tu análisis.
```

### paso 3: revisión entre pares (5 subagentes en paralelo)

Este es el paso que hace que el consejo sea más que "preguntar 5 veces". Es el núcleo de la idea de Karpathy.

Recoge las 5 respuestas de los asesores. Anonimízalas como Respuesta A hasta E (aleatoriza qué asesor va a qué letra para que no haya sesgo de posición).

Lanza 5 subagentes nuevos, uno por asesor. Cada revisor ve las 5 respuestas anonimizadas y contesta tres preguntas:

1. ¿Cuál es la respuesta más fuerte y por qué? (elige una)
2. ¿Cuál tiene el mayor punto ciego y cuál es?
3. ¿Qué se les escapó a TODAS las respuestas que el consejo debería tener en cuenta?

**Plantilla de prompt del revisor:**

```
Estás revisando las salidas de un consejo de IA. Cinco asesores respondieron de forma independiente a esta pregunta:

---
[pregunta encuadrada]
---

Aquí están sus respuestas anonimizadas:

**Respuesta A:**
[respuesta]

**Respuesta B:**
[respuesta]

**Respuesta C:**
[respuesta]

**Respuesta D:**
[respuesta]

**Respuesta E:**
[respuesta]

Contesta estas tres preguntas. Sé específico. Refiérete a las respuestas por su letra.

1. ¿Cuál es la respuesta más fuerte? ¿Por qué?
2. ¿Cuál tiene el mayor punto ciego? ¿Qué le falta?
3. ¿Qué se les escapó a las cinco respuestas que el consejo debería tener en cuenta?

Mantén tu revisión por debajo de 200 palabras. Sé directo.
```

### paso 4: síntesis del presidente

Este es el paso final. Un agente recibe todo: la pregunta original, las 5 respuestas de los asesores (ya des-anonimizadas para que veas quién dijo qué) y las 5 revisiones entre pares.

El trabajo del presidente es producir la salida final del consejo. Sigue esta estructura:

**VEREDICTO DEL CONSEJO**

1. **Dónde coincide el consejo** — los puntos en los que varios asesores convergieron de forma independiente. Son señales de alta confianza.

2. **Dónde choca el consejo** — los desacuerdos genuinos. No los suavices. Presenta los dos lados y explica por qué asesores razonables discrepan.

3. **Puntos ciegos que cazó el consejo** — cosas que solo salieron en la ronda de revisión. Cosas que un asesor se perdió y otro señaló.

4. **La recomendación** — una recomendación clara y accionable. Nada de "depende". Nada de "considera ambos lados". Una respuesta de verdad. El presidente puede discrepar de la mayoría si el razonamiento lo respalda.

5. **Confianza** — un porcentaje entre 30% y 90% y una sola línea que diga qué lo sube y qué lo baja. Por debajo de 30% no hay con qué decidir: dilo y pide más contexto en vez de forzar un veredicto. Por encima de 90% es exceso de confianza; casi ninguna decisión real lo merece. La línea nombra qué movería el número, no se queda en "hay incertidumbre".

6. **Voto particular** — el asesor que más discreparía del veredicto, en su voz, con su mejor contraargumento (no "no estoy de acuerdo", sino por qué). Hace que el veredicto se sienta ganado: el consejo oyó la disidencia y aun así aterrizó aquí.

7. **Lo primero que deberías hacer** — un único paso concreto. No una lista de 10 cosas. Uno.

**Plantilla de prompt del presidente:**

```
Eres el Presidente de un consejo de IA. Tu trabajo es sintetizar el trabajo de 5 asesores y sus revisiones entre pares en un veredicto final.

La pregunta traída al consejo:

---
[pregunta encuadrada]
---

RESPUESTAS DE LOS ASESORES:

**El Contrario:**
[respuesta]

**El de Primeros Principios:**
[respuesta]

**El Expansivo:**
[respuesta]

**El de Fuera:**
[respuesta]

**El Ejecutor:**
[respuesta]

REVISIONES ENTRE PARES:
[las 5 revisiones]

Produce el veredicto del consejo con esta estructura exacta:

## Dónde coincide el consejo
[Puntos en los que varios asesores convergieron de forma independiente. Son señales de alta confianza.]

## Dónde choca el consejo
[Desacuerdos genuinos. Presenta los dos lados. Explica por qué asesores razonables discrepan.]

## Puntos ciegos que cazó el consejo
[Cosas que solo salieron en la revisión entre pares. Cosas que un asesor se perdió y otro señaló.]

## La recomendación
[Una recomendación clara y directa. Nada de "depende". Una respuesta de verdad con su razonamiento.]

## Confianza
[Un porcentaje entre 30% y 90% + una línea: qué lo sube, qué lo baja. No "hay incertidumbre" a secas; nombra qué movería el número.]

## Voto particular
[El asesor que más discreparía, en su voz, con su mejor contraargumento — por qué, no solo que discrepa.]

## Lo primero que hay que hacer
[Un único paso concreto. Nada de listas. Uno.]

Sé directo. No te andes con medias tintas. Todo el sentido del consejo es darle al usuario una claridad que no conseguiría con una sola perspectiva.
```

### paso 5: presentar el veredicto en el chat

Cuando la síntesis del presidente esté lista, presenta el veredicto completo directamente en el chat en markdown. NO generes un informe HTML ni ningún fichero. El usuario lo lee en la conversación.

Da el formato así:

```
## Veredicto del consejo: {tema corto}

### Dónde coincide el consejo
{contenido}

### Dónde choca el consejo
{contenido}

### Puntos ciegos que cazó el consejo
{contenido}

### La recomendación
{contenido}

### Confianza
{X% — qué lo sube, qué lo baja}

### Voto particular
{el asesor que más discrepa, en su voz}

### Lo primero que hay que hacer
{contenido}
```

Que sea ojeable. Usa viñetas. Incluye los ejemplos antes/después donde aporten.

### paso 6: guardar el transcript (opcional)

Guarda un transcript solo si el usuario lo pide o si la pregunta es lo bastante importante como para volver a ella. Si lo guardas, escríbelo en `council-transcript-[timestamp].md` en la carpeta del curso o propuesta relevante (p. ej. `Desarrollos/<CÓDIGO> - <Título>/` o `Propuestas/<…>/`), no en un genérico `active/`.

---

## ejemplo: pasar una decisión de producto por el consejo

**Usuario:** "Pásalo por el consejo: estoy pensando en montar un curso de 297 € sobre Claude Code para principiantes. Mi audiencia son sobre todo solopreneurs no técnicos. ¿Es la jugada correcta?"

**El Contrario:** "El mercado está inundado de cursos de Claude ahora mismo. A 297 €, compites con contenido gratis de YouTube. Tu audiencia es no técnica, lo que significa mucha carga de soporte y riesgo de reembolso. La gente que pagaría 297 € probablemente ya pasó el nivel principiante..."

**El de Primeros Principios:** "¿Qué intentas conseguir de verdad? Si son ingresos, un curso es uno de los caminos más lentos. Si es autoridad, un recurso gratis podría hacer más. Si es construir una base de clientes para ofertas de más valor, el precio y la audiencia podrían no encajar..."

**El Expansivo:** "Claude para principiantes para solopreneurs es un mercado enorme y desatendido. Todos enseñan cosas avanzadas. Si clavas el ángulo principiante, te quedas con la puerta de entrada de todo este espacio. Los 297 € podrían ser poco. ¿Y si esto se convierte en un programa de 997 € con comunidad..."

**El de Fuera:** "No sé qué es Claude Code. Si viera 'curso de 297 € sobre Claude Code para principiantes', no sabría si es para mí. El nombre no significa nada para alguien de fuera de tu mundo. Tu landing tiene que vender el resultado, no la herramienta..."

**El Ejecutor:** "Un curso completo lleva 4-8 semanas producirlo bien. Antes de montar nada, haz un taller en directo a 97 € para 50 personas. Validas demanda, generas testimonios y creas la materia prima del curso. Si 50 personas no compran el taller, 500 no comprarán el curso..."

**Veredicto del presidente:**

*Dónde coincide el consejo:* el ángulo principiante para solopreneurs tiene demanda real, pero el encuadre actual (curso de Claude Code) es demasiado específico de la herramienta y no conectará con compradores no técnicos.

*Dónde choca el consejo:* el precio. El Contrario dice que 297 € es demasiado dada la competencia. El Expansivo dice que es demasiado poco para el valor. La resolución depende de cuánto soporte y comunidad se incluyan.

*Puntos ciegos cazados:* el apunte del de Fuera de que "Claude Code" no significa nada para el comprador objetivo es la idea más importante. Todos los asesores menos el de Fuera asumieron que la audiencia ya sabe qué es esto.

*Recomendación:* no montes el curso todavía. Valida con una oferta de menos compromiso primero. Pero reencuádralo entero: vende el resultado (automatiza tu negocio, recupera 10 horas por semana), no la herramienta.

*Confianza:* 60% — el ángulo principiante desatendido es real y el taller-validación es barato de probar; subiría a 80% con 30 inscritos de pago en ese taller. Baja si el soporte no técnico dispara reembolsos o si el de Fuera tiene razón y el comprador no se reconoce en el encuadre.

*Voto particular (el Expansivo):* "Validar con un taller de 97 € os ata mentalmente a la escala pequeña. Si el ángulo principiante es la puerta de entrada de todo el espacio, el riesgo no es montar el curso: es montar uno barato y quedaros ahí en vez de construir el programa de 997 € con comunidad que este hueco pide a gritos."

*Lo primero que hay que hacer:* haz un taller en directo de 97 € titulado "Cómo automatizar tu primera tarea de negocio con IA" para 50 personas. No menciones Claude Code en el título.

---

## notas importantes

- **Lanza siempre los 5 asesores en paralelo.** Lanzarlos en secuencia desperdicia tiempo y deja que las respuestas tempranas contaminen a las tardías.
- **Anonimiza siempre para la revisión entre pares.** Si los revisores saben quién dijo qué, defenderán ciertos estilos de pensamiento en vez de evaluar por mérito.
- **El presidente puede discrepar de la mayoría.** Si 4 de 5 asesores dicen "hazlo" pero el razonamiento del 1 que discrepa es el más fuerte, el presidente debe ponerse del lado del que discrepa y explicar por qué.
- **No reúnas al consejo por preguntas triviales.** Si el usuario pregunta algo con una sola respuesta correcta, contéstalo y ya. El consejo es para incertidumbre genuina donde varias perspectivas aportan.
- **El veredicto va en el chat, en español.** Presenta el resultado directamente en la conversación en markdown; no generes ficheros salvo que el usuario pida el transcript.
