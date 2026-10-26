using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Perfil del corredor logueado y perfil público de otro corredor.
public class CorredorController : Controller
{
    public IActionResult Perfil()
    {
        // TODO: datos del corredor logueado: stats, equipo, inscripciones pendientes,
        // resultados y últimas actividades (PerfilViewModel).
        return View();
    }

    public IActionResult Ver(int id)
    {
        // TODO: perfil público del corredor {id} (sus stats, resultados y actividades).
        return View();
    }
}
