using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class PlantillaTarea : IValidatableObject
{
    private string _titulo = string.Empty;

    public int Id { get; set; }

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

    public Categoria? Categoria { get; set; }

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
