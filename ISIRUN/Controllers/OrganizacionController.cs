using System.Globalization;
using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Panel de la ORGANIZACIÓN logueada: sus carreras, crear/editar/eliminar, terminar, vínculos y perfil.
[Authorize(Roles = Roles.Organizacion)]
public class OrganizacionController(IsirunContext db) : Controller
{
    public IActionResult Panel()
    {
        // TODO (Mauricio): carreras de la organización logueada (OrgCarrera), separadas en Abiertas y Terminadas,
        // con cantidad de inscritos (count de Resultado).
        return View();
    }

    // Gestionar una carrera: inscritos por categoría, editar, vínculos, terminar y eliminar.
    public IActionResult Carrera(int id, int? categoria = null)
    {
        // TODO (Mauricio): validar que la organización logueada esté en OrgCarrera de esa carrera.
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
        // TODO (Mauricio): CarreraFormViewModel con validaciones.
        TempData["Exito"] = "Carrera creada.";
        return RedirectToAction(nameof(Carrera), new { id = 1 });
    }

    public IActionResult EditarCarrera(int id)
    {
        // TODO (Mauricio): cargar carrera {id} con categorías. Solo la creadora y solo si está Abierta.
        ViewData["Editando"] = true;
        return View("FormularioCarrera");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditarCarrera(int id, IFormCollection form)
    {
        // TODO (Mauricio): mismas validaciones que CrearCarrera.
        TempData["Exito"] = "Cambios guardados.";
        return RedirectToAction(nameof(Carrera), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarCarrera(int id)
    {
        // TODO (Mauricio): solo si está Abierta y la organización es la creadora.
        TempData["Exito"] = "Carrera eliminada.";
        return RedirectToAction(nameof(Panel));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TerminarCarrera(int id)
    {
        // TODO (Mauricio): ver la sección "Terminar carrera" de collab.md.
        TempData["Exito"] = "Carrera terminada. El ranking ya es visible para todos los corredores.";
        return RedirectToAction(nameof(Carrera), new { id });
    }

    public async Task<IActionResult> Vinculos(int id)
    {
        var carrera = await db.Carreras
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCarrera == id);

        if (carrera == null) return NotFound();

        var orgId = User.ObtenerId();
        var participa = await db.OrgCarreras
            .AnyAsync(o => o.IdCarrera == id && o.IdOrganizacion == orgId);
        if (!participa)
        {
            TempData["Error"] = "No tienes acceso a los vínculos de esta carrera.";
            return RedirectToAction(nameof(Panel));
        }

        var orgCarreras = await db.OrgCarreras
            .AsNoTracking()
            .Where(o => o.IdCarrera == id)
            .Include(o => o.Organizacion)
            .ToListAsync();

        var sponsors = await db.Sponsors
            .AsNoTracking()
            .Where(s => s.IdCarrera == id)
            .Include(s => s.Organizacion)
            .ToListAsync();

        var vinculadas = orgCarreras
            .OrderByDescending(o => o.EsCreadora)
            .ThenBy(o => o.Organizacion.Nombre)
            .Select(o => new VinculoItemViewModel
            {
                IdOrganizacion = o.IdOrganizacion,
                Nombre = o.Organizacion.Nombre,
                Rol = "Organizadora",
                Detalle = (o.EsCreadora ? "Creadora" : "Co-organizadora")
                    + $" · desde {Formatos.FechaCorta(o.FechaInicio)} · equipo de {o.TamanoEquipo}",
                EsCreadora = o.EsCreadora
            })
            .Concat(sponsors
                .OrderByDescending(s => s.MontoAportado)
                .Select(s => new VinculoItemViewModel
                {
                    IdOrganizacion = s.IdOrganizacion,
                    Nombre = s.Organizacion.Nombre,
                    Rol = "Sponsor",
                    Detalle = $"{s.TipoParticipacion} · {Presentacion.Monto(s.MontoAportado)}",
                    EsCreadora = false
                }))
            .ToList();

        // Todas las orgs registradas (el controlador valida rol duplicado al agregar).
        var disponibles = await db.Organizaciones
            .AsNoTracking()
            .OrderBy(o => o.Nombre)
            .Select(o => new OrganizacionOpcionViewModel
            {
                Id = o.IdOrganizacion,
                Nombre = o.Nombre
            })
            .ToListAsync();

        var vm = new VinculosViewModel
        {
            IdCarrera = id,
            NombreCarrera = carrera.Nombre,
            Vinculadas = vinculadas,
            OrganizacionesDisponibles = disponibles
        };

        ViewData["Title"] = "Sponsors y co-organizadores";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarVinculo(int id, IFormCollection form)
    {
        var orgId = User.ObtenerId();
        var participa = await db.OrgCarreras
            .AnyAsync(o => o.IdCarrera == id && o.IdOrganizacion == orgId);
        if (!participa)
        {
            TempData["Error"] = "No tienes acceso a los vínculos de esta carrera.";
            return RedirectToAction(nameof(Panel));
        }

        if (!await db.Carreras.AnyAsync(c => c.IdCarrera == id))
            return NotFound();

        if (!int.TryParse(form["IdOrganizacion"], out var idOrganizacion))
        {
            TempData["Error"] = "Elige una organización.";
            return RedirectToAction(nameof(Vinculos), new { id });
        }

        if (!await db.Organizaciones.AnyAsync(o => o.IdOrganizacion == idOrganizacion))
        {
            TempData["Error"] = "Esa organización no existe.";
            return RedirectToAction(nameof(Vinculos), new { id });
        }

        var rol = form["Rol"].ToString();
        if (rol == "Organizadora")
        {
            if (await db.OrgCarreras.AnyAsync(o => o.IdCarrera == id && o.IdOrganizacion == idOrganizacion))
            {
                TempData["Error"] = "Esa organización ya está vinculada como organizadora.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            if (!DateTime.TryParse(form["FechaInicio"], out var fechaInicio))
            {
                TempData["Error"] = "La fecha de inicio es obligatoria.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            if (!int.TryParse(form["TamanoEquipo"], out var tamano) || tamano < 1)
            {
                TempData["Error"] = "El tamaño del equipo debe ser al menos 1.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            db.OrgCarreras.Add(new OrgCarrera
            {
                IdCarrera = id,
                IdOrganizacion = idOrganizacion,
                FechaInicio = fechaInicio.Date,
                TamanoEquipo = tamano,
                EsCreadora = false
            });
        }
        else if (rol == "Sponsor")
        {
            if (await db.Sponsors.AnyAsync(s => s.IdCarrera == id && s.IdOrganizacion == idOrganizacion))
            {
                TempData["Error"] = "Esa organización ya está vinculada como sponsor.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            if (!decimal.TryParse(form["MontoAportado"], NumberStyles.Number, CultureInfo.InvariantCulture, out var monto) || monto <= 0)
            {
                TempData["Error"] = "El monto aportado debe ser mayor que 0.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            var tipo = form["TipoParticipacion"].ToString()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(tipo))
            {
                TempData["Error"] = "El tipo de participación es obligatorio.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            db.Sponsors.Add(new Sponsor
            {
                IdCarrera = id,
                IdOrganizacion = idOrganizacion,
                MontoAportado = monto,
                TipoParticipacion = tipo
            });
        }
        else
        {
            TempData["Error"] = "Elige si participa como co-organizadora o sponsor.";
            return RedirectToAction(nameof(Vinculos), new { id });
        }

        await db.SaveChangesAsync();
        TempData["Exito"] = "Organización vinculada.";
        return RedirectToAction(nameof(Vinculos), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarVinculo(int id, int idOrganizacion, string rol)
    {
        var orgId = User.ObtenerId();
        var participa = await db.OrgCarreras
            .AnyAsync(o => o.IdCarrera == id && o.IdOrganizacion == orgId);
        if (!participa)
        {
            TempData["Error"] = "No tienes acceso a los vínculos de esta carrera.";
            return RedirectToAction(nameof(Panel));
        }

        if (rol == "Organizadora")
        {
            var vinculo = await db.OrgCarreras.FindAsync(id, idOrganizacion);
            if (vinculo == null)
            {
                TempData["Error"] = "Ese vínculo no existe.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            if (vinculo.EsCreadora)
            {
                TempData["Error"] = "La organización creadora no se puede quitar.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            db.OrgCarreras.Remove(vinculo);
        }
        else if (rol == "Sponsor")
        {
            var sponsor = await db.Sponsors.FindAsync(id, idOrganizacion);
            if (sponsor == null)
            {
                TempData["Error"] = "Ese vínculo no existe.";
                return RedirectToAction(nameof(Vinculos), new { id });
            }

            db.Sponsors.Remove(sponsor);
        }
        else
        {
            TempData["Error"] = "Rol de vínculo no válido.";
            return RedirectToAction(nameof(Vinculos), new { id });
        }

        await db.SaveChangesAsync();
        TempData["Exito"] = "Vínculo eliminado.";
        return RedirectToAction(nameof(Vinculos), new { id });
    }

    public async Task<IActionResult> Perfil()
    {
        var org = await db.Organizaciones.AsNoTracking()
            .FirstOrDefaultAsync(o => o.IdOrganizacion == User.ObtenerId());
        if (org == null) return NotFound();

        return View(new PerfilOrganizacionViewModel
        {
            Nombre = org.Nombre,
            Actividad = org.Actividad
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilOrganizacionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var org = await db.Organizaciones.FindAsync(User.ObtenerId());
        if (org == null) return NotFound();

        org.Nombre = model.Nombre.Trim();
        org.Actividad = model.Actividad.Trim();
        await db.SaveChangesAsync();

        TempData["Exito"] = "Perfil actualizado.";
        return RedirectToAction(nameof(Perfil));
    }
}
