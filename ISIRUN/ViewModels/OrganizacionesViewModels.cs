using System.ComponentModel.DataAnnotations;

namespace ISIRUN.ViewModels;

public class OrganizacionesIndexViewModel
{
    public List<OrganizacionResumenViewModel> Organizaciones { get; set; } = [];
    public OrganizacionDetalleViewModel? Seleccionada { get; set; }
}

public class OrganizacionResumenViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Actividad { get; set; } = "";
}

public class OrganizacionDetalleViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Actividad { get; set; } = "";
    public List<CarreraVinculoOrgViewModel> Organiza { get; set; } = [];
    public List<CarreraVinculoOrgViewModel> Patrocina { get; set; } = [];
}

public class CarreraVinculoOrgViewModel
{
    public int IdCarrera { get; set; }
    public string Carrera { get; set; } = "";
    public string Detalle { get; set; } = "";
}

public class PerfilOrganizacionViewModel
{
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Nombre { get; set; } = "";

    [Display(Name = "Actividad")]
    [Required(ErrorMessage = "La actividad es obligatoria.")]
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string Actividad { get; set; } = "";
}

public class VinculosViewModel
{
    public int IdCarrera { get; set; }
    public string NombreCarrera { get; set; } = "";
    public List<VinculoItemViewModel> Vinculadas { get; set; } = [];
    public List<OrganizacionOpcionViewModel> OrganizacionesDisponibles { get; set; } = [];
    public List<string> TiposSponsor { get; set; } =
    [
        "Sponsor principal",
        "Indumentaria",
        "Hidratación",
        "Apoyo médico",
        "Premios"
    ];
}

public class VinculoItemViewModel
{
    public int IdOrganizacion { get; set; }
    public string Nombre { get; set; } = "";
    public string Rol { get; set; } = "";
    public string Detalle { get; set; } = "";
    public bool EsCreadora { get; set; }
}

public class OrganizacionOpcionViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}
