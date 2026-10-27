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
    public IActionResult Rendimiento(int? carrera = null, string agrupar = "categoria")
    {
        // TODO: ver "Reporte 2" en collab.md.
        ViewData["Agrupar"] = agrupar == "equipo" ? "equipo" : "categoria";
        return View();
    }
}
