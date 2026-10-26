using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Equipo
{
    [Key]
    public int IdEquipo { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime FechaCreacion { get; set; } = DateTime.Today;

    public ICollection<Corredor> Corredores { get; set; } = new List<Corredor>();
}
