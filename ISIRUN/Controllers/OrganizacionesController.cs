using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Vista del CORREDOR (solo lectura): lista de organizaciones y qué carreras organizan o patrocinan.
// No confundir con OrganizacionController (singular), que es el panel de la organización logueada.
public class OrganizacionesController : Controller
{
    // /Organizaciones?id=3 muestra el detalle de la organización 3 al costado de la lista.
    public IActionResult Index(int? id = null)
    {
        // TODO: lista de organizaciones + la seleccionada con sus OrgCarrera y Sponsor (con nombre de carrera).
        return View();
    }
}
