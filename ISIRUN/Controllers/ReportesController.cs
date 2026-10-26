using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Los 2 reportes que pide la rúbrica. Solo datos de las carreras de la organización logueada.
// TODO (auth): [Authorize(Roles = "Organizacion")]
public class ReportesController : Controller
{
    // /Reportes/Inscripciones?desde=2026-01-01&hasta=2026-12-31&estado=Abierta
    public IActionResult Inscripciones(DateTime? desde = null, DateTime? hasta = null, string? estado = null)
    {
        // TODO: ver "Reporte 1" en collab.md (consulta, agrupación y totales).
        ViewData["EstadoFiltro"] = estado;
        return View();
    }

    // /Reportes/Rendimiento?carrera=3&agrupar=equipo
    public IActionResult Rendimiento(int? carrera = null, string agrupar = "categoria")
    {
        // TODO: ver "Reporte 2" en collab.md.
        ViewData["Agrupar"] = agrupar == "equipo" ? "equipo" : "categoria";
        return View();
    }
}
