using ISIRUN.Helpers;

namespace ISIRUN.ViewModels;

// Una actividad de un amigo en el feed.
public class FeedItemViewModel
{
    public int Id { get; set; }
    public int IdCorredor { get; set; }
    public string Corredor { get; set; } = "";
    public string Iniciales => IsirunHelpers.Iniciales(Corredor);
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Km { get; set; }
    public TimeSpan Tiempo { get; set; }
    public int Likes { get; set; }
    public bool MeGusta { get; set; }
}
