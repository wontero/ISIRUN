# ISIRUN

Aplicación web universitaria de carreras de running, desarrollada con C#, ASP.NET Core MVC (.NET 10), Entity Framework Core y SQL Server LocalDB.

Tiene dos tipos de cuenta:

- **Corredor:** se inscribe en carreras abiertas (la categoría se asigna sola según su edad el día de la carrera), ve el ranking de las carreras terminadas, registra entrenamientos, tiene amigos con un feed de actividades y likes, y se une a un equipo.
- **Organización:** crea carreras con categorías por edad, suma sponsors y co-organizadoras, termina la carrera (se simulan los tiempos y se arma el ranking de cada categoría) y consulta 2 reportes.

## Integrantes

| Integrante | Usuario de GitHub | Parte del proyecto |
|---|---|---|
| Mateo | [wontero](https://github.com/wontero) | Proyecto MVC, vistas, diseño y navegación |
| Victor | [VictorManrique01](https://github.com/VictorManrique01) | Base de datos: entidades, DbContext, migración, simulación y datos de prueba |
| Nicolás Colautti | [Niko1099](https://github.com/Niko1099) | Cuentas, roles, perfil del corredor, actividades, feed y amigos |
| Harold | [hnava08](https://github.com/hnava08) | Catálogo e inscripción a carreras, ranking, equipos, organizaciones y sponsors |
| Mauricio | [MauricioUri06](https://github.com/MauricioUri06) | Panel de la organización, crear/editar/eliminar/terminar carrera, reportes y entrega |

## Requisitos

- Windows con .NET 10 SDK.
- SQL Server Express LocalDB instalado, con la instancia `MSSQLLocalDB` (viene con Visual Studio).
- Herramienta `dotnet-ef` 10.0.12. Si no está instalada: `dotnet tool install --global dotnet-ef --version 10.0.12`.
- Opcional: Visual Studio 2026 con desarrollo web ASP.NET, o VS Code con C# Dev Kit.
- Opcional: SQL Server Management Studio para inspeccionar las tablas.

## Cómo crear la base y ejecutar

Desde la carpeta raíz del repositorio (donde está `ISIRUN.slnx`):

```powershell
dotnet restore ISIRUN.slnx
dotnet ef database update --project ISIRUN
dotnet run --project ISIRUN --launch-profile http
```

Abrir http://localhost:5136. También se puede abrir `ISIRUN.slnx` en Visual Studio y ejecutar el perfil `http`.

`dotnet ef database update` aplica la migración `Inicial`, que crea la base `IsirunDb` y sus 11 tablas. Al arrancar, la aplicación ejecuta `SeedData.InicializarAsync`: carga los datos de prueba **solo si la base está vacía**, en una sola transacción, así que reiniciar la app no duplica nada.

### Conexión a la base

La cadena de conexión está en `ISIRUN/appsettings.json`:

```text
Server=(localdb)\MSSQLLocalDB;Database=IsirunDb;Trusted_Connection=True;TrustServerCertificate=True
```

Usa autenticación de Windows: no contiene usuario ni contraseña. En Management Studio: servidor `(localdb)\MSSQLLocalDB`, base `IsirunDb`.

## Usuarios de prueba

| Tipo | Correo | Contraseña |
|---|---|---|
| Corredor (Ana Rojas) | ana.rojas@isirun.test | Prueba123! |
| Organización (Club Atlético Norte) | club.norte@isirun.test | Prueba123! |

Son cuentas ficticias. Todas las cuentas del seed usan esa contraseña, guardada como hash con `PasswordHasher` (nunca en texto plano).

Los datos de prueba incluyen:

- 3 equipos, 15 corredores (3 sin equipo) y 3 organizaciones.
- 4 carreras: 2 abiertas con inscritos pendientes y 2 terminadas con tiempos y ranking.
- 4 categorías por carrera: 18–29, 30–39, 40–49 y 50+ años.
- Sponsors, co-organizadoras, actividades, likes y amistades.

Las fechas se calculan respecto del día de la primera carga: las carreras abiertas quedan en el futuro y las terminadas en el pasado.

## Recorrido sugerido para probar

1. Entrar como **Ana Rojas** e inscribirse en una carrera abierta desde el catálogo. En "Mi perfil" aparece como pendiente.
2. Salir y entrar como **Club Atlético Norte**. En "Mis carreras", gestionar la carrera: se ven los inscritos por categoría como "Pendiente".
3. Apretar **Terminar carrera** y confirmar: se simulan los tiempos y aparece el ranking de cada categoría.
4. Abrir los **Reportes**.
5. Volver a entrar como Ana: la carrera muestra su resultado y sus kilómetros totales aumentaron.

## Reportes

Los dos reportes son de la organización logueada y solo usan datos de las carreras en las que participa. Ambos se pueden imprimir.

### 1. Inscripciones y recaudación (`/Reportes/Inscripciones`)

**Para qué sirve:** la organización ve cuántos inscritos y cuánto dinero entró por carrera y por categoría en un período.

- **Filtros:** fecha desde, fecha hasta (sobre la fecha de la carrera) y estado (abiertas, terminadas o todas).
- **Tabla:** una fila por carrera con sus inscritos en cada rango de edad, el total de inscritos y lo recaudado (inscritos × precio).
- **Totales:** cantidad de carreras, inscritos, recaudación total, promedio recaudado por carrera, fila de totales y un gráfico de barras con los inscritos por categoría.

### 2. Rendimiento por categoría o equipo (`/Reportes/Rendimiento`)

**Para qué sirve:** comparar el rendimiento de las categorías o de los equipos en una carrera terminada: quién ganó más podios y qué grupo es más rápido.

- **Filtros:** la carrera (solo las terminadas de la organización) y agrupar por categoría o por equipo.
- **Tabla:** por grupo, finalistas, podios (puestos 1 a 3 de su categoría), mejor tiempo con el nombre del corredor, tiempo promedio y ritmo promedio. Se ordena por podios y luego por tiempo promedio.
- **Totales:** finalistas, mejor tiempo de la carrera, tiempo promedio y ritmo promedio.

## Simulación al terminar una carrera

Cada inscrito recibe un ritmo aleatorio entre 4:00 y 8:00 min/km. El tiempo es ritmo × distancia y el ranking se calcula por separado en cada categoría (el más rápido es el 1). La distancia de la carrera se suma a los kilómetros totales de cada inscrito. Una carrera terminada ya no se puede editar, eliminar ni recibir inscripciones.

## Estructura

| Carpeta | Contenido |
|---|---|
| `ISIRUN/Models/` | Las 11 entidades del DER y las constantes `EstadosCarrera`. |
| `ISIRUN/Data/` | `IsirunContext` (claves compuestas, índices y reglas de borrado) y `SeedData`. |
| `ISIRUN/Controllers/` | Un controlador por módulo; leen y guardan con EF Core y validan las reglas de negocio. |
| `ISIRUN/ViewModels/` | Lo que necesita cada pantalla o formulario, con sus validaciones. |
| `ISIRUN/Views/` | Vistas Razor; solo muestran el `Model`. |
| `ISIRUN/Services/` | `SimulacionCarreraService`: tiempos y ranking al terminar una carrera. |
| `ISIRUN/Helpers/` | Formatos de tiempo, ritmo, fechas y montos, edad, iniciales, roles y `User.ObtenerId()`. |
| `ISIRUN/Migrations/` | Migración generada por EF Core. |

## DER

- `DER_actualizado.md`: diagrama de las 11 entidades (GitHub lo muestra directamente) y la tabla con la explicación de cada cambio respecto del DER original.
- `REDISENO_WEB.md`: pantallas y reglas de funcionamiento.
