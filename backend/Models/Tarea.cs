using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Tarea
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

    public bool EstaCompletada { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVencimiento { get; set; }

    public string? Notas { get; set; }

    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Normal;

    public bool EsRepetitiva { get; set; }

    public TipoRecurrencia? TipoRecurrencia { get; set; }

    public DateTime? ProximaRecurrencia { get; set; }

    public int? PlantillaTareaId { get; set; }

    public PlantillaTarea? PlantillaTarea { get; set; }

    public int? CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public int? UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

    public int TipoTareaId { get; set; }

    public TipoTarea? TipoTarea { get; set; }
}
