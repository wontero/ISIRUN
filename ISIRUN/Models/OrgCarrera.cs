using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class OrgCarrera
{
    public int IdCarrera { get; set; }
    public int IdOrganizacion { get; set; }

    [Column(TypeName = "date")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Range(1, int.MaxValue)]
    public int TamanoEquipo { get; set; }

    public bool EsCreadora { get; set; }

    [ForeignKey(nameof(IdCarrera))]
    public Carrera Carrera { get; set; } = null!;

    [ForeignKey(nameof(IdOrganizacion))]
    public Organizacion Organizacion { get; set; } = null!;
}
