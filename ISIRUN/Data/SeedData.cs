using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Data;

public static class SeedData
{
    public static async Task InicializarAsync(IsirunContext db, SimulacionCarreraService simulacion)
    {
        // Una unica transaccion impide dejar una carga de ejemplo incompleta.
        await using var transaccion = await db.Database.BeginTransactionAsync();
        if (await db.Equipos.AnyAsync() || await db.Corredores.AnyAsync()
            || await db.Organizaciones.AnyAsync() || await db.Carreras.AnyAsync()
            || await db.CategoriasEdad.AnyAsync() || await db.Resultados.AnyAsync()
            || await db.OrgCarreras.AnyAsync() || await db.Sponsors.AnyAsync()
            || await db.Actividades.AnyAsync() || await db.LikesActividad.AnyAsync()
            || await db.Amistades.AnyAsync())
            return;

        var hoy = DateTime.Today;
        const string contrasenaPrueba = "Prueba123!";
        var equipos = new[]
        {
            new Equipo { Nombre = "Runners del Norte", FechaCreacion = hoy.AddYears(-2) },
            new Equipo { Nombre = "Los Madrugadores", FechaCreacion = hoy.AddMonths(-18) },
            new Equipo { Nombre = "Corre Sur", FechaCreacion = hoy.AddYears(-1) }
        };
        var organizaciones = new[]
        {
            new Organizacion { Nombre = "Club Atlético Norte", Actividad = "Club deportivo", Email = "club.norte@isirun.test" },
            new Organizacion { Nombre = "Municipalidad", Actividad = "Gobierno local", Email = "municipalidad@isirun.test" },
            new Organizacion { Nombre = "Deportes Andes", Actividad = "Tienda de artículos deportivos", Email = "deportes.andes@isirun.test" }
        };
        var hasherOrganizacion = new PasswordHasher<Organizacion>();
        foreach (var organizacion in organizaciones)
            organizacion.ContrasenaHash = hasherOrganizacion.HashPassword(organizacion, contrasenaPrueba);

        var datosCorredores = new (string Nombre, string Email, int Edad)[]
        {
            ("Ana Rojas", "ana.rojas@isirun.test", 27),
            ("Diego Muñoz", "diego.munoz@isirun.test", 25),
            ("Camila Vera", "camila.vera@isirun.test", 26),
            ("Matías Rojas", "matias.rojas@isirun.test", 24),
            ("Valentina Díaz", "valentina.diaz@isirun.test", 28),
            ("Nicolás Pérez", "nicolas.perez@isirun.test", 34),
            ("Carolina Soto", "carolina.soto@isirun.test", 37),
            ("Javier Torres", "javier.torres@isirun.test", 35),
            ("Paula Herrera", "paula.herrera@isirun.test", 44),
            ("Andrés Silva", "andres.silva@isirun.test", 47),
            ("Francisca López", "francisca.lopez@isirun.test", 45),
            ("Rodrigo Castro", "rodrigo.castro@isirun.test", 56),
            ("Marcela Fuentes", "marcela.fuentes@isirun.test", 59),
            ("Luis González", "luis.gonzalez@isirun.test", 60),
            ("Sofía Morales", "sofia.morales@isirun.test", 19)
        };
        var corredores = new List<Corredor>();
        var hasherCorredor = new PasswordHasher<Corredor>();
        for (var i = 0; i < datosCorredores.Length; i++)
        {
            var datos = datosCorredores[i];
            var corredor = new Corredor
            {
                Nombre = datos.Nombre,
                Email = datos.Email,
                FechaNac = hoy.AddYears(-datos.Edad).AddMonths(-2),
                FechaRegistro = hoy.AddMonths(-6),
                Equipo = i < 12 ? equipos[i % equipos.Length] : null
            };
            corredor.ContrasenaHash = hasherCorredor.HashPassword(corredor, contrasenaPrueba);
            corredores.Add(corredor);
        }

        var carreras = new[]
        {
            new Carrera { Nombre = "Maratón de la Ciudad", Distancia = 42, PrecioInscripcion = 35, Fecha = hoy.AddDays(20).AddHours(8) },
            new Carrera { Nombre = "10K Nocturna del Parque", Distancia = 10, PrecioInscripcion = 15, Fecha = hoy.AddDays(27).AddHours(20) },
            new Carrera { Nombre = "10K Primavera", Distancia = 10, PrecioInscripcion = 15, Fecha = hoy.AddDays(-14).AddHours(8) },
            new Carrera { Nombre = "Media Maratón Costanera", Distancia = 21, PrecioInscripcion = 25, Fecha = hoy.AddDays(-28).AddHours(8) }
        };
        var rangos = new[] { (Minima: 18, Maxima: 29), (Minima: 30, Maxima: 39), (Minima: 40, Maxima: 49), (Minima: 50, Maxima: 150) };
        for (var i = 0; i < carreras.Length; i++)
        {
            var carrera = carreras[i];
            foreach (var rango in rangos)
                carrera.Categorias.Add(new CategoriaEdad
                {
                    Carrera = carrera,
                    EdadMinima = rango.Minima,
                    EdadMaxima = rango.Maxima,
                    Premios = "1.º: $100; 2.º: $50; 3.º: medalla"
                });

            var creadora = i == 1 ? organizaciones[1] : organizaciones[0];
            var coorganizadora = i == 1 ? organizaciones[0] : organizaciones[1];
            carrera.Organizadoras.Add(new OrgCarrera
            {
                Carrera = carrera, Organizacion = creadora, EsCreadora = true,
                FechaInicio = carrera.Fecha.Date.AddMonths(-2), TamanoEquipo = 25
            });
            carrera.Organizadoras.Add(new OrgCarrera
            {
                Carrera = carrera, Organizacion = coorganizadora, EsCreadora = false,
                FechaInicio = carrera.Fecha.Date.AddMonths(-1), TamanoEquipo = 10
            });
            carrera.Sponsors.Add(new Sponsor
            {
                Carrera = carrera, Organizacion = organizaciones[2],
                MontoAportado = 1200 + i * 300, TipoParticipacion = "Indumentaria"
            });

            foreach (var corredor in corredores.Take(i == 0 ? 12 : 15))
            {
                var edad = IsirunHelpers.Edad(corredor.FechaNac, carrera.Fecha);
                var categoria = carrera.Categorias.Single(c => c.EdadMinima <= edad && edad <= c.EdadMaxima);
                carrera.Resultados.Add(new Resultado
                {
                    Carrera = carrera, Corredor = corredor, Categoria = categoria,
                    FechaInscripcion = i < 2 ? hoy.AddDays(-3) : carrera.Fecha.Date.AddDays(-7)
                });
            }
        }

        db.Equipos.AddRange(equipos);
        db.Organizaciones.AddRange(organizaciones);
        db.Corredores.AddRange(corredores);
        db.Carreras.AddRange(carreras);
        // Obtiene las claves generadas antes de agrupar resultados por IdCategoria.
        await db.SaveChangesAsync();

        foreach (var carrera in carreras.Skip(2))
        {
            simulacion.Terminar(carrera);
            foreach (var resultado in carrera.Resultados)
                resultado.Corredor.KmsTotales += carrera.Distancia;
        }

        var actividades = new List<Actividad>();
        var nombresActividad = new[] { "Rodaje suave", "Tempo por la costanera", "Subida al cerro", "Series 6×800", "Fondo largo" };
        for (var i = 0; i < corredores.Count; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                var distancia = j == 0 ? 8m : 12m + i % 3;
                var actividad = new Actividad
                {
                    Corredor = corredores[i], Nombre = nombresActividad[(i + j) % nombresActividad.Length],
                    Descripcion = "Entrenamiento de ejemplo para probar perfiles y el feed.",
                    DistanciaKm = distancia, Tiempo = TimeSpan.FromMinutes((double)distancia * (5.5 + i % 3 * 0.5)),
                    Tipo = i % 3 == 0 ? "Trail" : "Calle", Notas = "Datos de demostración.",
                    Fecha = hoy.AddDays(-j - 1)
                };
                for (var offset = 1; offset <= 2; offset++)
                    actividad.Likes.Add(new LikeActividad
                    {
                        Actividad = actividad, Corredor = corredores[(i + offset) % corredores.Count],
                        Fecha = actividad.Fecha.AddHours(18)
                    });
                corredores[i].KmsTotales += distancia;
                actividades.Add(actividad);
            }
        }
        db.Actividades.AddRange(actividades);

        for (var i = 0; i < corredores.Count; i++)
        {
            var corredor = corredores[i];
            var amigo = corredores[(i + 1) % corredores.Count];
            db.Amistades.AddRange(
                new Amistad { Corredor = corredor, Amigo = amigo, Fecha = hoy.AddMonths(-1) },
                new Amistad { Corredor = amigo, Amigo = corredor, Fecha = hoy.AddMonths(-1) });
        }

        await db.SaveChangesAsync();
        await transaccion.CommitAsync();
    }
}
