using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class CategoriaEdad
{
    [Key]
    public int IdCategoria { get; set; }

    public int IdCarrera { get; set; }

    [Range(0, 150)]
    public int EdadMinima { get; set; }

    [Range(0, 150)]
    public int EdadMaxima { get; set; }

    [Required, StringLength(500)]
    public string Premios { get; set; } = "";

    [ForeignKey(nameof(IdCarrera))]
    public Carrera Carrera { get; set; } = null!;

    public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
}
