using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Poblacion
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CodigoIsoPais { get; set; } = "ES";

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
