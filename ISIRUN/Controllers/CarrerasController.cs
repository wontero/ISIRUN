using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Vista del CORREDOR: catálogo, detalle de carrera abierta, ranking de carrera terminada e inscripción.
public class CarrerasController : Controller
{
    // /Carreras?estado=abiertas  |  /Carreras?estado=terminadas&distancia=10
    public IActionResult Index(string estado = "abiertas", string? distancia = null)
    {
        // TODO: filtrar Carrera por Estado y distancia. Pasar la lista a la vista.
        ViewData["Estado"] = estado == "terminadas" ? "terminadas" : "abiertas";
        ViewData["Distancia"] = distancia;
        return View();
    }

    public IActionResult Detalle(int id)
    {
        // TODO: si la carrera {id} está Terminada -> return RedirectToAction(nameof(Resultados), new { id });
        // Traer categorías, organizadoras (OrgCarrera), sponsors y si el corredor ya está inscrito.
        return View();
    }

    // /Carreras/Resultados/5?categoria=2
    public IActionResult Resultados(int id, int? categoria = null)
    {
        // TODO: si la carrera NO está Terminada -> return RedirectToAction(nameof(Detalle), new { id });
        // Traer los Resultado de la categoría elegida ordenados por Ranking, y el resultado del usuario.
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Inscribirse(int id)
    {
        // TODO: validar que la carrera esté Abierta y que no esté ya inscrito.
        // Calcular la edad del corredor -> buscar la Categoria_edad de ESA carrera que la contenga
        // (si no hay, TempData["Error"] = "No hay categoría para tu edad en esta carrera.").
        // Crear Resultado { IdCorredor, IdCarrera, IdCategoria, Tiempo = null, Ranking = null, FechaInscripcion = ahora }.
        TempData["Exito"] = "¡Listo! Quedaste inscrito. Verás tu resultado cuando la organización termine la carrera.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelarInscripcion(int id)
    {
        // TODO: solo si la carrera está Abierta. Borrar el Resultado (corredor logueado, carrera {id}).
        TempData["Exito"] = "Inscripción cancelada.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
