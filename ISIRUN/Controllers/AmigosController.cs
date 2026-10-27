using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Buscar corredores, agregar y quitar amigos.
[Authorize(Roles = Roles.Corredor)]
public class AmigosController(IsirunContext db) : Controller
{
    private const int MaximoResultados = 20;

    // /Amigos?q=cata
    public async Task<IActionResult> Index(string? q = null)
    {
        var miId = User.ObtenerId();
        var vm = new AmigosViewModel { Busqueda = q };

        vm.Amigos = await db.Amistades
            .Where(a => a.IdCorredor == miId)
            .OrderBy(a => a.Amigo.Nombre)
            .Select(a => new AmigoViewModel
            {
                Id = a.IdAmigo,
                Nombre = a.Amigo.Nombre,
                Kms = a.Amigo.KmsTotales
            })
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            vm.Resultados = await db.Corredores
                .Where(c => c.IdCorredor != miId && (c.Nombre.Contains(texto) || c.Email.Contains(texto)))
                .OrderBy(c => c.Nombre)
                .Take(MaximoResultados)
                .Select(c => new CorredorBusquedaViewModel
                {
                    Id = c.IdCorredor,
                    Nombre = c.Nombre,
                    Equipo = c.Equipo != null ? c.Equipo.Nombre : "Sin equipo",
                    EsAmigo = db.Amistades.Any(a => a.IdCorredor == miId && a.IdAmigo == c.IdCorredor)
                })
                .ToListAsync();
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int id)
    {
        var miId = User.ObtenerId();

        if (id == miId)
            TempData["Error"] = "No puedes agregarte a ti mismo.";
        else if (!await db.Corredores.AnyAsync(c => c.IdCorredor == id))
            TempData["Error"] = "Ese corredor no existe.";
        else if (await db.Amistades.AnyAsync(a => a.IdCorredor == miId && a.IdAmigo == id))
            TempData["Error"] = "Ya es tu amigo.";
        else
        {
            db.Amistades.Add(new Amistad { IdCorredor = miId, IdAmigo = id, Fecha = DateTime.Now });
            await db.SaveChangesAsync();
            TempData["Exito"] = "Amigo agregado.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quitar(int id)
    {
        var miId = User.ObtenerId();
        var amistad = await db.Amistades.FindAsync(miId, id);
        if (amistad == null)
        {
            TempData["Error"] = "Ese corredor no está en tu lista de amigos.";
            return RedirectToAction(nameof(Index));
        }

        db.Amistades.Remove(amistad);
        await db.SaveChangesAsync();
        TempData["Exito"] = "Amigo quitado.";
        return RedirectToAction(nameof(Index));
    }
}
