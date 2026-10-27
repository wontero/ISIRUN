namespace ISIRUN.ViewModels;

// Reporte 1: inscripciones y recaudación. Todos los totales vienen calculados desde el controlador.
public class ReporteInscripcionesViewModel
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public string? Estado { get; set; }

    // Columnas de categoría de la tabla (los rangos de todas las carreras filtradas).
    public List<string> Rangos { get; set; } = [];
    public List<FilaInscripcionesViewModel> Filas { get; set; } = [];

    // Mismo orden que Rangos.
    public List<BarraCategoriaViewModel> TotalesPorCategoria { get; set; } = [];
    public int TotalInscritos { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PromedioPorCarrera { get; set; }
}

public class FilaInscripcionesViewModel
{
    public string Carrera { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "";

    // Inscritos en cada rango de ReporteInscripcionesViewModel.Rangos (0 si la carrera no lo tiene).
    public List<int> InscritosPorRango { get; set; } = [];
    public int Inscritos { get; set; }
    public decimal Recaudado { get; set; }
}

public class BarraCategoriaViewModel
{
    public string Rango { get; set; } = "";
    public int Inscritos { get; set; }

    // Ancho de la barra: % respecto de la categoría con más inscritos.
    public int PorcentajeBarra { get; set; }

    // % del total de inscritos.
    public int Participacion { get; set; }
}

// Reporte 2: rendimiento por categoría o equipo en una carrera terminada.
public class ReporteRendimientoViewModel
{
    public List<CarreraOpcionViewModel> Carreras { get; set; } = [];
    public int? CarreraSeleccionada { get; set; }
    public string NombreCarrera { get; set; } = "";
    public bool PorEquipo { get; set; }

    public List<FilaRendimientoViewModel> Filas { get; set; } = [];
    public int TotalFinalistas { get; set; }
    public int TotalPodios { get; set; }
    public string MejorTiempo { get; set; } = "—";
    public string MejorNombre { get; set; } = "";
    public string TiempoPromedio { get; set; } = "—";
    public string RitmoPromedio { get; set; } = "—";
}

public class CarreraOpcionViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}

public class FilaRendimientoViewModel
{
    public string Grupo { get; set; } = "";
    public int Finalistas { get; set; }
    public int Podios { get; set; }
    public string MejorTiempo { get; set; } = "";
    public string MejorNombre { get; set; } = "";
    public string TiempoPromedio { get; set; } = "";
    public string Ritmo { get; set; } = "";
}
