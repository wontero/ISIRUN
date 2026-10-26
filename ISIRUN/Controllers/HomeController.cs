using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ISIRUN.Models;

namespace ISIRUN.Controllers;

// Página pública de inicio.
public class HomeController : Controller
{
    public IActionResult Index()
    {
        // TODO: traer las próximas 3 carreras abiertas (ordenadas por fecha).
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
