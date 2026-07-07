using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Tarea
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    public bool EstaCompletada { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVencimiento { get; set; }

    public string? Notas { get; set; }

    public int? CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}
