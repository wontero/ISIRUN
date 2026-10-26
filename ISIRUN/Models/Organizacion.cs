using System.ComponentModel.DataAnnotations;

namespace ISIRUN.Models;

public class Organizacion
{
    [Key]
    public int IdOrganizacion { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(500)]
    public string Actividad { get; set; } = "";

    [Required, StringLength(254), EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(500)]
    public string ContrasenaHash { get; set; } = "";

    public ICollection<OrgCarrera> OrgCarreras { get; set; } = new List<OrgCarrera>();
    public ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
}
