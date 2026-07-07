from __future__ import annotations

import argparse
from pathlib import Path
import shutil
import sys

from md_to_docx import convert_markdown_to_docx


REQUIRED_HEADINGS = [
    "## 1. Resumen ejecutivo",
    "## 2. Matriz de trazabilidad",
    "## 3. Hallazgos",
    "## 4. Plan de sincronizacion",
    "## 5. Criterio de cierre",
]


def _read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def _validate_markdown_report(path: Path) -> list[str]:
    errors: list[str] = []

    if not path.exists():
        return [f"No existe el archivo markdown: {path}"]

    text = _read_text(path)
    lines = text.splitlines()

    if not lines:
        errors.append("El markdown esta vacio")
        return errors

    first_non_empty = next((line.strip() for line in lines if line.strip()), "")
    if not first_non_empty.startswith("# "):
        errors.append("El documento debe comenzar con un titulo H1 (# ...)")

    for heading in REQUIRED_HEADINGS:
        if heading not in text:
            errors.append(f"Falta la seccion obligatoria: {heading}")

    matrix_section = "## 2. Matriz de trazabilidad"
    if matrix_section in text and "|---|---|---|---|---|" not in text:
        errors.append("La matriz de trazabilidad debe incluir una tabla con cabecera")

    return errors


def _build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Pipeline oficial para generar documentacion profesional MD + DOCX",
    )
    subparsers = parser.add_subparsers(dest="command", required=True)

    init_parser = subparsers.add_parser(
        "init",
        help="Crea un markdown nuevo desde la plantilla estandar",
    )
    init_parser.add_argument(
        "--output",
        required=True,
        help="Ruta del markdown de salida",
    )
    init_parser.add_argument(
        "--force",
        action="store_true",
        help="Sobrescribe el archivo markdown de salida si ya existe",
    )

    validate_parser = subparsers.add_parser(
        "validate",
        help="Valida estructura minima del markdown",
    )
    validate_parser.add_argument("--input", required=True, help="Ruta del markdown")

    build_parser = subparsers.add_parser(
        "build",
        help="Valida y genera DOCX profesional sobrescribiendo por defecto",
    )
    build_parser.add_argument("--input", required=True, help="Ruta del markdown")
    build_parser.add_argument(
        "--output",
        help="Ruta del docx de salida. Si no se indica, usa <input>.docx",
    )
    build_parser.add_argument(
        "--skip-validation",
        action="store_true",
        help="Omite validacion estructural del markdown",
    )

    return parser


def _cmd_init(output: Path, force: bool) -> int:
    template = Path("documentacion/plantillas/informe-validacion-analisis-prd.md")
    if not template.exists():
        print(f"No existe la plantilla: {template}")
        return 1

    if output.exists() and not force:
        print(f"El archivo ya existe: {output}. Usa --force para sobrescribir.")
        return 1

    output.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(template, output)
    print(f"Markdown base generado: {output}")
    return 0


def _cmd_validate(input_path: Path) -> int:
    errors = _validate_markdown_report(input_path)
    if errors:
        print("Validacion fallida:")
        for error in errors:
            print(f"- {error}")
        return 1

    print("Validacion OK")
    return 0


def _cmd_build(input_path: Path, output_path: Path | None, skip_validation: bool) -> int:
    if not skip_validation:
        validation_errors = _validate_markdown_report(input_path)
        if validation_errors:
            print("No se genero DOCX porque el markdown no cumple la estructura minima:")
            for error in validation_errors:
                print(f"- {error}")
            return 1

    try:
        final_output = convert_markdown_to_docx(input_path, output_path)
    except (FileNotFoundError, ValueError) as error:
        print(error)
        return 1

    print(f"DOCX final generado: {final_output}")
    return 0


def main() -> int:
    parser = _build_parser()
    args = parser.parse_args()

    if args.command == "init":
        return _cmd_init(Path(args.output), args.force)

    if args.command == "validate":
        return _cmd_validate(Path(args.input))

    if args.command == "build":
        output = Path(args.output) if args.output else None
        return _cmd_build(Path(args.input), output, args.skip_validation)

    parser.print_help()
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
