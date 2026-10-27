using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Actividad
{
    [Key]
    public int IdActividad { get; set; }

    public int IdCorredor { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [StringLength(1000)]
    public string Descripcion { get; set; } = "";

    [Column(TypeName = "decimal(6,2)")]
    [Range(typeof(decimal), "0.01", "9999.99")]
    public decimal DistanciaKm { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan Tiempo { get; set; }

    [Required, StringLength(50)]
    public string Tipo { get; set; } = "";

    [StringLength(1000)]
    public string Notas { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [ForeignKey(nameof(IdCorredor))]
    public Corredor Corredor { get; set; } = null!;

    public ICollection<LikeActividad> Likes { get; set; } = new List<LikeActividad>();
}
