using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Vista del CORREDOR (solo lectura): lista de organizaciones y qué carreras organizan o patrocinan.
// No confundir con OrganizacionController (singular), que es el panel de la organización logueada.
public class OrganizacionesController(IsirunContext db) : Controller
{
    // /Organizaciones?id=3 muestra el detalle de la organización 3 al costado de la lista.
    public async Task<IActionResult> Index(int? id = null)
    {
        var orgs = await db.Organizaciones
            .AsNoTracking()
            .OrderBy(o => o.Nombre)
            .Select(o => new
            {
                o.IdOrganizacion,
                o.Nombre,
                o.Actividad
            })
            .ToListAsync();

        if (orgs.Count == 0)
            return View(new OrganizacionesIndexViewModel());

        var seleccionId = id ?? orgs[0].IdOrganizacion;
        if (orgs.All(o => o.IdOrganizacion != seleccionId))
            seleccionId = orgs[0].IdOrganizacion;

        var actual = orgs.First(o => o.IdOrganizacion == seleccionId);

        var organiza = await db.OrgCarreras
            .AsNoTracking()
            .Where(o => o.IdOrganizacion == seleccionId)
            .OrderBy(o => o.Carrera.Fecha)
            .Select(o => new
            {
                o.IdCarrera,
                Carrera = o.Carrera.Nombre,
                o.FechaInicio,
                o.TamanoEquipo,
                o.EsCreadora
            })
            .ToListAsync();

        var patrocina = await db.Sponsors
            .AsNoTracking()
            .Where(s => s.IdOrganizacion == seleccionId)
            .OrderBy(s => s.Carrera.Fecha)
            .Select(s => new
            {
                s.IdCarrera,
                Carrera = s.Carrera.Nombre,
                s.TipoParticipacion,
                s.MontoAportado
            })
            .ToListAsync();

        var vm = new OrganizacionesIndexViewModel
        {
            Organizaciones = orgs.Select(o => new OrganizacionResumenViewModel
            {
                Id = o.IdOrganizacion,
                Iniciales = IsirunHelpers.Iniciales(o.Nombre),
                Nombre = o.Nombre,
                Actividad = o.Actividad
            }).ToList(),
            Seleccionada = new OrganizacionDetalleViewModel
            {
                Id = actual.IdOrganizacion,
                Iniciales = IsirunHelpers.Iniciales(actual.Nombre),
                Nombre = actual.Nombre,
                Actividad = actual.Actividad,
                Organiza = organiza.Select(o => new CarreraVinculoOrgViewModel
                {
                    IdCarrera = o.IdCarrera,
                    Carrera = o.Carrera,
                    Detalle = (o.EsCreadora ? "Creadora · " : "")
                        + $"Desde {Formatos.FechaCorta(o.FechaInicio)} · equipo de {o.TamanoEquipo}"
                }).ToList(),
                Patrocina = patrocina.Select(s => new CarreraVinculoOrgViewModel
                {
                    IdCarrera = s.IdCarrera,
                    Carrera = s.Carrera,
                    Detalle = $"{s.TipoParticipacion} · {Presentacion.Monto(s.MontoAportado)}"
                }).ToList()
            }
        };

        return View(vm);
    }
}
