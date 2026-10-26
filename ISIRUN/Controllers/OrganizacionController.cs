using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Panel de la ORGANIZACIÓN logueada: sus carreras, crear/editar/eliminar, terminar, vínculos y perfil.
// TODO (auth): agregar [Authorize(Roles = "Organizacion")] a la clase cuando exista el login.
public class OrganizacionController : Controller
{
    public IActionResult Panel()
    {
        // TODO: carreras de la organización logueada (OrgCarrera), separadas en Abiertas y Terminadas,
        // con cantidad de inscritos (count de Resultado).
        return View();
    }

    // Gestionar una carrera: inscritos por categoría, editar, vínculos, terminar y eliminar.
    public IActionResult Carrera(int id, int? categoria = null)
    {
        // TODO: validar que la organización logueada esté en OrgCarrera de esa carrera.
        // Si está Terminada, mostrar el ranking en vez de "Pendiente".
        return View();
    }

    public IActionResult CrearCarrera()
    {
        return View("FormularioCarrera");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearCarrera(IFormCollection form)
    {
        // TODO: CarreraFormViewModel con validaciones:
        //  - nombre obligatorio, distancia > 0, precio >= 0, fecha futura
        //  - al menos 1 categoría; EdadMinima <= EdadMaxima; rangos sin superponerse
        // Guardar Carrera (Estado = "Abierta"), sus Categoria_edad y OrgCarrera (EsCreadora = true).
        TempData["Exito"] = "Carrera creada.";
        return RedirectToAction(nameof(Carrera), new { id = 1 });
    }

    public IActionResult EditarCarrera(int id)
    {
        // TODO: cargar carrera {id} con categorías. Solo la creadora y solo si está Abierta.
        ViewData["Editando"] = true;
        return View("FormularioCarrera");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditarCarrera(int id, IFormCollection form)
    {
        // TODO: mismas validaciones que CrearCarrera. Ojo: no borrar una categoría que ya tiene inscritos.
        TempData["Exito"] = "Cambios guardados.";
        return RedirectToAction(nameof(Carrera), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarCarrera(int id)
    {
        // TODO: solo si está Abierta y la organización es la creadora.
        // Borrar en cascada: Resultado, Categoria_edad, OrgCarrera, Sponsor y la Carrera.
        TempData["Exito"] = "Carrera eliminada.";
        return RedirectToAction(nameof(Panel));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TerminarCarrera(int id)
    {
        // TODO: ver la sección "Terminar carrera" de collab.md (algoritmo exacto de la simulación).
        TempData["Exito"] = "Carrera terminada. El ranking ya es visible para todos los corredores.";
        return RedirectToAction(nameof(Carrera), new { id });
    }

    public IActionResult Vinculos(int id)
    {
        // TODO: OrgCarrera y Sponsor de la carrera {id} + lista de organizaciones para el <select>.
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AgregarVinculo(int id, IFormCollection form)
    {
        // TODO: según form["Rol"]:
        //  "Organizadora" -> OrgCarrera { FechaInicio, TamanoEquipo, EsCreadora = false }
        //  "Sponsor"      -> Sponsor { MontoAportado > 0, TipoParticipacion }
        // Validar que esa organización no esté ya vinculada con ese rol.
        TempData["Exito"] = "Organización vinculada.";
        return RedirectToAction(nameof(Vinculos), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuitarVinculo(int id, int idOrganizacion, string rol)
    {
        // TODO: borrar OrgCarrera o Sponsor. La organización creadora NO se puede quitar.
        TempData["Exito"] = "Vínculo eliminado.";
        return RedirectToAction(nameof(Vinculos), new { id });
    }

    public IActionResult Perfil()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Perfil(IFormCollection form)
    {
        // TODO: validar y actualizar Nombre y Actividad de la organización logueada.
        TempData["Exito"] = "Perfil actualizado.";
        return RedirectToAction(nameof(Perfil));
    }
}
