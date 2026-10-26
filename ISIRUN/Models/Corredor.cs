using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Corredor
{
    [Key]
    public int IdCorredor { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(254), EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(500)]
    public string ContrasenaHash { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime FechaNac { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(10,2)")]
    public decimal KmsTotales { get; set; }

    public int? IdEquipo { get; set; }

    public Equipo? Equipo { get; set; }
    public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
    public ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
    public ICollection<Amistad> Amigos { get; set; } = new List<Amistad>();
}
