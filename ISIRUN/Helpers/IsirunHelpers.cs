namespace ISIRUN.Helpers;

public static class IsirunHelpers
{
    public static int Edad(DateTime nacimiento, DateTime dia)
    {
        if (nacimiento.Date > dia.Date)
            throw new ArgumentOutOfRangeException(nameof(nacimiento), "El nacimiento no puede ser posterior a la fecha de referencia.");

        var edad = dia.Year - nacimiento.Year;
        if (nacimiento.Date > dia.Date.AddYears(-edad)) edad--;
        return edad;
    }

    public static string Iniciales(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return "";

        var palabras = nombre.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(palabras.Take(2).Select(p => p[0])).ToUpperInvariant();
    }

    public static string FormatoTiempo(TimeSpan tiempo)
    {
        if (tiempo < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(tiempo), "El tiempo no puede ser negativo.");

        return $"{(int)tiempo.TotalHours}:{tiempo.Minutes:00}:{tiempo.Seconds:00}";
    }

    public static string FormatoRitmo(TimeSpan tiempo, decimal km)
    {
        if (km <= 0)
            throw new ArgumentOutOfRangeException(nameof(km), "La distancia debe ser mayor que cero.");
        if (tiempo < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(tiempo), "El tiempo no puede ser negativo.");

        var segundos = (int)Math.Round(tiempo.TotalSeconds / (double)km);
        return $"{segundos / 60}:{segundos % 60:00}";
    }
}
