using System.Globalization;

namespace ISIRUN.Helpers;

// Formatos para mostrar fechas y kilómetros en las vistas, siempre igual
// (en español, sin depender del idioma de la PC donde corre la app).
public static class Formatos
{
    private static readonly CultureInfo Espanol = new("es-ES");

    // 22 nov 2026
    public static string FechaCorta(DateTime fecha) => fecha.ToString("dd MMM yyyy", Espanol).Replace(".", "");

    // mar 2025
    public static string MesAnio(DateTime fecha) => fecha.ToString("MMM yyyy", Espanol).Replace(".", "");

    // Hoy | Ayer | 23 oct
    public static string FechaRelativa(DateTime fecha)
    {
        var dias = (DateTime.Today - fecha.Date).Days;
        return dias switch
        {
            0 => "Hoy",
            1 => "Ayer",
            _ => fecha.ToString("dd MMM", Espanol).Replace(".", "")
        };
    }

    // 486.2
    public static string Km(decimal km) => km.ToString("0.0", CultureInfo.InvariantCulture);

    // 10K, 21.1K
    public static string DistanciaCorta(decimal km) => km.ToString("0.#", CultureInfo.InvariantCulture) + "K";
}
