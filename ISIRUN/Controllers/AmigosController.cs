using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

public class AmigosController : Controller
{
    // /Amigos?q=cata
    public IActionResult Index(string? q = null)
    {
        // TODO: mis amigos (Amistad) + resultados de búsqueda por nombre/email (excluyéndome a mí).
        ViewData["Busqueda"] = q;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Agregar(int id)
    {
        // TODO: crear Amistad(corredorLogueado, id). Validar que id != corredorLogueado y que no exista.
        TempData["Exito"] = "Amigo agregado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Quitar(int id)
    {
        // TODO: borrar Amistad(corredorLogueado, id).
        TempData["Exito"] = "Amigo quitado.";
        return RedirectToAction(nameof(Index));
    }
}
