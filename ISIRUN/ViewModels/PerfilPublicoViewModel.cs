using ISIRUN.Helpers;

namespace ISIRUN.ViewModels;

// Lo que cualquiera puede ver de un corredor (/Corredor/Ver/5).
// PerfilViewModel (Mi perfil) hereda de esta clase y le agrega lo privado.
public class PerfilPublicoViewModel
{
    public int IdCorredor { get; set; }
    public string Nombre { get; set; } = "";
    public string Iniciales => IsirunHelpers.Iniciales(Nombre);
    public DateTime FechaRegistro { get; set; }
    public int Edad { get; set; }
    public int? IdEquipo { get; set; }
    public string? Equipo { get; set; }

    public decimal KmsTotales { get; set; }
    public int CarrerasCorridas { get; set; }
    public int? MejorRanking { get; set; }

    // true si el usuario logueado ya lo tiene como amigo (solo se usa en el perfil público).
    public bool EsAmigo { get; set; }

    public List<ResultadoPerfilViewModel> Resultados { get; set; } = [];
    public List<ActividadResumenViewModel> Actividades { get; set; } = [];
}

public class ResultadoPerfilViewModel
{
    public int IdCarrera { get; set; }
    public string Carrera { get; set; } = "";
    public int EdadMinima { get; set; }
    public int EdadMaxima { get; set; }
    public string Categoria => $"{EdadMinima}–{EdadMaxima}";
    public TimeSpan Tiempo { get; set; }
    public int Ranking { get; set; }
}

public class ActividadResumenViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public DateTime Fecha { get; set; }
    public decimal Km { get; set; }
    public TimeSpan Tiempo { get; set; }
    public string Tipo { get; set; } = "";
    public int Likes { get; set; }
}
