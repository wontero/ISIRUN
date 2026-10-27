using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Actividades de los amigos, con likes.
[Authorize(Roles = Roles.Corredor)]
public class FeedController(IsirunContext db) : Controller
{
    private const int MaximoActividades = 50;

    public async Task<IActionResult> Index()
    {
        var miId = User.ObtenerId();

        // Mis amigos = los corredores que yo agregué (Amistad con IdCorredor = yo).
        var idsAmigos = db.Amistades.Where(a => a.IdCorredor == miId).Select(a => a.IdAmigo);

        var feed = await db.Actividades
            .Where(a => idsAmigos.Contains(a.IdCorredor))
            .OrderByDescending(a => a.Fecha).ThenByDescending(a => a.IdActividad)
            .Take(MaximoActividades)
            .Select(a => new FeedItemViewModel
            {
                Id = a.IdActividad,
                IdCorredor = a.IdCorredor,
                Corredor = a.Corredor.Nombre,
                Fecha = a.Fecha,
                Tipo = a.Tipo,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Km = a.DistanciaKm,
                Tiempo = a.Tiempo,
                Likes = a.Likes.Count,
                MeGusta = a.Likes.Any(l => l.IdCorredor == miId)
            })
            .ToListAsync();

        return View(feed);
    }

    // Toggle: si ya le di like lo quito, si no lo creo.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        var miId = User.ObtenerId();
        var actividad = await db.Actividades.FindAsync(id);
        if (actividad == null)
            return NotFound();

        if (actividad.IdCorredor == miId)
        {
            TempData["Error"] = "No puedes dar like a tu propia actividad.";
            return RedirectToAction(nameof(Index));
        }

        var like = await db.LikesActividad.FindAsync(id, miId);
        if (like == null)
            db.LikesActividad.Add(new LikeActividad { IdActividad = id, IdCorredor = miId, Fecha = DateTime.Now });
        else
            db.LikesActividad.Remove(like);

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
