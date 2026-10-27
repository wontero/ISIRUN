using System.Globalization;

namespace ISIRUN.Helpers;

// Textos listos para mostrar en las vistas (montos, rangos, fechas de tarjeta).
public static class Presentacion
{
    private static readonly CultureInfo EsCl = new("es-CL");
    private static readonly CultureInfo EsEs = new("es-ES");

    public static string Monto(decimal valor) =>
        "$" + valor.ToString("N0", EsCl);

    public static string RangoEdad(int min, int max) =>
        max >= 100 ? $"{min}+" : $"{min}–{max}";

    // 15 NOV
    public static string FechaTarjeta(DateTime fecha) =>
        fecha.ToString("dd MMM", EsEs).Replace(".", "").ToUpperInvariant();

    // Sáb 22 nov 2026 · 20:00
    public static string FechaLarga(DateTime fecha)
    {
        var dia = fecha.ToString("ddd dd MMM yyyy", EsEs).Replace(".", "");
        dia = char.ToUpper(dia[0], EsEs) + dia[1..];
        return $"{dia} · {fecha:HH:mm}";
    }

    public static string PremioPuesto(string premios, int ranking)
    {
        if (ranking is < 1 or > 3 || string.IsNullOrWhiteSpace(premios))
            return "";

        var partes = premios.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (ranking > partes.Length) return "";

        var parte = partes[ranking - 1];
        var sep = parte.IndexOf(':');
        var texto = sep >= 0 ? parte[(sep + 1)..].Trim() : parte.Trim();
        return $"{ranking}° · {texto}";
    }
}
