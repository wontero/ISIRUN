using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// CRUD de actividades (entrenamientos) del corredor logueado.
// KmsTotales del corredor se mantiene al día: sube al crear, se ajusta al editar y baja al eliminar.
[Authorize(Roles = Roles.Corredor)]
public class ActividadesController(IsirunContext db) : Controller
{
    private const string Vista = "Formulario";

    public IActionResult Crear()
    {
        return View(Vista, new ActividadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ActividadViewModel vm)
    {
        var tiempo = ValidarYConvertirTiempo(vm);
        if (!ModelState.IsValid)
            return View(Vista, vm);

        var corredor = await db.Corredores.FirstAsync(c => c.IdCorredor == User.ObtenerId());
        var actividad = new Actividad { IdCorredor = corredor.IdCorredor };
        CopiarDatos(vm, actividad, tiempo);
        db.Actividades.Add(actividad);
        corredor.KmsTotales += actividad.DistanciaKm;

        await db.SaveChangesAsync();
        TempData["Exito"] = "Actividad registrada.";
        return RedirectToAction("Perfil", "Corredor");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var actividad = await BuscarMiActividadAsync(id);
        if (actividad == null)
            return NotFound();

        var vm = new ActividadViewModel
        {
            IdActividad = actividad.IdActividad,
            Nombre = actividad.Nombre,
            Tipo = actividad.Tipo,
            DistanciaKm = actividad.DistanciaKm,
            Tiempo = IsirunHelpers.FormatoTiempo(actividad.Tiempo),
            Fecha = actividad.Fecha,
            Descripcion = actividad.Descripcion,
            Notas = actividad.Notas
        };
        ViewData["Editando"] = true;
        return View(Vista, vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ActividadViewModel vm)
    {
        var actividad = await BuscarMiActividadAsync(id);
        if (actividad == null)
            return NotFound();

        var tiempo = ValidarYConvertirTiempo(vm);
        if (!ModelState.IsValid)
        {
            ViewData["Editando"] = true;
            return View(Vista, vm);
        }

        // Se ajusta por la diferencia: si antes eran 8 km y ahora 10, se suman 2.
        var distanciaAnterior = actividad.DistanciaKm;
        CopiarDatos(vm, actividad, tiempo);
        actividad.Corredor.KmsTotales = Math.Max(0, actividad.Corredor.KmsTotales - distanciaAnterior + actividad.DistanciaKm);

        await db.SaveChangesAsync();
        TempData["Exito"] = "Actividad actualizada.";
        return RedirectToAction("Perfil", "Corredor");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var actividad = await BuscarMiActividadAsync(id);
        if (actividad == null)
            return NotFound();

        // Los likes de la actividad se borran en cascada.
        actividad.Corredor.KmsTotales = Math.Max(0, actividad.Corredor.KmsTotales - actividad.DistanciaKm);
        db.Actividades.Remove(actividad);

        await db.SaveChangesAsync();
        TempData["Exito"] = "Actividad eliminada.";
        return RedirectToAction("Perfil", "Corredor");
    }

    // Solo devuelve la actividad si es del corredor logueado (nadie edita lo de otro).
    private Task<Actividad?> BuscarMiActividadAsync(int id)
    {
        var miId = User.ObtenerId();
        return db.Actividades
            .Include(a => a.Corredor)
            .FirstOrDefaultAsync(a => a.IdActividad == id && a.IdCorredor == miId);
    }

    // Reglas que no se pueden poner como atributo en el ViewModel.
    // Devuelve el tiempo convertido a TimeSpan (o cero si el formato no era válido).
    private TimeSpan ValidarYConvertirTiempo(ActividadViewModel vm)
    {
        if (!ActividadViewModel.Tipos.Contains(vm.Tipo))
            ModelState.AddModelError(nameof(vm.Tipo), "Tipo de actividad no válido.");

        if (vm.Fecha?.Date > DateTime.Today)
            ModelState.AddModelError(nameof(vm.Fecha), "La fecha no puede ser futura.");

        if (!TimeSpan.TryParseExact(vm.Tiempo, @"h\:mm\:ss", null, out var tiempo))
            return TimeSpan.Zero; // el [RegularExpression] del ViewModel ya marcó el error

        if (tiempo <= TimeSpan.Zero)
            ModelState.AddModelError(nameof(vm.Tiempo), "El tiempo debe ser mayor que 0.");
        return tiempo;
    }

    private static void CopiarDatos(ActividadViewModel vm, Actividad actividad, TimeSpan tiempo)
    {
        actividad.Nombre = vm.Nombre.Trim();
        actividad.Tipo = vm.Tipo;
        actividad.DistanciaKm = vm.DistanciaKm!.Value;
        actividad.Tiempo = tiempo;
        actividad.Fecha = vm.Fecha!.Value.Date;
        actividad.Descripcion = vm.Descripcion?.Trim() ?? "";
        actividad.Notas = vm.Notas?.Trim() ?? "";
    }
}
