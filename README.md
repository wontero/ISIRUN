# ISIRUN

Aplicación web universitaria de carreras de running, desarrollada con C#, ASP.NET Core MVC (.NET 10), Entity Framework Core y SQL Server LocalDB.

Los corredores podrán inscribirse en carreras, consultar rankings, registrar actividades, formar equipos y agregar amigos. Las organizaciones podrán gestionar carreras, categorías de edad, sponsors y coorganizadoras, y consultar reportes.

## Estado actual

Está terminada la base del turno de Victor: 11 entidades, contexto, migración inicial, funciones auxiliares, servicio de simulación y datos de prueba. Las vistas todavía muestran ejemplos y los controladores conservan las acciones pendientes de los siguientes turnos. El login real y la conexión de las pantallas a la base corresponden a esos turnos.

## Requisitos

- Windows con .NET 10 SDK.
- SQL Server Express LocalDB instalado, con la instancia `MSSQLLocalDB`.
- Herramienta `dotnet-ef` 10.0.12. Si no está instalada: `dotnet tool install --global dotnet-ef --version 10.0.12`.
- Opcional: Visual Studio 2026 con desarrollo web ASP.NET, o VS Code con C# Dev Kit.
- Opcional: SQL Server Management Studio para inspeccionar las tablas.

## Preparación y ejecución

Desde la carpeta raíz del repositorio (donde está `ISIRUN.slnx`):

```powershell
dotnet restore ISIRUN.slnx
dotnet ef database update --project ISIRUN
dotnet run --project ISIRUN --launch-profile http
```

Abrir http://localhost:5136. También se puede abrir `ISIRUN.slnx` en Visual Studio y ejecutar el perfil `http`.

La migración `Inicial` crea la base `IsirunDb` y sus tablas. Al arrancar la aplicación se ejecuta `SeedData.InicializarAsync`: carga los ejemplos solo si las 11 tablas de negocio están vacías, y confirma toda la carga en una sola transacción. No duplica datos ni reemplaza registros existentes al reiniciar. No usa `EnsureCreated`; los cambios de estructura se aplican con migraciones.

Cada integrante crea su propia base local con esos pasos. GitHub comparte el código y las migraciones; no sincroniza los registros de las bases.

## Conexión a la base

La conexión de desarrollo está en `ISIRUN/appsettings.json`:

```text
Server=(localdb)\MSSQLLocalDB;Database=IsirunDb;Trusted_Connection=True;TrustServerCertificate=True
```

En Management Studio: servidor `(localdb)\MSSQLLocalDB`, autenticación de Windows, base `IsirunDb`. La conexión no contiene usuario ni contraseña de SQL Server.

## Datos de prueba

| Tipo | Correo | Contraseña de demostración |
|---|---|---|
| Corredor (Ana Rojas) | ana.rojas@isirun.test | Prueba123! |
| Organización (Club Atlético Norte) | club.norte@isirun.test | Prueba123! |

Son cuentas ficticias para desarrollo. Todas las cuentas del seed usan esa contraseña de demostración, almacenada con `PasswordHasher` como hash. Estarán disponibles para iniciar sesión cuando se implemente el login real del turno de Nico.

La carga inicial contiene:

- 3 equipos, 15 corredores (3 sin equipo) y 3 organizaciones.
- 4 carreras: 2 abiertas con inscritos pendientes y 2 terminadas con tiempos y rankings.
- 4 categorías por carrera: 18–29, 30–39, 40–49 y 50–150 años.
- 57 inscripciones/resultados, 8 vínculos de organización y 4 sponsors.
- 30 actividades, 60 likes y 30 relaciones de amistad dirigidas (15 pares recíprocos).

Las fechas se calculan respecto del día de la primera carga: las carreras abiertas quedan en el futuro y las terminadas en el pasado. El seed no modifica esas fechas en los siguientes arranques.

Las carreras terminadas usan `SimulacionCarreraService.Terminar`: ritmo aleatorio de 240 a 480 segundos por kilómetro, tiempo según distancia y ranking independiente por categoría. Los tiempos pueden variar entre instalaciones. `KmsTotales` incluye las actividades y las carreras terminadas del corredor.

## Componentes para los siguientes turnos

- `Models/`: las 11 entidades y las constantes `EstadosCarrera`.
- `Data/IsirunContext.cs`: tablas, claves compuestas, índices y reglas de borrado.
- `Data/SeedData.cs`: datos iniciales; no debe llamarse para sobrescribir una base con datos.
- `Helpers/IsirunHelpers.cs`: `Edad(nacimiento, dia)`, `Iniciales(nombre)`, `FormatoTiempo(tiempo)` y `FormatoRitmo(tiempo, km)`.
- `Services/SimulacionCarreraService.cs`: `Terminar(carrera)` requiere los resultados cargados (`Include(c => c.Resultados)`). Cambia el estado y calcula tiempos/rankings en memoria. Quien lo llame debe sumar los kilómetros a los corredores y ejecutar `SaveChangesAsync`, como hace el seed. No repetir la suma al terminar una carrera que ya está terminada.
- `Migrations/`: migración inicial y descripción del modelo. Los demás aplican la migración existente; no generan otra `Inicial`.

## Reportes previstos

1. **Inscripciones y recaudación:** permite a la organización revisar inscritos e ingresos por carrera y categoría, con filtros de fecha y estado y totales.
2. **Rendimiento por categoría o equipo:** compara resultados de una carrera terminada, con finalistas, podios, mejores tiempos y promedios.

Las vistas de los reportes están diseñadas; las consultas reales se implementan en el turno de Mauricio.

## Documentación y equipo

- `DER_actualizado.md`: diagrama de las 11 entidades y explicación de cambios.
- `REDISENO_WEB.md`: pantallas y reglas de funcionamiento.
- Equipo de trabajo: Victor (base de datos), Nico (cuentas y funciones sociales), Harold (carreras, equipos y organizaciones), Mauricio (gestión, reportes y entrega). Mateo preparó la estructura visual inicial.
- Los integrantes completarán sus nombres y usuarios de GitHub para la entrega final.
