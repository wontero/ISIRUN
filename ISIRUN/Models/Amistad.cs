namespace ISIRUN.Models;

public class Amistad
{
    public int IdCorredor { get; set; }
    public int IdAmigo { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    public Corredor Corredor { get; set; } = null!;
    public Corredor Amigo { get; set; } = null!;
}
