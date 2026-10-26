using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

public class EquiposController : Controller
{
    public IActionResult Index()
    {
        // TODO: lista de equipos con cantidad de miembros y suma de KmsTotales de sus miembros.
        return View();
    }

    public IActionResult Detalle(int id)
    {
        // TODO: equipo {id} con sus miembros ordenados por KmsTotales y si el usuario es miembro.
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(string nombre)
    {
        // TODO: validar nombre (obligatorio, único), crear Equipo con FechaCreacion = hoy
        // y asignar el corredor logueado a ese equipo.
        TempData["Exito"] = "Equipo creado.";
        return RedirectToAction(nameof(Detalle), new { id = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Unirse(int id)
    {
        // TODO: Corredor.IdEquipo = id (un corredor está en un solo equipo a la vez).
        TempData["Exito"] = "Te uniste al equipo.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Salir(int id)
    {
        // TODO: Corredor.IdEquipo = null.
        TempData["Exito"] = "Saliste del equipo.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
