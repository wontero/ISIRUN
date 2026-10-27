using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Los 2 reportes que pide la rúbrica. Solo datos de las carreras de la organización logueada.
[Authorize(Roles = Roles.Organizacion)]
public class ReportesController(IsirunContext db) : Controller
{
    // /Reportes/Inscripciones?desde=2026-01-01&hasta=2026-12-31&estado=Abierta
    public async Task<IActionResult> Inscripciones(DateTime? desde = null, DateTime? hasta = null, string? estado = null)
    {
        if (estado != EstadosCarrera.Abierta && estado != EstadosCarrera.Terminada)
            estado = null; // "Todas"

        var orgId = User.ObtenerId();
        var q = db.Carreras.AsNoTracking()
            .Where(c => c.Organizadoras.Any(o => o.IdOrganizacion == orgId));
        if (desde.HasValue) q = q.Where(c => c.Fecha >= desde.Value.Date);
        if (hasta.HasValue) q = q.Where(c => c.Fecha < hasta.Value.Date.AddDays(1));
        if (estado != null) q = q.Where(c => c.Estado == estado);

        var carreras = await q
            .OrderBy(c => c.Fecha)
            .Select(c => new
            {
                c.Nombre,
                c.Fecha,
                c.Estado,
                Inscritos = c.Resultados.Count,
                Recaudado = c.Resultados.Count * c.PrecioInscripcion,
                PorCategoria = c.Categorias.Select(cat => new
                {
                    cat.EdadMinima,
                    cat.EdadMaxima,
                    Inscritos = cat.Resultados.Count
                }).ToList()
            })
            .ToListAsync();

        // Columnas de categoría: los rangos distintos de todas las carreras, de menor a mayor edad.
        var rangos = carreras
            .SelectMany(c => c.PorCategoria)
            .Select(p => (p.EdadMinima, p.EdadMaxima))
            .Distinct()
            .OrderBy(r => r.EdadMinima).ThenBy(r => r.EdadMaxima)
            .ToList();

        var filas = carreras.Select(c => new FilaInscripcionesViewModel
        {
            Carrera = c.Nombre,
            Fecha = c.Fecha,
            Estado = c.Estado,
            // Si la carrera no tiene ese rango, se muestra 0.
            InscritosPorRango = rangos
                .Select(r => c.PorCategoria
                    .Where(p => p.EdadMinima == r.EdadMinima && p.EdadMaxima == r.EdadMaxima)
                    .Sum(p => p.Inscritos))
                .ToList(),
            Inscritos = c.Inscritos,
            Recaudado = c.Recaudado
        }).ToList();

        var totalInscritos = filas.Sum(f => f.Inscritos);
        var totalRecaudado = filas.Sum(f => f.Recaudado);
        var totalesPorRango = rangos.Select((_, i) => filas.Sum(f => f.InscritosPorRango[i])).ToList();
        var maxCategoria = Math.Max(1, totalesPorRango.DefaultIfEmpty(0).Max());

        var vm = new ReporteInscripcionesViewModel
        {
            Desde = desde,
            Hasta = hasta,
            Estado = estado,
            Rangos = rangos.Select(r => Presentacion.RangoEdad(r.EdadMinima, r.EdadMaxima)).ToList(),
            Filas = filas,
            TotalesPorCategoria = rangos.Select((r, i) => new BarraCategoriaViewModel
            {
                Rango = Presentacion.RangoEdad(r.EdadMinima, r.EdadMaxima),
                Inscritos = totalesPorRango[i],
                PorcentajeBarra = totalesPorRango[i] * 100 / maxCategoria,
                Participacion = totalInscritos == 0 ? 0 : totalesPorRango[i] * 100 / totalInscritos
            }).ToList(),
            TotalInscritos = totalInscritos,
            TotalRecaudado = totalRecaudado,
            PromedioPorCarrera = filas.Count == 0 ? 0 : Math.Round(totalRecaudado / filas.Count)
        };

        return View(vm);
    }

    // /Reportes/Rendimiento?carrera=3&agrupar=equipo
    public async Task<IActionResult> Rendimiento(int? carrera = null, string agrupar = "categoria")
    {
        var orgId = User.ObtenerId();
        var porEquipo = agrupar == "equipo";

        // Solo las carreras terminadas de la organización, la más reciente primero.
        var terminadas = await db.Carreras.AsNoTracking()
            .Where(c => c.Estado == EstadosCarrera.Terminada
                && c.Organizadoras.Any(o => o.IdOrganizacion == orgId))
            .OrderByDescending(c => c.Fecha)
            .Select(c => new { c.IdCarrera, c.Nombre, c.Distancia })
            .ToListAsync();

        var vm = new ReporteRendimientoViewModel
        {
            PorEquipo = porEquipo,
            Carreras = terminadas
                .Select(c => new CarreraOpcionViewModel { Id = c.IdCarrera, Nombre = c.Nombre })
                .ToList()
        };

        // Si no eligió carrera (o eligió una que no es suya), se muestra la más reciente.
        var elegida = terminadas.FirstOrDefault(c => c.IdCarrera == carrera) ?? terminadas.FirstOrDefault();
        if (elegida == null)
            return View(vm);

        vm.CarreraSeleccionada = elegida.IdCarrera;
        vm.NombreCarrera = elegida.Nombre;

        var resultados = await db.Resultados.AsNoTracking()
            .Where(r => r.IdCarrera == elegida.IdCarrera && r.Tiempo != null)
            .Select(r => new
            {
                Tiempo = r.Tiempo!.Value,
                r.Ranking,
                Corredor = r.Corredor.Nombre,
                Equipo = r.Corredor.Equipo != null ? r.Corredor.Equipo.Nombre : "Sin equipo",
                r.Categoria.EdadMinima,
                r.Categoria.EdadMaxima
            })
            .ToListAsync(); // se traen a memoria: el promedio de TimeSpan se calcula en C#

        if (resultados.Count == 0)
            return View(vm);

        vm.Filas = resultados
            .GroupBy(r => porEquipo ? r.Equipo : Presentacion.RangoEdad(r.EdadMinima, r.EdadMaxima))
            .Select(g =>
            {
                var mejor = g.OrderBy(r => r.Tiempo).First();
                var promedio = TimeSpan.FromSeconds(Math.Round(g.Average(r => r.Tiempo.TotalSeconds)));
                return new
                {
                    Fila = new FilaRendimientoViewModel
                    {
                        Grupo = g.Key,
                        Finalistas = g.Count(),
                        Podios = g.Count(r => r.Ranking <= 3),
                        MejorTiempo = IsirunHelpers.FormatoTiempo(mejor.Tiempo),
                        MejorNombre = mejor.Corredor,
                        TiempoPromedio = IsirunHelpers.FormatoTiempo(promedio),
                        Ritmo = IsirunHelpers.FormatoRitmo(promedio, elegida.Distancia)
                    },
                    Promedio = promedio
                };
            })
            .OrderByDescending(x => x.Fila.Podios).ThenBy(x => x.Promedio)
            .Select(x => x.Fila)
            .ToList();

        var mejorGeneral = resultados.OrderBy(r => r.Tiempo).First();
        var promedioGeneral = TimeSpan.FromSeconds(Math.Round(resultados.Average(r => r.Tiempo.TotalSeconds)));

        vm.TotalFinalistas = resultados.Count;
        vm.TotalPodios = resultados.Count(r => r.Ranking <= 3);
        vm.MejorTiempo = IsirunHelpers.FormatoTiempo(mejorGeneral.Tiempo);
        vm.MejorNombre = mejorGeneral.Corredor;
        vm.TiempoPromedio = IsirunHelpers.FormatoTiempo(promedioGeneral);
        vm.RitmoPromedio = IsirunHelpers.FormatoRitmo(promedioGeneral, elegida.Distancia);

        return View(vm);
    }
}
