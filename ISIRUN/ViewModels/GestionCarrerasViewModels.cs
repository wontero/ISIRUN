using System.ComponentModel.DataAnnotations;

namespace ISIRUN.ViewModels;

// Organizacion/Panel: carreras de la organización logueada.
public class PanelOrganizacionViewModel
{
    public string Nombre { get; set; } = "";
    public string Actividad { get; set; } = "";
    public int InscritosEnAbiertas { get; set; }
    public List<CarreraPanelAbiertaViewModel> Abiertas { get; set; } = [];
    public List<CarreraPanelTerminadaViewModel> Terminadas { get; set; } = [];
}

public class CarreraPanelAbiertaViewModel
{
    public int Id { get; set; }
    public string Distancia { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string Rol { get; set; } = "";
    public int Inscritos { get; set; }
}

public class CarreraPanelTerminadaViewModel
{
    public int Id { get; set; }
    public string Distancia { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string Ganador { get; set; } = "";
}

// Organizacion/Carrera/5?categoria=2: inscritos (o ranking) de una categoría.
public class GestionCarreraViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Detalle { get; set; } = "";
    public bool Terminada { get; set; }
    public bool EsCreadora { get; set; }
    public int TotalInscritos { get; set; }
    public int? CategoriaActual { get; set; }
    public List<CategoriaChipViewModel> Categorias { get; set; } = [];
    public List<InscritoFilaViewModel> Filas { get; set; } = [];
}

public class CategoriaChipViewModel
{
    public int Id { get; set; }
    public string Rango { get; set; } = "";
    public int Inscritos { get; set; }
}

public class InscritoFilaViewModel
{
    public int? Ranking { get; set; }
    public string Corredor { get; set; } = "";
    public string Equipo { get; set; } = "";
    public string? Tiempo { get; set; }
    public string? Ritmo { get; set; }
}

// Formulario de Crear y Editar carrera (la misma vista para los dos).
public class CarreraFormViewModel
{
    public int IdCarrera { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La distancia es obligatoria.")]
    [Range(typeof(decimal), "0.1", "500", ParseLimitsInInvariantCulture = true, ErrorMessage = "La distancia debe estar entre 0.1 y 500 km.")]
    public decimal? Distancia { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(typeof(decimal), "0", "99999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal? PrecioInscripcion { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "Indica cuántas personas hay en tu equipo.")]
    [Range(1, 100000, ErrorMessage = "El equipo debe tener al menos 1 persona.")]
    public int? TamanoEquipo { get; set; }

    public List<CategoriaFormViewModel> Categorias { get; set; } = [];
}

public class CategoriaFormViewModel
{
    // 0 = categoría nueva. Al editar, identifica la categoría que ya existe en la base.
    public int IdCategoria { get; set; }

    [Required(ErrorMessage = "Obligatoria.")]
    [Range(0, 150, ErrorMessage = "Entre 0 y 150.")]
    public int? EdadMinima { get; set; }

    [Required(ErrorMessage = "Obligatoria.")]
    [Range(0, 150, ErrorMessage = "Entre 0 y 150.")]
    public int? EdadMaxima { get; set; }

    [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string? Premios { get; set; }
}
