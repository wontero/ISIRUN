using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIRUN.Models;

public class Carrera
{
    [Key]
    public int IdCarrera { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [Column(TypeName = "decimal(6,2)")]
    [Range(typeof(decimal), "0.1", "500")]
    public decimal Distancia { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal PrecioInscripcion { get; set; }

    public DateTime Fecha { get; set; }

    [Required, StringLength(20)]
    public string Estado { get; set; } = EstadosCarrera.Abierta;

    public ICollection<CategoriaEdad> Categorias { get; set; } = new List<CategoriaEdad>();
    public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
    public ICollection<OrgCarrera> Organizadoras { get; set; } = new List<OrgCarrera>();
    public ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
}
