using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Perfil del corredor logueado y perfil público de otro corredor.
[Authorize(Roles = Roles.Corredor)]
public class CorredorController(IsirunContext db) : Controller
{
    private const int UltimasActividades = 5;

    public async Task<IActionResult> Perfil()
    {
        var miId = User.ObtenerId();
        var vm = await CargarPerfilAsync<PerfilViewModel>(miId);
        if (vm == null)
        {
            // La cookie apunta a un corredor que ya no existe (por ejemplo, se recreó la base).
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Cuenta");
        }

        // Inscripciones pendientes = carreras todavía abiertas en las que estoy inscrito.
        vm.Inscripciones = await db.Resultados
            .Where(r => r.IdCorredor == miId && r.Carrera.Estado == EstadosCarrera.Abierta)
            .OrderBy(r => r.Carrera.Fecha)
            .Select(r => new InscripcionPerfilViewModel
            {
                IdCarrera = r.IdCarrera,
                Nombre = r.Carrera.Nombre,
                Distancia = r.Carrera.Distancia,
                Fecha = r.Carrera.Fecha,
                EdadMinima = r.Categoria.EdadMinima,
                EdadMaxima = r.Categoria.EdadMaxima
            })
            .ToListAsync();

        vm.TotalActividades = await db.Actividades.CountAsync(a => a.IdCorredor == miId);
        return View(vm);
    }

    public async Task<IActionResult> Ver(int id)
    {
        var miId = User.ObtenerId();
        if (id == miId)
            return RedirectToAction(nameof(Perfil));

        var vm = await CargarPerfilAsync<PerfilPublicoViewModel>(id);
        if (vm == null)
            return NotFound();

        vm.EsAmigo = await db.Amistades.AnyAsync(a => a.IdCorredor == miId && a.IdAmigo == id);
        return View(vm);
    }

    // Datos que comparten "Mi perfil" y el perfil público: datos personales, stats,
    // resultados de carreras terminadas y últimas actividades.
    private async Task<T?> CargarPerfilAsync<T>(int idCorredor) where T : PerfilPublicoViewModel, new()
    {
        var corredor = await db.Corredores
            .Where(c => c.IdCorredor == idCorredor)
            .Select(c => new
            {
                c.Nombre,
                c.FechaNac,
                c.FechaRegistro,
                c.KmsTotales,
                c.IdEquipo,
                Equipo = c.Equipo != null ? c.Equipo.Nombre : null
            })
            .FirstOrDefaultAsync();
        if (corredor == null)
            return null;

        var resultados = await db.Resultados
            .Where(r => r.IdCorredor == idCorredor && r.Carrera.Estado == EstadosCarrera.Terminada)
            .OrderByDescending(r => r.Carrera.Fecha)
            .Select(r => new ResultadoPerfilViewModel
            {
                IdCarrera = r.IdCarrera,
                Carrera = r.Carrera.Nombre,
                EdadMinima = r.Categoria.EdadMinima,
                EdadMaxima = r.Categoria.EdadMaxima,
                Tiempo = r.Tiempo!.Value,
                Ranking = r.Ranking!.Value
            })
            .ToListAsync();

        var actividades = await db.Actividades
            .Where(a => a.IdCorredor == idCorredor)
            .OrderByDescending(a => a.Fecha).ThenByDescending(a => a.IdActividad)
            .Take(UltimasActividades)
            .Select(a => new ActividadResumenViewModel
            {
                Id = a.IdActividad,
                Nombre = a.Nombre,
                Fecha = a.Fecha,
                Km = a.DistanciaKm,
                Tiempo = a.Tiempo,
                Tipo = a.Tipo,
                Likes = a.Likes.Count
            })
            .ToListAsync();

        return new T
        {
            IdCorredor = idCorredor,
            Nombre = corredor.Nombre,
            FechaRegistro = corredor.FechaRegistro,
            Edad = IsirunHelpers.Edad(corredor.FechaNac, DateTime.Today),
            IdEquipo = corredor.IdEquipo,
            Equipo = corredor.Equipo,
            KmsTotales = corredor.KmsTotales,
            CarrerasCorridas = resultados.Count,
            MejorRanking = resultados.Count > 0 ? resultados.Min(r => r.Ranking) : null,
            Resultados = resultados,
            Actividades = actividades
        };
    }
}
