using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Resultado
{
    // La clave compuesta (corredor, carrera) se configura en IsirunContext.
    public int IdCorredor { get; set; }
    public int IdCarrera { get; set; }
    public int IdCategoria { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan? Tiempo { get; set; }

    [Range(1, int.MaxValue)]
    public int? Ranking { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [ForeignKey(nameof(IdCorredor))]
    public Corredor Corredor { get; set; } = null!;

    [ForeignKey(nameof(IdCarrera))]
    public Carrera Carrera { get; set; } = null!;

    public CategoriaEdad Categoria { get; set; } = null!;
}
