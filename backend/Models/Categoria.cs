using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Backend.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = string.Empty;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
