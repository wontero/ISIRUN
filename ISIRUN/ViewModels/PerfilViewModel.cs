namespace ISIRUN.ViewModels;

// Mi perfil (/Corredor/Perfil): lo público + inscripciones pendientes y total de actividades.
public class PerfilViewModel : PerfilPublicoViewModel
{
    public int TotalActividades { get; set; }
    public List<InscripcionPerfilViewModel> Inscripciones { get; set; } = [];
}

public class InscripcionPerfilViewModel
{
    public int IdCarrera { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Distancia { get; set; }
    public DateTime Fecha { get; set; }
    public int EdadMinima { get; set; }
    public int EdadMaxima { get; set; }
    public string Categoria => $"{EdadMinima}–{EdadMaxima}";
}
