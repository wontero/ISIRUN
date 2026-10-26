using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Sponsor
{
    public int IdCarrera { get; set; }
    public int IdOrganizacion { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal MontoAportado { get; set; }

    [Required, StringLength(150)]
    public string TipoParticipacion { get; set; } = "";

    [ForeignKey(nameof(IdCarrera))]
    public Carrera Carrera { get; set; } = null!;

    [ForeignKey(nameof(IdOrganizacion))]
    public Organizacion Organizacion { get; set; } = null!;
}
