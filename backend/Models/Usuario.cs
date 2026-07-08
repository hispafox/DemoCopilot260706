using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Email { get; set; }

    public int DepartamentoId { get; set; }

    public Departamento Departamento { get; set; } = null!;

    public int SedeId { get; set; }

    public Sede Sede { get; set; } = null!;

    public int PoblacionId { get; set; }

    public Poblacion Poblacion { get; set; } = null!;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
