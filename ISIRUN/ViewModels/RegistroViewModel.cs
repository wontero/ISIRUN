using System.ComponentModel.DataAnnotations;
using ISIRUN.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISIRUN.ViewModels;

// Un solo formulario para los dos tipos de cuenta.
// Los campos de cada tipo no llevan [Required] porque el formulario solo envía los del
// tipo elegido; esos obligatorios se validan en CuentaController.ValidarCamposDelTipo.
public class RegistroViewModel
{
    [Required(ErrorMessage = "Elige un tipo de cuenta.")]
    [RegularExpression($"^({Roles.Corredor}|{Roles.Organizacion})$", ErrorMessage = "Tipo de cuenta no válido.")]
    public string Tipo { get; set; } = Roles.Corredor;

    // Solo corredor
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string? Nombre { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaNac { get; set; }

    public int? IdEquipo { get; set; }

    // Solo organización
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string? NombreOrganizacion { get; set; }

    [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string? Actividad { get; set; }

    // Los dos tipos
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [StringLength(254, ErrorMessage = "Máximo 254 caracteres.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = "";

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmarContrasena { get; set; } = "";

    // Opciones del <select> de equipos (las llena el controlador, no llegan en el POST).
    [ValidateNever]
    public IEnumerable<SelectListItem> Equipos { get; set; } = [];
}
