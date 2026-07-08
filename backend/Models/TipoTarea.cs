using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class TipoTarea
{
    public int Id { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool EstaActivo { get; set; } = true;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
