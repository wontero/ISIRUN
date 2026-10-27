using System.ComponentModel.DataAnnotations;

namespace ISIRUN.ViewModels;

// Formulario de Crear y Editar actividad (la misma vista para los dos).
public class ActividadViewModel
{
    public static readonly string[] Tipos = ["Calle", "Pista", "Trail", "Cinta"];

    public int IdActividad { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "Elige un tipo.")]
    public string Tipo { get; set; } = Tipos[0];

    [Required(ErrorMessage = "La distancia es obligatoria.")]
    [Range(typeof(decimal), "0.01", "9999.99", ErrorMessage = "La distancia debe ser mayor que 0.")]
    public decimal? DistanciaKm { get; set; }

    // Se escribe como texto "h:mm:ss" y el controlador lo convierte a TimeSpan.
    // Horas de 0 a 23 porque la columna de la base es de tipo time.
    [Required(ErrorMessage = "El tiempo es obligatorio.")]
    [RegularExpression(@"^([01]?\d|2[0-3]):[0-5]\d:[0-5]\d$", ErrorMessage = "Usa el formato h:mm:ss (por ejemplo 0:52:30).")]
    public string Tiempo { get; set; } = "";

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres.")]
    public string? Notas { get; set; }
}
