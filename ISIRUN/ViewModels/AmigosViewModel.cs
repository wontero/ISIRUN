using ISIRUN.Helpers;

namespace ISIRUN.ViewModels;

// Pantalla de Amigos: búsqueda de corredores + mis amigos.
public class AmigosViewModel
{
    public string? Busqueda { get; set; }
    public List<CorredorBusquedaViewModel> Resultados { get; set; } = [];
    public List<AmigoViewModel> Amigos { get; set; } = [];
}

public class CorredorBusquedaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Iniciales => IsirunHelpers.Iniciales(Nombre);
    public string Equipo { get; set; } = "";
    public bool EsAmigo { get; set; }
}

public class AmigoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Iniciales => IsirunHelpers.Iniciales(Nombre);
    public decimal Kms { get; set; }
}
