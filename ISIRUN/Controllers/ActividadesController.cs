using ISIRUN.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// CRUD de actividades (entrenamientos) del corredor logueado.
[Authorize(Roles = Roles.Corredor)]
public class ActividadesController : Controller
{
    public IActionResult Crear()
    {
        return View("Formulario");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(IFormCollection form)
    {
        // TODO: ActividadViewModel + validaciones (nombre obligatorio, distancia > 0, tiempo hh:mm:ss).
        // Si !ModelState.IsValid: return View("Formulario", vm);
        // Guardar y sumar la distancia a Corredor.KmsTotales.
        TempData["Exito"] = "Actividad registrada.";
        return RedirectToAction("Perfil", "Corredor");
    }

    public IActionResult Editar(int id)
    {
        // TODO: cargar la actividad {id} (solo si es del corredor logueado) y pasarla a la vista.
        ViewData["Editando"] = true;
        return View("Formulario");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, IFormCollection form)
    {
        // TODO: validar, actualizar y ajustar KmsTotales con la diferencia de distancia.
        TempData["Exito"] = "Actividad actualizada.";
        return RedirectToAction("Perfil", "Corredor");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        // TODO: borrar la actividad {id} (y sus likes) y restar su distancia de KmsTotales.
        TempData["Exito"] = "Actividad eliminada.";
        return RedirectToAction("Perfil", "Corredor");
    }
}
