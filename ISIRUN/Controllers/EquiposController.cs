using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

[Authorize(Roles = Roles.Corredor)]
public class EquiposController(IsirunContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var miId = User.ObtenerId();
        var miEquipo = await db.Corredores
            .Where(c => c.IdCorredor == miId)
            .Select(c => c.IdEquipo)
            .FirstAsync();

        var filas = await db.Equipos
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new
            {
                e.IdEquipo,
                e.Nombre,
                e.FechaCreacion,
                Miembros = e.Corredores.Count,
                Kms = e.Corredores.Sum(c => (decimal?)c.KmsTotales) ?? 0
            })
            .ToListAsync();

        var equipos = filas.Select(e => new EquipoResumenViewModel
        {
            Id = e.IdEquipo,
            Iniciales = IsirunHelpers.Iniciales(e.Nombre),
            Nombre = e.Nombre,
            FechaCreacion = e.FechaCreacion,
            Miembros = e.Miembros,
            Kms = e.Kms,
            EsMio = e.IdEquipo == miEquipo
        }).ToList();

        return View(equipos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var equipo = await db.Equipos
            .AsNoTracking()
            .Include(e => e.Corredores)
            .FirstOrDefaultAsync(e => e.IdEquipo == id);

        if (equipo == null) return NotFound();

        var miId = User.ObtenerId();
        var idsMiembros = equipo.Corredores.Select(c => c.IdCorredor).ToList();

        var podios = await db.Resultados
            .AsNoTracking()
            .CountAsync(r => idsMiembros.Contains(r.IdCorredor) && r.Ranking != null && r.Ranking <= 3);

        var miembros = equipo.Corredores
            .OrderByDescending(c => c.KmsTotales)
            .Select(c => new MiembroEquipoViewModel
            {
                Id = c.IdCorredor,
                Nombre = c.Nombre,
                Categoria = IsirunHelpers.Edad(c.FechaNac, DateTime.Today) + " años",
                Kms = c.KmsTotales
            }).ToList();

        var vm = new EquipoDetalleViewModel
        {
            Id = equipo.IdEquipo,
            Iniciales = IsirunHelpers.Iniciales(equipo.Nombre),
            Nombre = equipo.Nombre,
            FechaCreacion = equipo.FechaCreacion,
            SoyMiembro = equipo.Corredores.Any(c => c.IdCorredor == miId),
            TotalMiembros = miembros.Count,
            KmsEquipo = miembros.Sum(m => m.Kms),
            Podios = podios,
            Miembros = miembros
        };

        ViewData["Title"] = equipo.Nombre;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(string nombre)
    {
        nombre = (nombre ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            TempData["Error"] = "El nombre es obligatorio.";
            return RedirectToAction(nameof(Index));
        }

        if (nombre.Length > 150)
        {
            TempData["Error"] = "Máximo 150 caracteres.";
            return RedirectToAction(nameof(Index));
        }

        if (await db.Equipos.AnyAsync(e => e.Nombre == nombre))
        {
            TempData["Error"] = "Ya existe un equipo con ese nombre.";
            return RedirectToAction(nameof(Index));
        }

        var miId = User.ObtenerId();
        var corredor = await db.Corredores.FindAsync(miId);
        if (corredor == null) return NotFound();

        var equipo = new Equipo
        {
            Nombre = nombre,
            FechaCreacion = DateTime.Today
        };
        db.Equipos.Add(equipo);
        await db.SaveChangesAsync();

        corredor.IdEquipo = equipo.IdEquipo;
        await db.SaveChangesAsync();

        TempData["Exito"] = "Equipo creado.";
        return RedirectToAction(nameof(Detalle), new { id = equipo.IdEquipo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unirse(int id)
    {
        if (!await db.Equipos.AnyAsync(e => e.IdEquipo == id))
            return NotFound();

        var corredor = await db.Corredores.FindAsync(User.ObtenerId());
        if (corredor == null) return NotFound();

        corredor.IdEquipo = id;
        await db.SaveChangesAsync();

        TempData["Exito"] = "Te uniste al equipo.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir(int id)
    {
        var corredor = await db.Corredores.FindAsync(User.ObtenerId());
        if (corredor == null) return NotFound();

        if (corredor.IdEquipo != id)
        {
            TempData["Error"] = "No perteneces a ese equipo.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        corredor.IdEquipo = null;
        await db.SaveChangesAsync();

        TempData["Exito"] = "Saliste del equipo.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
