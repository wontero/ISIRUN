using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Actividades de los amigos, con likes.
public class FeedController : Controller
{
    public IActionResult Index()
    {
        // TODO: actividades de los amigos del corredor logueado, más recientes primero,
        // con cantidad de likes y si el usuario ya dio like.
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Like(int id)
    {
        // TODO: si ya existe LikeActividad(id, corredorLogueado) borrarlo; si no, crearlo.
        return RedirectToAction(nameof(Index));
    }
}
