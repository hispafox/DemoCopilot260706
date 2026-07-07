# DemoCopilot260706

Repositorio base para las clases de formacion de GitHub Copilot.

## Objetivo

Este repo sirve como punto de partida para practicar:

- Flujo Git basico (`clone`, `branch`, `commit`, `push`, `pull request`).
- Uso de GitHub Copilot en tareas de desarrollo.
- Buenas practicas de organizacion de cambios y documentacion.

## Estructura actual

- `.github/copilot-instructions.md`: reglas y convenciones para el asistente.
- `documentacion/instalar-gh.md`: instalacion de GitHub CLI.
- `documentacion/conectar-mcp-github.md`: guia para conectar GitHub MCP en este repo.

## Sistema oficial de generacion de documentacion

Desde ahora, la generacion de informes usa un pipeline unico para que el resultado salga bien a la primera:

- Markdown con estructura estandar.
- Validacion previa obligatoria.
- DOCX profesional con portada, tabla de contenido, estilos y paginacion.

### Scripts

- `scripts/documentacion_pipeline.py`
	- `init`: crea markdown desde plantilla base.
	- `validate`: valida estructura minima del markdown.
	- `build`: valida y genera docx final (sobrescribe por defecto `<input>.docx`).
- `scripts/md_to_docx.py`
	- Conversor oficial Markdown -> DOCX con estilo profesional por defecto.

### Plantilla base

- `documentacion/plantillas/informe-validacion-analisis-prd.md`

### Uso rapido

1. Crear markdown base:

```powershell
c:/w/DemoCopilot260706/.venv/Scripts/python.exe scripts/documentacion_pipeline.py init --output documentacion/informes/nuevo-informe.md
```

2. Validar estructura:

```powershell
c:/w/DemoCopilot260706/.venv/Scripts/python.exe scripts/documentacion_pipeline.py validate --input documentacion/informes/nuevo-informe.md
```

3. Generar DOCX final (sobrescribe por defecto):

```powershell
c:/w/DemoCopilot260706/.venv/Scripts/python.exe scripts/documentacion_pipeline.py build --input documentacion/informes/nuevo-informe.md
```

### Nota de indice en Word

La tabla de contenido se inserta automaticamente. En Word, si no aparecen paginas al abrir por primera vez, actualizar campos del indice.

## Siguientes pasos recomendados para clase

1. Crear ramas por ejercicio.
2. Abrir pull requests entre alumnos.
3. Revisar cambios con Copilot y revision manual.
4. Mantener el README actualizado con cada practica.
