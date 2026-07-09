using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Contracts;

public class TareaDto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public bool EstaCompletada { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string? Notas { get; set; }

    public PrioridadTarea Prioridad { get; set; }

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public DateTime? ProximaRecurrencia { get; set; }

    public int? PlantillaTareaId { get; set; }

    public int? CategoriaId { get; set; }

    public int? UsuarioId { get; set; }

    public string? UsuarioNombre { get; set; }

    public int TipoTareaId { get; set; }

    public string TipoTareaNombre { get; set; } = string.Empty;
}

public class CrearActualizarTareaRequest : IValidatableObject
{
    private string _titulo = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El titulo no puede estar vacio.")]
    [StringLength(200)]
    public string Titulo
    {
        get => _titulo;
        set => _titulo = (value ?? string.Empty).Trim();
    }

    public bool EstaCompletada { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string? Notas { get; set; }

    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Normal;

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public DateTime? ProximaRecurrencia { get; set; }

    public int? PlantillaTareaId { get; set; }

    public int? CategoriaId { get; set; }

    public int? UsuarioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El tipo de tarea es obligatorio.")]
    public int TipoTareaId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!EsRepetitiva)
        {
            if (TipoRecurrencia is not null)
            {
                yield return new ValidationResult(
                    "TipoRecurrencia debe ser null cuando EsRepetitiva es false.",
                    new[] { nameof(TipoRecurrencia), nameof(EsRepetitiva) });
            }

            if (ProximaRecurrencia is not null)
            {
                yield return new ValidationResult(
                    "ProximaRecurrencia debe ser null cuando EsRepetitiva es false.",
                    new[] { nameof(ProximaRecurrencia), nameof(EsRepetitiva) });
            }
        }

        if (EsRepetitiva && TipoRecurrencia is null)
        {
            yield return new ValidationResult(
                "TipoRecurrencia es obligatorio cuando EsRepetitiva es true.",
                new[] { nameof(TipoRecurrencia), nameof(EsRepetitiva) });
        }
    }
}

public class TipoTareaDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EstaActivo { get; set; }
}

public class CrearActualizarTipoTareaRequest
{
    private string _nombre = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(100)]
    public string Nombre
    {
        get => _nombre;
        set => _nombre = (value ?? string.Empty).Trim();
    }

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool EstaActivo { get; set; } = true;
}

public class PlantillaTareaDto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Notas { get; set; }

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public int? CategoriaId { get; set; }

    public bool EstaActiva { get; set; }
}

public class CrearActualizarPlantillaTareaRequest : IValidatableObject
{
    private string _titulo = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El titulo no puede estar vacio.")]
    [StringLength(200)]
    public string Titulo
    {
        get => _titulo;
        set => _titulo = (value ?? string.Empty).Trim();
    }

    public string? Notas { get; set; }

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public int? CategoriaId { get; set; }

    public bool EstaActiva { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!EsRepetitiva && TipoRecurrencia is not null)
        {
            yield return new ValidationResult(
                "TipoRecurrencia debe ser null cuando EsRepetitiva es false.",
                new[] { nameof(TipoRecurrencia), nameof(EsRepetitiva) });
        }

        if (EsRepetitiva && TipoRecurrencia is null)
        {
            yield return new ValidationResult(
                "TipoRecurrencia es obligatorio cuando EsRepetitiva es true.",
                new[] { nameof(TipoRecurrencia), nameof(EsRepetitiva) });
        }
    }
}

public class UsuarioDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Email { get; set; }

    public int DepartamentoId { get; set; }

    public string DepartamentoNombre { get; set; } = string.Empty;

    public int SedeId { get; set; }

    public string SedeNombre { get; set; } = string.Empty;

    public int PoblacionId { get; set; }

    public string PoblacionNombre { get; set; } = string.Empty;

    public string PoblacionCodigoIsoPais { get; set; } = string.Empty;
}

public class CrearActualizarUsuarioRequest
{
    private string _nombre = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(150)]
    public string Nombre
    {
        get => _nombre;
        set => _nombre = (value ?? string.Empty).Trim();
    }

    [StringLength(200)]
    public string? Email { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El departamento es obligatorio.")]
    public int DepartamentoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La sede es obligatoria.")]
    public int SedeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La poblacion es obligatoria.")]
    public int PoblacionId { get; set; }
}

public class PoblacionDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string CodigoIsoPais { get; set; } = string.Empty;
}

public class CrearActualizarPoblacionRequest
{
    private string _nombre = string.Empty;
    private string _codigoIsoPais = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(100)]
    public string Nombre
    {
        get => _nombre;
        set => _nombre = (value ?? string.Empty).Trim();
    }

    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "El codigo ISO del pais debe tener exactamente 2 caracteres.")]
    public string CodigoIsoPais
    {
        get => _codigoIsoPais;
        set => _codigoIsoPais = (value ?? string.Empty).Trim().ToUpperInvariant();
    }
}

public class DepartamentoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
}

public class CrearActualizarDepartamentoRequest
{
    private string _nombre = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(100)]
    public string Nombre
    {
        get => _nombre;
        set => _nombre = (value ?? string.Empty).Trim();
    }
}

public class SedeDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
}

public class CrearActualizarSedeRequest
{
    private string _nombre = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(100)]
    public string Nombre
    {
        get => _nombre;
        set => _nombre = (value ?? string.Empty).Trim();
    }
}