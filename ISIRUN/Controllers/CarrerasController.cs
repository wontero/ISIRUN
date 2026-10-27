using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Vista del CORREDOR: catálogo, detalle de carrera abierta, ranking de carrera terminada e inscripción.
public class CarrerasController(IsirunContext db) : Controller
{
    // /Carreras?estado=abiertas&distancia=10&q=
    public async Task<IActionResult> Index(string estado = "abiertas", string? distancia = null, string? q = null)
    {
        var terminadas = estado == "terminadas";
        var estadoDb = terminadas ? EstadosCarrera.Terminada : EstadosCarrera.Abierta;

        var query = db.Carreras
            .AsNoTracking()
            .Where(c => c.Estado == estadoDb);

        if (!string.IsNullOrWhiteSpace(distancia) && decimal.TryParse(distancia, out var km))
            query = query.Where(c => c.Distancia == km);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(c =>
                c.Nombre.Contains(texto) ||
                c.Organizadoras.Any(o => o.Organizacion.Nombre.Contains(texto)));
        }

        query = terminadas
            ? query.OrderByDescending(c => c.Fecha)
            : query.OrderBy(c => c.Fecha);

        var filas = await query
            .Select(c => new
            {
                c.IdCarrera,
                c.Nombre,
                c.Distancia,
                c.Fecha,
                c.PrecioInscripcion,
                Categorias = c.Categorias.Count,
                Inscritos = c.Resultados.Count,
                Organiza = c.Organizadoras
                    .Where(o => o.EsCreadora)
                    .Select(o => o.Organizacion.Nombre)
                    .FirstOrDefault()
                    ?? c.Organizadoras.Select(o => o.Organizacion.Nombre).FirstOrDefault()
                    ?? "Sin organización"
            })
            .ToListAsync();

        var vm = new CarrerasIndexViewModel
        {
            Estado = terminadas ? "terminadas" : "abiertas",
            Distancia = distancia,
            Q = q,
            Carreras = filas.Select(c => new CarreraTarjetaViewModel
            {
                Id = c.IdCarrera,
                Distancia = Formatos.DistanciaCorta(c.Distancia),
                Fecha = Presentacion.FechaTarjeta(c.Fecha),
                Nombre = c.Nombre,
                Organiza = c.Organiza,
                Info = terminadas
                    ? $"{c.Inscritos} corredores en el ranking"
                    : $"{c.Categorias} categorías de edad",
                Precio = terminadas ? "" : Presentacion.Monto(c.PrecioInscripcion)
            }).ToList()
        };

        ViewData["Estado"] = vm.Estado;
        ViewData["Distancia"] = distancia;
        return View(vm);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var carrera = await db.Carreras
            .AsNoTracking()
            .Include(c => c.Categorias)
            .Include(c => c.Organizadoras).ThenInclude(o => o.Organizacion)
            .Include(c => c.Sponsors).ThenInclude(s => s.Organizacion)
            .Include(c => c.Resultados)
            .FirstOrDefaultAsync(c => c.IdCarrera == id);

        if (carrera == null) return NotFound();

        if (carrera.Estado == EstadosCarrera.Terminada)
            return RedirectToAction(nameof(Resultados), new { id });

        var estaInscrito = false;
        if (User.Identity?.IsAuthenticated == true && User.IsInRole(Roles.Corredor))
        {
            var miId = User.ObtenerId();
            estaInscrito = carrera.Resultados.Any(r => r.IdCorredor == miId);
        }

        var vm = new CarreraDetalleViewModel
        {
            Id = carrera.IdCarrera,
            Nombre = carrera.Nombre,
            DistanciaCorta = Formatos.DistanciaCorta(carrera.Distancia),
            DistanciaTexto = Formatos.Km(carrera.Distancia) + " km",
            FechaTexto = Presentacion.FechaLarga(carrera.Fecha),
            Precio = carrera.PrecioInscripcion,
            PrecioTexto = Presentacion.Monto(carrera.PrecioInscripcion),
            EstaInscrito = estaInscrito,
            Categorias = carrera.Categorias
                .OrderBy(c => c.EdadMinima)
                .Select(c => new CategoriaDetalleViewModel
                {
                    Rango = Presentacion.RangoEdad(c.EdadMinima, c.EdadMaxima),
                    Premios = c.Premios,
                    Inscritos = carrera.Resultados.Count(r => r.IdCategoria == c.IdCategoria)
                }).ToList(),
            Organizadoras = carrera.Organizadoras
                .OrderByDescending(o => o.EsCreadora)
                .ThenBy(o => o.Organizacion.Nombre)
                .Select(o => new OrganizacionVinculoViewModel
                {
                    Id = o.IdOrganizacion,
                    Iniciales = IsirunHelpers.Iniciales(o.Organizacion.Nombre),
                    Nombre = o.Organizacion.Nombre,
                    Desde = Formatos.FechaCorta(o.FechaInicio),
                    Equipo = o.TamanoEquipo
                }).ToList(),
            Sponsors = carrera.Sponsors
                .OrderByDescending(s => s.MontoAportado)
                .Select(s => new SponsorDetalleViewModel
                {
                    Id = s.IdOrganizacion,
                    Iniciales = IsirunHelpers.Iniciales(s.Organizacion.Nombre),
                    Nombre = s.Organizacion.Nombre,
                    Tipo = s.TipoParticipacion,
                    Monto = Presentacion.Monto(s.MontoAportado)
                }).ToList()
        };

        ViewData["Title"] = carrera.Nombre;
        return View(vm);
    }

    // /Carreras/Resultados/5?categoria=2
    public async Task<IActionResult> Resultados(int id, int? categoria = null)
    {
        var carrera = await db.Carreras
            .AsNoTracking()
            .Include(c => c.Categorias)
            .Include(c => c.Organizadoras).ThenInclude(o => o.Organizacion)
            .Include(c => c.Resultados).ThenInclude(r => r.Corredor).ThenInclude(c => c.Equipo)
            .Include(c => c.Resultados).ThenInclude(r => r.Categoria)
            .FirstOrDefaultAsync(c => c.IdCarrera == id);

        if (carrera == null) return NotFound();

        if (carrera.Estado != EstadosCarrera.Terminada)
            return RedirectToAction(nameof(Detalle), new { id });

        var categorias = carrera.Categorias.OrderBy(c => c.EdadMinima).ToList();
        var categoriaActual = categoria
            ?? categorias.FirstOrDefault()?.IdCategoria;

        if (categoriaActual.HasValue && categorias.All(c => c.IdCategoria != categoriaActual.Value))
            categoriaActual = categorias.FirstOrDefault()?.IdCategoria;

        var catActual = categorias.FirstOrDefault(c => c.IdCategoria == categoriaActual);
        var miId = User.Identity?.IsAuthenticated == true && User.IsInRole(Roles.Corredor)
            ? User.ObtenerId()
            : (int?)null;

        var ranking = carrera.Resultados
            .Where(r => r.IdCategoria == categoriaActual && r.Tiempo != null && r.Ranking != null)
            .OrderBy(r => r.Ranking)
            .Select(r => new RankingFilaViewModel
            {
                Pos = r.Ranking!.Value,
                Corredor = r.Corredor.Nombre,
                Equipo = r.Corredor.Equipo?.Nombre ?? "Sin equipo",
                Tiempo = IsirunHelpers.FormatoTiempo(r.Tiempo!.Value),
                Ritmo = IsirunHelpers.FormatoRitmo(r.Tiempo.Value, carrera.Distancia),
                Premio = Presentacion.PremioPuesto(catActual?.Premios ?? "", r.Ranking.Value),
                EsYo = miId.HasValue && r.IdCorredor == miId.Value
            }).ToList();

        MiResultadoViewModel? miResultado = null;
        if (miId.HasValue)
        {
            var mio = carrera.Resultados.FirstOrDefault(r => r.IdCorredor == miId.Value && r.Tiempo != null);
            if (mio != null)
            {
                miResultado = new MiResultadoViewModel
                {
                    Pos = mio.Ranking ?? 0,
                    Tiempo = IsirunHelpers.FormatoTiempo(mio.Tiempo!.Value),
                    Categoria = Presentacion.RangoEdad(mio.Categoria.EdadMinima, mio.Categoria.EdadMaxima),
                    Ritmo = IsirunHelpers.FormatoRitmo(mio.Tiempo.Value, carrera.Distancia)
                };
            }
        }

        var organiza = carrera.Organizadoras
            .Where(o => o.EsCreadora)
            .Select(o => o.Organizacion.Nombre)
            .FirstOrDefault()
            ?? carrera.Organizadoras.Select(o => o.Organizacion.Nombre).FirstOrDefault()
            ?? "Sin organización";

        var vm = new CarreraResultadosViewModel
        {
            Id = carrera.IdCarrera,
            Nombre = carrera.Nombre,
            FechaTexto = Formatos.FechaCorta(carrera.Fecha),
            DistanciaTexto = Formatos.Km(carrera.Distancia) + " km",
            Organiza = organiza,
            TotalCorredores = carrera.Resultados.Count,
            DistanciaKm = carrera.Distancia,
            CategoriaActual = categoriaActual,
            Categorias = categorias.Select(c => new CategoriaTabViewModel
            {
                Id = c.IdCategoria,
                Rango = Presentacion.RangoEdad(c.EdadMinima, c.EdadMaxima)
            }).ToList(),
            Ranking = ranking,
            MiResultado = miResultado
        };

        ViewData["Title"] = $"{carrera.Nombre} · Ranking";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Corredor)]
    public async Task<IActionResult> Inscribirse(int id)
    {
        var carrera = await db.Carreras
            .Include(c => c.Categorias)
            .Include(c => c.Resultados)
            .FirstOrDefaultAsync(c => c.IdCarrera == id);

        if (carrera == null) return NotFound();

        if (carrera.Estado != EstadosCarrera.Abierta)
        {
            TempData["Error"] = "Esta carrera ya no acepta inscripciones.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var miId = User.ObtenerId();
        if (carrera.Resultados.Any(r => r.IdCorredor == miId))
        {
            TempData["Error"] = "Ya estás inscrito en esta carrera.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var corredor = await db.Corredores.FindAsync(miId);
        if (corredor == null) return NotFound();

        var edad = IsirunHelpers.Edad(corredor.FechaNac, carrera.Fecha);
        var categoria = carrera.Categorias
            .FirstOrDefault(c => c.EdadMinima <= edad && edad <= c.EdadMaxima);

        if (categoria == null)
        {
            TempData["Error"] = "No hay una categoría para tu edad en esta carrera.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        db.Resultados.Add(new Resultado
        {
            IdCorredor = miId,
            IdCarrera = id,
            IdCategoria = categoria.IdCategoria,
            Tiempo = null,
            Ranking = null,
            FechaInscripcion = DateTime.Now
        });
        await db.SaveChangesAsync();

        TempData["Exito"] = "¡Listo! Quedaste inscrito. Verás tu resultado cuando la organización termine la carrera.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Corredor)]
    public async Task<IActionResult> CancelarInscripcion(int id)
    {
        var carrera = await db.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.IdCarrera == id);
        if (carrera == null) return NotFound();

        if (carrera.Estado != EstadosCarrera.Abierta)
        {
            TempData["Error"] = "Solo puedes cancelar la inscripción mientras la carrera esté abierta.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var miId = User.ObtenerId();
        var resultado = await db.Resultados.FindAsync(miId, id);
        if (resultado == null)
        {
            TempData["Error"] = "No tienes una inscripción en esta carrera.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        db.Resultados.Remove(resultado);
        await db.SaveChangesAsync();

        TempData["Exito"] = "Inscripción cancelada.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
