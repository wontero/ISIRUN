using ISIRUN.Models;

namespace ISIRUN.Services;

public class SimulacionCarreraService
{
    // Cargar Resultados antes de llamar: Include(c => c.Resultados).
    // El llamador actualiza KmsTotales y guarda los cambios en la misma transaccion.
    public void Terminar(Carrera carrera)
    {
        ArgumentNullException.ThrowIfNull(carrera);
        if (carrera.Estado != EstadosCarrera.Abierta)
            throw new InvalidOperationException("La carrera ya está terminada.");
        if (carrera.Distancia <= 0)
            throw new InvalidOperationException("La distancia de la carrera debe ser mayor que cero.");

        foreach (var resultado in carrera.Resultados)
        {
            var segundosPorKm = Random.Shared.Next(240, 481);
            resultado.Tiempo = TimeSpan.FromSeconds(
                Math.Round(segundosPorKm * (double)carrera.Distancia));
        }

        foreach (var categoria in carrera.Resultados.GroupBy(r => r.IdCategoria))
        {
            var posicion = 1;
            foreach (var resultado in categoria.OrderBy(r => r.Tiempo).ThenBy(r => r.IdCorredor))
                resultado.Ranking = posicion++;
        }

        carrera.Estado = EstadosCarrera.Terminada;
    }
}
