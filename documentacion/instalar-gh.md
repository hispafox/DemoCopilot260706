# Instalacion de GitHub CLI (`gh`)

Este documento explica como instalar GitHub CLI (`gh`) y dejarlo listo para usar.

## Que es `gh`

`gh` es la herramienta oficial de linea de comandos de GitHub. Te permite crear repositorios, issues, pull requests y ejecutar muchas tareas sin salir de la terminal.

## Windows

### Opcion 1: Winget (recomendada)

```powershell
winget install --id GitHub.cli
```

### Opcion 2: Installer MSI

1. Abre: https://cli.github.com/
2. Descarga el instalador para Windows.
3. Ejecuta el `.msi` y completa el asistente.

## macOS

### Opcion 1: Homebrew (recomendada)

```bash
brew install gh
```

### Opcion 2: MacPorts

```bash
sudo port install gh
```

## Linux

La forma recomendada es usar el gestor de paquetes de tu distribucion.

### Ubuntu / Debian

```bash
(type -p wget >/dev/null || (sudo apt update && sudo apt install wget -y)) \
  && sudo mkdir -p -m 755 /etc/apt/keyrings \
  && wget -qO- https://cli.github.com/packages/githubcli-archive-keyring.gpg \
  | sudo tee /etc/apt/keyrings/githubcli-archive-keyring.gpg > /dev/null \
  && sudo chmod go+r /etc/apt/keyrings/githubcli-archive-keyring.gpg \
  && echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" \
  | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
  && sudo apt update \
  && sudo apt install gh -y
```

### Fedora / RHEL / CentOS (DNF)

```bash
sudo dnf install gh -y
```

### Arch Linux

```bash
sudo pacman -S github-cli
```

## Verificar instalacion

```bash
gh --version
```

Si todo esta correcto, veras una salida similar a:

```text
gh version 2.x.x
```

## Iniciar sesion en GitHub

```bash
gh auth login
```

Recomendaciones durante el asistente:

1. `GitHub.com`
2. `HTTPS`
3. `Login with a web browser`

Luego valida el estado:

```bash
gh auth status
```

## Comandos utiles para empezar

```bash
gh repo create
```

```bash
gh repo view --web
```

```bash
gh pr create
```

```bash
gh issue list
```

## Solucion de problemas rapida

- Si `gh` no se reconoce como comando:
  1. Cierra y abre de nuevo la terminal.
  2. Verifica que la instalacion termino correctamente.
  3. Reinstala con el metodo recomendado para tu sistema.

- Si falla la autenticacion:
  1. Ejecuta `gh auth logout`.
  2. Ejecuta `gh auth login` de nuevo.
  3. Revisa permisos de red o proxy corporativo.

- Si usas varias cuentas:
  1. Revisa cuenta activa con `gh auth status`.
  2. Cambia de cuenta cerrando sesion y volviendo a autenticar.

## Referencias oficiales

- Sitio oficial: https://cli.github.com/
- Manual: https://cli.github.com/manual/
