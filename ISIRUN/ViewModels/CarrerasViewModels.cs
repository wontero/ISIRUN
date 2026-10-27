namespace ISIRUN.ViewModels;

public class CarrerasIndexViewModel
{
    public string Estado { get; set; } = "abiertas";
    public string? Distancia { get; set; }
    public string? Q { get; set; }
    public List<CarreraTarjetaViewModel> Carreras { get; set; } = [];
}

public class CarreraTarjetaViewModel
{
    public int Id { get; set; }
    public string Distancia { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Organiza { get; set; } = "";
    public string Info { get; set; } = "";
    public string Precio { get; set; } = "";
}

public class CarreraDetalleViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string DistanciaCorta { get; set; } = "";
    public string DistanciaTexto { get; set; } = "";
    public string FechaTexto { get; set; } = "";
    public string PrecioTexto { get; set; } = "";
    public decimal Precio { get; set; }
    public bool EstaInscrito { get; set; }
    public List<CategoriaDetalleViewModel> Categorias { get; set; } = [];
    public List<OrganizacionVinculoViewModel> Organizadoras { get; set; } = [];
    public List<SponsorDetalleViewModel> Sponsors { get; set; } = [];
}

public class CategoriaDetalleViewModel
{
    public string Rango { get; set; } = "";
    public string Premios { get; set; } = "";
    public int Inscritos { get; set; }
}

public class OrganizacionVinculoViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Desde { get; set; } = "";
    public int Equipo { get; set; }
}

public class SponsorDetalleViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Monto { get; set; } = "";
}

public class CarreraResultadosViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string FechaTexto { get; set; } = "";
    public string DistanciaTexto { get; set; } = "";
    public string Organiza { get; set; } = "";
    public int TotalCorredores { get; set; }
    public decimal DistanciaKm { get; set; }
    public int? CategoriaActual { get; set; }
    public List<CategoriaTabViewModel> Categorias { get; set; } = [];
    public List<RankingFilaViewModel> Ranking { get; set; } = [];
    public MiResultadoViewModel? MiResultado { get; set; }
}

public class CategoriaTabViewModel
{
    public int Id { get; set; }
    public string Rango { get; set; } = "";
}

public class RankingFilaViewModel
{
    public int Pos { get; set; }
    public string Corredor { get; set; } = "";
    public string Equipo { get; set; } = "";
    public string Tiempo { get; set; } = "";
    public string Ritmo { get; set; } = "";
    public string Premio { get; set; } = "";
    public bool EsYo { get; set; }
}

public class MiResultadoViewModel
{
    public int Pos { get; set; }
    public string Tiempo { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Ritmo { get; set; } = "";
}
