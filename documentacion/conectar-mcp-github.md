# Conectar GitHub MCP en este repositorio

Esta guia deja configurado el servidor MCP oficial de GitHub para usarlo desde VS Code dentro de este repositorio.

## Objetivo

Al terminar, podras usar Copilot en modo Agent con herramientas de GitHub para:

- Leer repositorios y archivos.
- Buscar y consultar issues.
- Crear y actualizar issues.
- Crear y revisar pull requests.

Para este repositorio, el remoto actual es:

```text
https://github.com/hispafox/DemoCopilot260706.git
```

## Opcion recomendada

Usar el servidor remoto oficial de GitHub MCP con autenticacion OAuth desde VS Code.

Ventajas de esta opcion:

- No requiere guardar un token en el repositorio.
- La configuracion es minima.
- GitHub hospeda el servidor MCP remoto.

## Requisitos

Antes de empezar, verifica esto:

1. Tienes acceso a GitHub Copilot.
2. Estas usando VS Code 1.101 o superior.
3. Si tu cuenta pertenece a una organizacion con Copilot Business o Enterprise, la politica MCP servers in Copilot debe estar habilitada.

## Configuracion para este repositorio

La forma correcta de compartir la configuracion a nivel de repo es crear el archivo .vscode/mcp.json en la raiz del proyecto.

Contenido recomendado:

```json
{
  "servers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/"
    }
  }
}
```

## Pasos en VS Code

1. Crea la carpeta .vscode en la raiz si todavia no existe.
2. Crea el archivo .vscode/mcp.json.
3. Pega la configuracion anterior y guarda.
4. Abre la paleta de comandos con Ctrl+Shift+P.
5. Ejecuta MCP: List Servers.
6. Verifica que aparezca github.
7. Si no esta arrancado, abre .vscode/mcp.json y pulsa Start sobre el servidor, o arrancalo desde MCP: List Servers.
8. Acepta el dialogo de confianza cuando VS Code lo pida.
9. Si aparece el flujo OAuth, autentica tu cuenta de GitHub en el navegador.

## Verificacion funcional

Despues de arrancar el servidor:

1. Abre Copilot Chat.
2. Cambia el modo a Agent.
3. Abre el selector de herramientas.
4. Comprueba que aparecen herramientas del servidor github.

Pruebas rapidas recomendadas:

```text
Usa GitHub MCP para decirme a que remoto apunta este repositorio.
```

```text
Usa GitHub MCP para listar los issues abiertos de hispafox/DemoCopilot260706.
```

```text
Usa GitHub MCP para crear un issue de prueba con el titulo "Prueba MCP".
```

## Alternativa con PAT

Solo usa esta opcion si no puedes completar OAuth o necesitas forzar una identidad concreta.

No guardes el token en el archivo. Usa un input de VS Code:

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "github_mcp_pat",
      "description": "GitHub Personal Access Token",
      "password": true
    }
  ],
  "servers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/",
      "headers": {
        "Authorization": "Bearer ${input:github_mcp_pat}"
      }
    }
  }
}
```

## Alternativa local con Docker

Si prefieres ejecutar el servidor MCP de GitHub en local, usa Docker y un PAT.

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "github_token",
      "description": "GitHub Personal Access Token",
      "password": true
    }
  ],
  "servers": {
    "github": {
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "-e",
        "GITHUB_PERSONAL_ACCESS_TOKEN",
        "ghcr.io/github/github-mcp-server"
      ],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "${input:github_token}"
      }
    }
  }
}
```

Usa esta variante solo si realmente necesitas un servidor local. Para este repo, la opcion remota con OAuth es mas simple.

## Errores habituales

### El servidor no aparece

- Ejecuta MCP: List Servers.
- Revisa que el archivo sea exactamente .vscode/mcp.json.
- Verifica que el JSON sea valido.

### El servidor aparece pero no arranca

- Asegurate de haber aceptado el dialogo de confianza.
- Reinicia el servidor desde MCP: List Servers.
- Abre la salida del servidor con Show Output.

### No salen herramientas en Copilot Chat

- Cambia a modo Agent.
- Abre el selector de herramientas y habilita github si estuviera desactivado.
- Evita configurar el mismo servidor a la vez en el perfil de usuario y en el workspace.

### OAuth no completa

- Cierra la autenticacion y vuelve a iniciar el servidor.
- Comprueba que VS Code tiene iniciada sesion con GitHub Copilot.
- Si tu organizacion aplica politicas, confirma que MCP esta permitido.

## Recomendacion para este repo

Para una clase o demo, usa la configuracion remota con OAuth en .vscode/mcp.json y evita PATs compartidos.

Si mas adelante quieres versionar la configuracion del repo, puedes guardar solo la variante OAuth. No requiere secretos y sirve como plantilla comun para cualquier persona que abra este workspace.

## Referencias oficiales

- VS Code: Add and manage MCP servers in VS Code
- GitHub Docs: Extending GitHub Copilot Chat with Model Context Protocol servers
- GitHub MCP Server: github/github-mcp-server