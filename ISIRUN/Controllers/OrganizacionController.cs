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
    public async Task<IActionResult> Panel()
    {
        var orgId = User.ObtenerId();
        var org = await db.Organizaciones.AsNoTracking()
            .FirstOrDefaultAsync(o => o.IdOrganizacion == orgId);
        if (org == null) return NotFound();

        // Todas las carreras donde participa (como creadora o como co-organizadora).
        var carreras = await db.OrgCarreras
            .AsNoTracking()
            .Where(o => o.IdOrganizacion == orgId)
            .Select(o => new
            {
                o.Carrera.IdCarrera,
                o.Carrera.Nombre,
                o.Carrera.Distancia,
                o.Carrera.Fecha,
                o.Carrera.Estado,
                o.EsCreadora,
                Creadora = o.Carrera.Organizadoras
                    .Where(x => x.EsCreadora)
                    .Select(x => x.Organizacion.Nombre)
                    .FirstOrDefault(),
                Inscritos = o.Carrera.Resultados.Count,
                Ganador = o.Carrera.Resultados
                    .Where(r => r.Tiempo != null)
                    .OrderBy(r => r.Tiempo)
                    .Select(r => new { r.Corredor.Nombre, r.Tiempo })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var abiertas = carreras
            .Where(c => c.Estado == EstadosCarrera.Abierta)
            .OrderBy(c => c.Fecha)
            .Select(c => new CarreraPanelAbiertaViewModel
            {
                Id = c.IdCarrera,
                Distancia = Formatos.DistanciaCorta(c.Distancia),
                Nombre = c.Nombre,
                Fecha = Formatos.FechaCorta(c.Fecha),
                Rol = c.EsCreadora ? "Organizadora" : $"Co-organizadora con {c.Creadora}",
                Inscritos = c.Inscritos
            }).ToList();

        var terminadas = carreras
            .Where(c => c.Estado == EstadosCarrera.Terminada)
            .OrderByDescending(c => c.Fecha)
            .Select(c => new CarreraPanelTerminadaViewModel
            {
                Id = c.IdCarrera,
                Distancia = Formatos.DistanciaCorta(c.Distancia),
                Nombre = c.Nombre,
                Fecha = Formatos.FechaCorta(c.Fecha),
                Ganador = c.Ganador == null
                    ? "sin corredores"
                    : $"{c.Ganador.Nombre} ({IsirunHelpers.FormatoTiempo(c.Ganador.Tiempo!.Value)})"
            }).ToList();

        return View(new PanelOrganizacionViewModel
        {
            Nombre = org.Nombre,
            Actividad = org.Actividad,
            InscritosEnAbiertas = abiertas.Sum(c => c.Inscritos),
            Abiertas = abiertas,
            Terminadas = terminadas
        });
    }

    // Gestionar una carrera: inscritos por categoría, editar, vínculos, terminar y eliminar.
    // /Organizacion/Carrera/5?categoria=2
    public async Task<IActionResult> Carrera(int id, int? categoria = null)
    {
        var carrera = await db.Carreras
            .AsNoTracking()
            .Include(c => c.Categorias)
            .Include(c => c.Organizadoras)
            .Include(c => c.Resultados).ThenInclude(r => r.Corredor).ThenInclude(c => c.Equipo)
            .FirstOrDefaultAsync(c => c.IdCarrera == id);

        if (carrera == null) return NotFound();

        var orgId = User.ObtenerId();
        var vinculo = carrera.Organizadoras.FirstOrDefault(o => o.IdOrganizacion == orgId);
        if (vinculo == null)
        {
            TempData["Error"] = "No tienes acceso a esta carrera.";
            return RedirectToAction(nameof(Panel));
        }

        var categorias = carrera.Categorias.OrderBy(c => c.EdadMinima).ToList();
        var categoriaActual = categorias.Any(c => c.IdCategoria == categoria)
            ? categoria
            : categorias.FirstOrDefault()?.IdCategoria;

        var terminada = carrera.Estado == EstadosCarrera.Terminada;
        var inscritos = carrera.Resultados.Where(r => r.IdCategoria == categoriaActual);

        // Abierta: por nombre y sin tiempos ("Pendiente"). Terminada: ordenados por ranking.
        var filas = (terminada
                ? inscritos.OrderBy(r => r.Ranking)
                : inscritos.OrderBy(r => r.Corredor.Nombre))
            .Select(r => new InscritoFilaViewModel
            {
                Ranking = r.Ranking,
                Corredor = r.Corredor.Nombre,
                Equipo = r.Corredor.Equipo?.Nombre ?? "Sin equipo",
                Tiempo = r.Tiempo.HasValue ? IsirunHelpers.FormatoTiempo(r.Tiempo.Value) : null,
                Ritmo = r.Tiempo.HasValue ? IsirunHelpers.FormatoRitmo(r.Tiempo.Value, carrera.Distancia) : null
            }).ToList();

        var vm = new GestionCarreraViewModel
        {
            Id = carrera.IdCarrera,
            Nombre = carrera.Nombre,
            Detalle = $"{Presentacion.FechaLarga(carrera.Fecha)} · {Formatos.Km(carrera.Distancia)} km"
                + $" · {Presentacion.Monto(carrera.PrecioInscripcion)} · {carrera.Resultados.Count} inscritos",
            Terminada = terminada,
            EsCreadora = vinculo.EsCreadora,
            TotalInscritos = carrera.Resultados.Count,
            CategoriaActual = categoriaActual,
            Categorias = categorias.Select(c => new CategoriaChipViewModel
            {
                Id = c.IdCategoria,
                Rango = Presentacion.RangoEdad(c.EdadMinima, c.EdadMaxima),
                Inscritos = carrera.Resultados.Count(r => r.IdCategoria == c.IdCategoria)
            }).ToList(),
            Filas = filas
        };

        return View(vm);
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
