using System.ComponentModel.DataAnnotations;

namespace ISIRUN.ViewModels;

public class EquipoResumenViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public int Miembros { get; set; }
    public decimal Kms { get; set; }
    public bool EsMio { get; set; }
}

public class EquipoDetalleViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public bool SoyMiembro { get; set; }
    public int TotalMiembros { get; set; }
    public decimal KmsEquipo { get; set; }
    public int Podios { get; set; }
    public List<MiembroEquipoViewModel> Miembros { get; set; } = [];
}

public class MiembroEquipoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public decimal Kms { get; set; }
}

public class CrearEquipoViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Nombre { get; set; } = "";
}
