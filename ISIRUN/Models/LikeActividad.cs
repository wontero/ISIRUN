using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class LikeActividad
{
    public int IdActividad { get; set; }
    public int IdCorredor { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    [ForeignKey(nameof(IdActividad))]
    public Actividad Actividad { get; set; } = null!;

    public Corredor Corredor { get; set; } = null!;
}
