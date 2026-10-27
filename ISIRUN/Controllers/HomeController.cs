using System.Diagnostics;
using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Página pública de inicio.
public class HomeController(IsirunContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var filas = await db.Carreras
            .AsNoTracking()
            .Where(c => c.Estado == EstadosCarrera.Abierta)
            .OrderBy(c => c.Fecha)
            .Take(3)
            .Select(c => new
            {
                c.IdCarrera,
                c.Nombre,
                c.Distancia,
                c.Fecha,
                c.PrecioInscripcion,
                Organiza = c.Organizadoras
                    .Where(o => o.EsCreadora)
                    .Select(o => o.Organizacion.Nombre)
                    .FirstOrDefault()
                    ?? c.Organizadoras.Select(o => o.Organizacion.Nombre).FirstOrDefault()
                    ?? "Sin organización"
            })
            .ToListAsync();

        var proximas = filas.Select(c => new CarreraTarjetaViewModel
        {
            Id = c.IdCarrera,
            Distancia = Formatos.DistanciaCorta(c.Distancia),
            Fecha = Presentacion.FechaTarjeta(c.Fecha),
            Nombre = c.Nombre,
            Organiza = c.Organiza,
            Precio = Presentacion.Monto(c.PrecioInscripcion)
        }).ToList();

        return View(proximas);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
