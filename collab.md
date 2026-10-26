# collab.md: guía para el equipo

> ## ⚠️ ESTE ARCHIVO SE BORRA ANTES DE ENTREGAR
> Es una guía interna. **No forma parte de la entrega.** Antes de generar el ZIP final y de la última versión del repo:
> ```bash
> git rm collab.md
> git commit -m "Elimina guía interna collab.md"
> git push
> ```
> Va a seguir en el historial de commits (git no borra el pasado), y está bien: no tiene contraseñas ni nada privado.
> Lo que importa es que **no esté en la versión final ni en el ZIP**.

> **Orden de trabajo: Victor → Nico → Harold → Mauricio, de a uno.** Tus tareas están en la [sección 15](#15-turnos-de-trabajo-git-y-registro-de-avance). Antes de empezar, lee el **registro de avance (15.4)** para saber en qué quedó la persona anterior.

Lee este archivo **completo** antes de escribir una línea de código. Si usas una IA para programar, **pásale este archivo entero como contexto** antes de pedirle cualquier cosa (ver la sección 14).

---

## Índice
1. [Qué es ISIRUN](#1-qué-es-isirun)
2. [Qué nos evalúan (rúbrica resumida)](#2-qué-nos-evalúan-rúbrica-resumida)
3. [Qué ya está hecho y qué falta](#3-qué-ya-está-hecho-y-qué-falta)
4. [Cómo correr el proyecto](#4-cómo-correr-el-proyecto)
5. [Estructura del proyecto y dónde va cada cosa](#5-estructura-del-proyecto-y-dónde-va-cada-cosa)
6. [Reglas que NO se negocian](#6-reglas-que-no-se-negocian)
7. [Base de datos con Entity Framework Core](#7-base-de-datos-con-entity-framework-core)
8. [Login y roles](#8-login-y-roles)
9. [Mapa de pantallas → controlador → vista → ViewModel](#9-mapa-de-pantallas--controlador--vista--viewmodel)
10. [Cómo conectar una vista con datos reales (paso a paso)](#10-cómo-conectar-una-vista-con-datos-reales-paso-a-paso)
11. [Lógica de negocio importante](#11-lógica-de-negocio-importante)
12. [Los 2 reportes](#12-los-2-reportes)
13. [Validaciones y mensajes](#13-validaciones-y-mensajes)
14. [Cómo usar la IA sin romper el proyecto](#14-cómo-usar-la-ia-sin-romper-el-proyecto)
15. [Git: ramas, commits y reparto del trabajo](#15-git-ramas-commits-y-reparto-del-trabajo)
16. [Checklist de entrega](#16-checklist-de-entrega)

---

## 1. Qué es ISIRUN

Una app web de **carreras de running** con **dos tipos de cuenta**:

| Tipo | Qué hace |
|---|---|
| **Corredor** | Se inscribe en carreras abiertas, ve rankings de carreras terminadas, registra entrenamientos (actividades), tiene amigos, da likes y se une a un equipo. |
| **Organización** | Crea carreras con categorías por edad, suma sponsors y co-organizadoras, **termina la carrera** (se simulan los tiempos y se arma el ranking) y ve **2 reportes**. |

### Ciclo de vida de una carrera (lo más importante de todo el proyecto)
```
   [Organización crea la carrera]
              │
              ▼
   ┌──────────────────────┐   Corredores se inscriben → se crea un Resultado
   │   Estado = Abierta   │   con Tiempo = NULL y Ranking = NULL ("Pendiente")
   └──────────────────────┘
              │  Organización aprieta "Terminar carrera" y confirma
              ▼
   ┌──────────────────────┐   Se simula un tiempo para cada inscrito
   │  Estado = Terminada  │   y se calcula el Ranking dentro de cada categoría
   └──────────────────────┘
```
- En una carrera **Abierta**: se puede inscribir, cancelar inscripción, editar y eliminar. **No** se muestra ranking.
- En una carrera **Terminada**: **no** se puede inscribir, editar ni eliminar. **Sí** se muestra el ranking.

### Documentos de referencia (en la raíz del repo)
- `REDISENO_WEB.md`: lista de todas las pantallas y qué hace cada una.
- `DER_actualizado.md`: **el DER que hay que implementar**, más la tabla de cambios respecto del original (eso también se entrega).
- Diseño visual en Claude (canvas): lo tiene Mateo. Las vistas del proyecto ya copian ese diseño, así que **no hace falta abrirlo para programar**.

---

## 2. Qué nos evalúan (rúbrica resumida)

| Criterio | Pts | Qué significa en la práctica |
|---|---|---|
| Funcionamiento y persistencia | 20 | Todo guarda y lee de la base con **EF Core**. CRUD real. Relaciones y validaciones bien. |
| Estructura y organización | 15 | MVC limpio: lógica en controladores/servicios, **nada de lógica en las vistas**, **ViewModels** para formularios, nombres claros, **sin código muerto ni duplicado**. |
| Interfaces | 15 | Ya está casi todo hecho. Hay que mostrar **nombres en vez de IDs**, mensajes de error y de confirmación. |
| Reportes | 15 | 2 reportes con **datos de la base, filtros y totales**. Están diseñados (sección 12). |
| DER y coherencia | 10 | El código tiene que coincidir con `DER_actualizado.md`. Se entregan los 2 DER y la explicación. |
| Entrega y documentación | 5 | README completo, ZIP **sin** `bin`, `obj`, `.vs`. Repo accesible. **Sin contraseñas reales.** |
| GitHub individual | 10 | **Cada uno** hace commits **propios**, con aportes reales y mensajes descriptivos. |
| Pregunta individual | 10 | El profe le pregunta a **cada uno** algo del proyecto. **Tienes que entender lo que subiste.** |

**80 pts son grupales y 20 individuales** (commits + pregunta). Si la IA hizo tu parte y no la entiendes, pierdes esos 20.

---

## 3. Qué ya está hecho y qué falta

### ✅ Hecho
- Proyecto **ASP.NET Core MVC (.NET 10)**: `ISIRUN/`, solución `ISIRUN.slnx`.
- **Todas las vistas** (`Views/`) con el diseño final, **con datos de ejemplo escritos a mano** dentro de cada vista.
- **Todos los controladores** con sus acciones. Hoy solo devuelven la vista o redirigen. Cada uno tiene comentarios `// TODO:` que dicen exactamente qué falta.
- Navegación completa: todos los links y formularios llevan a donde tienen que ir (probado: todas las rutas responden 200).
- CSS global con todos los estilos (`wwwroot/css/site.css`).
- Mensajes de éxito/error (`TempData["Exito"]` / `TempData["Error"]`) que se muestran solos.
- Diálogos de confirmación para eliminar (`<dialog>` + `site.js`).

### ❌ Falta (esto es lo que tienen que hacer)
1. **Modelos (entidades)** según el DER → carpeta `Models/`.
2. **DbContext + EF Core + migraciones** → carpeta `Data/`.
3. **Login real** con cookies y roles (Corredor / Organizacion).
4. **ViewModels** para cada pantalla con formulario o datos combinados → carpeta `ViewModels/`.
5. **Reemplazar los datos de ejemplo** de cada vista por `@model` (sección 10).
6. **La lógica** de cada `// TODO` de los controladores.
7. **Validaciones** (DataAnnotations + reglas de negocio) y mensajes.
8. Los **2 reportes** con consultas reales.
9. **Datos semilla** (seed) para que la app no arranque vacía en la presentación.
10. **README.md** (lo que pide la rúbrica, sección 16).

Para ver todo lo pendiente en Visual Studio: **Ver → Lista de tareas** (View → Task List) muestra todos los `TODO`. También puedes buscar `DATOS DE EJEMPLO` en las vistas.

---

## 4. Cómo correr el proyecto

Requisitos: **.NET 10 SDK** y **Visual Studio 2026** (o VS Code con C# Dev Kit), más **SQL Server** (LocalDB viene con Visual Studio).

```bash
git clone <url-del-repo>
cd ISIRUNWEB
dotnet run --project ISIRUN
```
Abre http://localhost:5136. En Visual Studio basta con abrir `ISIRUN.slnx` y apretar ▶.

> Si ves un error de versión de .NET, instala el SDK de .NET 10 (https://dotnet.microsoft.com/download). **No cambien el `TargetFramework`** sin avisar al grupo.

---

## 5. Estructura del proyecto y dónde va cada cosa

```
ISIRUNWEB/
├── ISIRUN.slnx
├── DER_actualizado.md          ← DER a implementar (se entrega)
├── REDISENO_WEB.md             ← lista de pantallas
├── collab.md                   ← ESTE archivo (se borra antes de entregar)
└── ISIRUN/
    ├── Program.cs              ← configuración: EF Core, login, rutas
    ├── appsettings.json        ← cadena de conexión (SIN contraseñas reales)
    ├── Controllers/            ← reciben la petición, llaman a la base, devuelven la vista
    ├── Models/                 ← ENTIDADES de la base (una clase por tabla del DER)   [CREAR]
    ├── ViewModels/             ← clases para formularios y pantallas                 [CREAR]
    ├── Data/IsirunContext.cs   ← DbContext de EF Core                                [CREAR]
    ├── Services/               ← lógica reutilizable (simulación, edad, etc.)         [OPCIONAL]
    ├── Migrations/             ← las genera EF, NO se editan a mano                   [AUTO]
    ├── Views/
    │   ├── Shared/_Layout.cshtml        ← esqueleto de todas las páginas
    │   ├── Shared/_NavCorredor.cshtml   ← barra del corredor
    │   ├── Shared/_NavOrganizacion.cshtml
    │   ├── Shared/_Mensajes.cshtml      ← muestra TempData["Exito"] / ["Error"]
    │   └── <Controlador>/<Accion>.cshtml
    └── wwwroot/css/site.css    ← TODOS los estilos
```

### Qué va en cada capa (esto lo preguntan en la evaluación individual)
| Capa | Sí | No |
|---|---|---|
| **Model** (entidad) | Propiedades = columnas, `[Key]`, navegación (`ICollection<...>`), `[Required]`, `[StringLength]` | Lógica de pantallas |
| **ViewModel** | Lo que una pantalla necesita: campos del formulario + listas para `<select>` + validaciones | Acceso a la base |
| **Controller** | Leer/guardar con el DbContext, validar reglas, `TempData`, `RedirectToAction` | HTML |
| **View** | Mostrar `Model`, `asp-for`, `foreach`, `if` simples para mostrar u ocultar | Consultas a la base, cálculos de negocio |

### Nombres (consistentes en TODO el proyecto)
- Clases en **PascalCase y singular**: `Carrera`, `CategoriaEdad`, `OrgCarrera`, `LikeActividad`.
- `DbSet` en **plural**: `Carreras`, `CategoriasEdad`, `Resultados`.
- Claves: `IdCarrera`, `IdCorredor`, etc. (igual que el DER).
- ViewModels terminan en `ViewModel`: `CarreraFormViewModel`, `PerfilViewModel`.
- **Sin tildes ni ñ en el código** (`Contrasena`, `TamanoEquipo`), pero **sí** en los textos que ve el usuario.

---

## 6. Reglas que NO se negocian

1. **No usar `style="..."` en las vistas.** Todo el estilo está en `site.css` (la única excepción ya existe: el ancho de las barras del reporte). Si necesitas un estilo nuevo, agrégalo como clase en `site.css`, arriba de la sección "Celular".
2. **No agregar Bootstrap, Tailwind ni ninguna librería de CSS.** Se quitó Bootstrap a propósito: rompe el diseño.
3. **No cambiar el diseño de las vistas.** Solo reemplazar datos de ejemplo por datos reales. Si algo de diseño hay que cambiar, se avisa en el grupo primero.
4. **Nada de lógica de negocio en las vistas ni en JavaScript.** Ni consultas, ni cálculos de ranking. Eso va en C#.
5. **Todo POST lleva `[ValidateAntiForgeryToken]`** (ya está puesto) y los formularios usan `asp-action` (eso agrega el token solo).
6. **Después de un POST exitoso siempre `RedirectToAction`** (patrón PRG), nunca `return View()`. Si hay errores de validación: `return View(vm)`.
7. **Contraseñas: nunca en texto plano.** Se usa `PasswordHasher` (sección 8).
8. **Nunca subir contraseñas, cadenas de conexión con contraseña ni API keys.** Usa LocalDB (no tiene contraseña) o `dotnet user-secrets`.
9. **Compila antes de cada commit** (`dotnet build`). Un commit que no compila rompe el trabajo de todos.
10. **Borra lo que no se usa.** Código comentado viejo, clases de prueba, `using` sin usar, archivos de la plantilla: la rúbrica penaliza el código sin uso.

---

## 7. Base de datos con Entity Framework Core

### 7.1 Instalar paquetes (una sola persona, una sola vez)
```bash
cd ISIRUN
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.*
dotnet tool install --global dotnet-ef
```

### 7.2 Entidades (`Models/`)
Una clase por tabla de `DER_actualizado.md`. Ejemplo **completo** de una (las demás siguen el mismo patrón):

```csharp
namespace ISIRUN.Models;

public class Carrera
{
    [Key]
    public int IdCarrera { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [Column(TypeName = "decimal(6,2)")]
    public decimal Distancia { get; set; }            // en km

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioInscripcion { get; set; }

    public DateTime Fecha { get; set; }

    [Required, StringLength(20)]
    public string Estado { get; set; } = EstadosCarrera.Abierta;

    public ICollection<CategoriaEdad> Categorias { get; set; } = new List<CategoriaEdad>();
    public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
    public ICollection<OrgCarrera> Organizadoras { get; set; } = new List<OrgCarrera>();
    public ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
}

public static class EstadosCarrera
{
    public const string Abierta = "Abierta";
    public const string Terminada = "Terminada";
}
```
- `Resultado.Tiempo` es `TimeSpan?` y `Resultado.Ranking` es `int?` (nullable, porque quedan vacíos hasta terminar).
- `Corredor.IdEquipo` es `int?` (el equipo es opcional).
- Usa las constantes `EstadosCarrera.Abierta` y `EstadosCarrera.Terminada`. **Nunca** escribas `"Abierta"` suelto en varios lados.

### 7.3 DbContext (`Data/IsirunContext.cs`)
Las **claves compuestas** y los **cascade** se configuran acá. SQL Server **no deja** crear la base si hay "múltiples caminos de cascada", así que **cópialo tal cual**:

```csharp
using ISIRUN.Models;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Data;

public class IsirunContext(DbContextOptions<IsirunContext> options) : DbContext(options)
{
    public DbSet<Corredor> Corredores => Set<Corredor>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<CategoriaEdad> CategoriasEdad => Set<CategoriaEdad>();
    public DbSet<Resultado> Resultados => Set<Resultado>();
    public DbSet<Organizacion> Organizaciones => Set<Organizacion>();
    public DbSet<OrgCarrera> OrgCarreras => Set<OrgCarrera>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<LikeActividad> LikesActividad => Set<LikeActividad>();
    public DbSet<Amistad> Amistades => Set<Amistad>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Claves compuestas (ver DER)
        b.Entity<Resultado>().HasKey(r => new { r.IdCorredor, r.IdCarrera });
        b.Entity<OrgCarrera>().HasKey(o => new { o.IdCarrera, o.IdOrganizacion });
        b.Entity<Sponsor>().HasKey(s => new { s.IdCarrera, s.IdOrganizacion });
        b.Entity<LikeActividad>().HasKey(l => new { l.IdActividad, l.IdCorredor });
        b.Entity<Amistad>().HasKey(a => new { a.IdCorredor, a.IdAmigo });

        // Emails únicos
        b.Entity<Corredor>().HasIndex(c => c.Email).IsUnique();
        b.Entity<Organizacion>().HasIndex(o => o.Email).IsUnique();

        // Resultado -> Categoria: SIN cascada (la carrera ya borra sus resultados)
        b.Entity<Resultado>().HasOne(r => r.Categoria).WithMany(c => c.Resultados)
            .HasForeignKey(r => r.IdCategoria).OnDelete(DeleteBehavior.NoAction);

        // Like -> Corredor: SIN cascada (la actividad ya borra sus likes)
        b.Entity<LikeActividad>().HasOne(l => l.Corredor).WithMany()
            .HasForeignKey(l => l.IdCorredor).OnDelete(DeleteBehavior.NoAction);

        // Amistad tiene DOS FK al mismo Corredor: hay que decir cuál es cuál
        b.Entity<Amistad>().HasOne(a => a.Corredor).WithMany(c => c.Amigos)
            .HasForeignKey(a => a.IdCorredor).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Amistad>().HasOne(a => a.Amigo).WithMany()
            .HasForeignKey(a => a.IdAmigo).OnDelete(DeleteBehavior.NoAction);

        // Corredor -> Equipo opcional: si se borra el equipo, el corredor queda sin equipo
        b.Entity<Corredor>().HasOne(c => c.Equipo).WithMany(e => e.Corredores)
            .HasForeignKey(c => c.IdEquipo).OnDelete(DeleteBehavior.SetNull);
    }
}
```

### 7.4 Conexión (`appsettings.json` y `Program.cs`)
```json
"ConnectionStrings": {
  "IsirunDb": "Server=(localdb)\\MSSQLLocalDB;Database=IsirunDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```
LocalDB **no usa contraseña**, así que se puede subir. Si alguien usa SQL Server con usuario y contraseña, **no lo sube**: lo pone con `dotnet user-secrets set "ConnectionStrings:IsirunDb" "..."`.

En `Program.cs`, antes de `builder.Build()`:
```csharp
builder.Services.AddDbContext<IsirunContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("IsirunDb")));
```

### 7.5 Migraciones
```bash
cd ISIRUN
dotnet ef migrations add Inicial
dotnet ef database update
```
- **Una sola persona** crea la migración `Inicial`, la sube, y **el resto hace `git pull` + `dotnet ef database update`**.
- Si cambias un modelo: `dotnet ef migrations add <QueCambio>` (por ejemplo `AgregaFechaACarrera`). **Nunca** edites ni borres migraciones ya subidas.
- Si dos personas crean migraciones a la vez, hay conflicto. **Avisen en el grupo antes de crear una.**

### 7.6 Datos semilla
Para la presentación, la base tiene que tener datos: 2 organizaciones, 4 carreras (2 abiertas y 2 terminadas, con ranking), unos 15 corredores, 3 equipos, actividades y amistades. Hagan una clase `Data/SeedData.cs` que inserte todo **si la base está vacía** y se llame desde `Program.cs` al arrancar. Usen los nombres de ejemplo que ya están en las vistas. Para las carreras terminadas, inscriban a los corredores y llamen al mismo método de "Terminar carrera" (sección 11.2).

---

## 8. Login y roles

Se usa **autenticación por cookies** (sin ASP.NET Identity completo: es más simple y se explica mejor).

`Program.cs`:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => { o.LoginPath = "/Cuenta/Login"; o.AccessDeniedPath = "/Cuenta/Login"; });
// ...después de app.UseRouting():
app.UseAuthentication();   // ANTES de UseAuthorization
app.UseAuthorization();
```

Al iniciar sesión (`CuentaController.Login` POST):
```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, corredor.IdCorredor.ToString()),
    new(ClaimTypes.Name, corredor.Nombre),
    new(ClaimTypes.Role, "Corredor")          // o "Organizacion"
};
var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
```
- Para leer el id logueado: `int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)`. Hagan **un** método de extensión `User.ObtenerId()` y úsenlo en todos lados (no lo copien en cada controlador).
- `[Authorize(Roles = "Corredor")]` en: `CorredorController`, `ActividadesController`, `FeedController`, `AmigosController`, `EquiposController`, y en las acciones POST de `CarrerasController`.
- `[Authorize(Roles = "Organizacion")]` en: `OrganizacionController` y `ReportesController`.
- `Carreras/Index`, `Carreras/Detalle`, `Carreras/Resultados`, `Organizaciones` y `Home` quedan públicas.
- En `_Layout.cshtml` hay un `TODO`: cuando haya login, la barra se elige por rol. En `_NavCorredor` y `_NavOrganizacion` hay que poner las iniciales reales en lugar de "AR" y "CA".

**Contraseñas** (ya viene incluido en ASP.NET, no hay que instalar nada):
```csharp
var hasher = new PasswordHasher<Corredor>();
corredor.ContrasenaHash = hasher.HashPassword(corredor, vm.Contrasena);                         // registrar
var ok = hasher.VerifyHashedPassword(corredor, corredor.ContrasenaHash, vm.Contrasena)
         != PasswordVerificationResult.Failed;                                                 // login
```
**Login**: se busca el email primero en `Corredores` y después en `Organizaciones`. Si no está, o la contraseña no coincide: `ModelState.AddModelError("", "Correo o contraseña incorrectos.")` (el mismo mensaje en los dos casos, por seguridad). **Registro**: el email no puede existir en **ninguna** de las dos tablas.

> ⚠️ El proyecto WPF viejo tenía este error en el login: `WHERE Nombre = @n OR Email = @n AND PasswordHash = @h`. Como el `AND` se evalúa antes que el `OR`, cualquiera entraba con solo el nombre de usuario. No lo repitan: con EF y `PasswordHasher` no pasa.

---

## 9. Mapa de pantallas → controlador → vista → ViewModel

Los ViewModels son **sugerencias de nombre**. Lo que importa es que existan y tengan lo que la vista muestra.

### Públicas
| Pantalla | URL | Vista | ViewModel sugerido |
|---|---|---|---|
| Inicio | `/` | `Home/Index` | `List<CarreraTarjetaViewModel>` (3 próximas abiertas) |
| Login | `/Cuenta/Login` | `Cuenta/Login` | `LoginViewModel` (Email, Contrasena) |
| Registro | `/Cuenta/Registro` | `Cuenta/Registro` | `RegistroViewModel` (Tipo + campos de los dos tipos + `SelectList Equipos`) |

### Corredor
| Pantalla | URL | Vista | ViewModel sugerido |
|---|---|---|---|
| Mi perfil | `/Corredor/Perfil` | `Corredor/Perfil` | `PerfilViewModel` (datos, stats, inscripciones, resultados, actividades) |
| Perfil de otro | `/Corredor/Ver/5` | `Corredor/Ver` | `PerfilPublicoViewModel` |
| Crear/editar actividad | `/Actividades/Crear`, `/Actividades/Editar/5` | `Actividades/Formulario` (la misma para las dos) | `ActividadViewModel` |
| Feed | `/Feed` | `Feed/Index` | `List<FeedItemViewModel>` |
| Catálogo | `/Carreras?estado=abiertas&distancia=10&q=` | `Carreras/Index` | `CarrerasIndexViewModel` (filtros + lista) |
| Carrera abierta | `/Carreras/Detalle/5` | `Carreras/Detalle` | `CarreraDetalleViewModel` (+ `EstaInscrito`) |
| Carrera terminada | `/Carreras/Resultados/5?categoria=2` | `Carreras/Resultados` | `CarreraResultadosViewModel` (+ `MiResultado`) |
| Equipos | `/Equipos` | `Equipos/Index` | `List<EquipoResumenViewModel>` |
| Equipo | `/Equipos/Detalle/5` | `Equipos/Detalle` | `EquipoDetalleViewModel` |
| Organizaciones | `/Organizaciones?id=3` | `Organizaciones/Index` | `OrganizacionesIndexViewModel` |
| Amigos | `/Amigos?q=` | `Amigos/Index` | `AmigosViewModel` |

### Organización
| Pantalla | URL | Vista | ViewModel sugerido |
|---|---|---|---|
| Mis carreras | `/Organizacion/Panel` | `Organizacion/Panel` | `PanelOrganizacionViewModel` |
| Gestionar carrera | `/Organizacion/Carrera/5?categoria=2` | `Organizacion/Carrera` | `GestionCarreraViewModel` |
| Crear/editar carrera | `/Organizacion/CrearCarrera`, `/Organizacion/EditarCarrera/5` | `Organizacion/FormularioCarrera` | `CarreraFormViewModel` + `List<CategoriaFormViewModel>` |
| Sponsors y co-organizadoras | `/Organizacion/Vinculos/5` | `Organizacion/Vinculos` | `VinculosViewModel` |
| Perfil de la organización | `/Organizacion/Perfil` | `Organizacion/Perfil` | `PerfilOrganizacionViewModel` |
| Reporte 1 | `/Reportes/Inscripciones?desde=&hasta=&estado=` | `Reportes/Inscripciones` | `ReporteInscripcionesViewModel` |
| Reporte 2 | `/Reportes/Rendimiento?carrera=4&agrupar=equipo` | `Reportes/Rendimiento` | `ReporteRendimientoViewModel` |

### Eliminaciones (todas con diálogo de confirmación, ya hechas en la vista)
| Qué | Acción POST | Regla |
|---|---|---|
| Actividad | `Actividades/Eliminar/{id}` | Solo el dueño. Resta sus km de `KmsTotales`. |
| Inscripción | `Carreras/CancelarInscripcion/{id}` | Solo si la carrera está Abierta. |
| Amigo | `Amigos/Quitar/{id}` | |
| Carrera | `Organizacion/EliminarCarrera/{id}` | Solo la **creadora** y solo si está **Abierta**. Cascada. |
| Vínculo | `Organizacion/QuitarVinculo/{id}` | La creadora no se puede quitar. |
| Categoría | botón "Quitar" en el formulario de carrera | No se puede quitar una categoría que ya tiene inscritos. |

---

## 10. Cómo conectar una vista con datos reales (paso a paso)

Cada vista empieza con un bloque así:
```cshtml
@{
    ViewData["Title"] = "Equipos";
    ViewData["Nav"] = "equipos";

    // DATOS DE EJEMPLO — reemplazar por @model IEnumerable<EquipoResumenViewModel>
    var equipos = new[]
    {
        new { Id = 1, Iniciales = "RN", Nombre = "Runners del Norte", Desde = "feb 2024", Miembros = 18, Kms = "6.240", EsMio = true },
        ...
    };
}
```
**Los nombres de los campos del ejemplo ya son los que necesita tu ViewModel.** Pasos:

**1. Crear el ViewModel** con esos mismos campos (`ViewModels/EquipoResumenViewModel.cs`):
```csharp
public class EquipoResumenViewModel
{
    public int Id { get; set; }
    public string Iniciales { get; set; } = "";
    public string Nombre { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public int Miembros { get; set; }
    public decimal Kms { get; set; }
    public bool EsMio { get; set; }
}
```
**2. Llenarlo en el controlador:**
```csharp
public async Task<IActionResult> Index()
{
    var miId = User.ObtenerId();
    var miEquipo = await _db.Corredores.Where(c => c.IdCorredor == miId).Select(c => c.IdEquipo).FirstAsync();
    var equipos = await _db.Equipos
        .Select(e => new EquipoResumenViewModel
        {
            Id = e.IdEquipo,
            Nombre = e.Nombre,
            FechaCreacion = e.FechaCreacion,
            Miembros = e.Corredores.Count,
            Kms = e.Corredores.Sum(c => c.KmsTotales),
            EsMio = e.IdEquipo == miEquipo
        })
        .OrderBy(e => e.Nombre)
        .ToListAsync();
    return View(equipos);
}
```
**3. En la vista:** borrar el bloque de ejemplo, agregar `@model IEnumerable<EquipoResumenViewModel>` arriba, y cambiar `@foreach (var e in equipos)` por `@foreach (var e in Model)`. Las fechas se formatean en la vista: `@e.FechaCreacion.ToString("MMM yyyy")`.

**4. En formularios**, cambiar cada `<input id="X" name="X">` por `<input asp-for="X" class="input" />` y agregar debajo `<span asp-validation-for="X"></span>`. **Mantengan las clases CSS** (`class="input"`, etc.).

**5. Las iniciales** ("RN", "AR") no se guardan en la base. Hagan **un** helper `Iniciales(string nombre)` que tome la primera letra de las 2 primeras palabras, y úsenlo en todos lados.

> El `@{ var pos = 1; }` y los `if` para mostrar u ocultar que hay en las vistas están bien. Lo que **no** puede quedar en la vista es: consultas, `Sum` sobre datos de la base, cálculo de rankings, etc.

---

## 11. Lógica de negocio importante

### 11.1 Inscribirse (`CarrerasController.Inscribirse`)
1. La carrera existe y su `Estado == Abierta`. Si no: `TempData["Error"]`.
2. El corredor no tiene ya un `Resultado` en esa carrera.
3. Calcular la **edad del corredor el día de la carrera**:
   ```csharp
   int Edad(DateTime nacimiento, DateTime dia)
   {
       var edad = dia.Year - nacimiento.Year;
       if (nacimiento.Date > dia.Date.AddYears(-edad)) edad--;
       return edad;
   }
   ```
4. Buscar la `CategoriaEdad` **de esa carrera** con `EdadMinima <= edad <= EdadMaxima`. Si no hay: `TempData["Error"] = "No hay una categoría para tu edad en esta carrera."`.
5. Crear `Resultado { IdCorredor, IdCarrera, IdCategoria, Tiempo = null, Ranking = null, FechaInscripcion = DateTime.Now }`.
6. `TempData["Exito"]` y `RedirectToAction("Detalle", new { id })`.

### 11.2 Terminar carrera: la simulación (`OrganizacionController.TerminarCarrera`)
**Regla:** cada inscrito recibe un **ritmo aleatorio entre 4:00 y 8:00 min/km** (240 a 480 segundos por km). Tiempo = ritmo × distancia. El ranking se calcula **por separado en cada categoría** (el más rápido es 1).

Pónganlo en un servicio (`Services/SimulacionCarreraService.cs`) para que lo use el controlador y también el seed:
```csharp
public void Terminar(Carrera carrera)   // carrera con .Include(c => c.Resultados)
{
    if (carrera.Estado != EstadosCarrera.Abierta)
        throw new InvalidOperationException("La carrera ya está terminada.");

    foreach (var r in carrera.Resultados)
    {
        var segundosPorKm = Random.Shared.Next(240, 481);   // 481 porque el máximo no se incluye
        r.Tiempo = TimeSpan.FromSeconds(Math.Round(segundosPorKm * (double)carrera.Distancia));
    }

    foreach (var categoria in carrera.Resultados.GroupBy(r => r.IdCategoria))
    {
        var posicion = 1;
        foreach (var r in categoria.OrderBy(r => r.Tiempo))
            r.Ranking = posicion++;
    }

    carrera.Estado = EstadosCarrera.Terminada;
}
```
En el controlador: validar que la organización logueada sea la **creadora**, cargar la carrera con `Include`, llamar a `Terminar`, sumar `carrera.Distancia` a `KmsTotales` de cada inscrito, `SaveChangesAsync()`, `TempData["Exito"]` y redirigir a `Carrera/{id}`. Esa vista, con la carrera Terminada, muestra el ranking en lugar de "Pendiente".
- **Ritmo** para mostrar: `Tiempo.TotalSeconds / Distancia` → formato `m:ss`.
- Si una carrera no tiene inscritos, igual se puede terminar (queda sin ranking).

### 11.3 Formato de tiempos (un solo helper, no lo copien)
```csharp
public static string FormatoTiempo(TimeSpan t) => $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}";   // 0:40:12
public static string FormatoRitmo(TimeSpan t, decimal km)
{
    var s = (int)Math.Round(t.TotalSeconds / (double)km);
    return $"{s / 60}:{s % 60:00}";                                                                       // 4:01
}
```

### 11.4 Kms totales del corredor
`Corredor.KmsTotales` se mantiene actualizado: **sube** al crear una actividad o al terminar una carrera en la que corrió, **baja** al eliminar una actividad, y al editar se ajusta por la diferencia.

### 11.5 Feed y likes
- Feed: actividades de los corredores que están en `Amistad` con el usuario (`IdCorredor == yo`), de la más reciente a la más vieja.
- Like: si ya existe `LikeActividad(actividad, yo)` se borra, si no se crea (toggle). No se puede dar like a una actividad propia (validarlo).

---

## 12. Los 2 reportes

La rúbrica pide: **título, datos de la base, filtros, totales/resúmenes, encabezados claros**. Las vistas ya tienen todo eso. Falta la consulta.

### Reporte 1: Inscripciones y recaudación (`/Reportes/Inscripciones`)
**Para qué sirve:** la organización ve cuántos inscritos y cuánta plata entró por carrera y por categoría en un período.
**Filtros:** `desde`, `hasta` (sobre `Carrera.Fecha`) y `estado` (Abierta / Terminada / todas).
**Totales:** cantidad de carreras, inscritos, recaudación total, promedio por carrera, total por categoría (barras) y fila de totales.

```csharp
var orgId = User.ObtenerId();
var q = _db.Carreras.Where(c => c.Organizadoras.Any(o => o.IdOrganizacion == orgId));
if (desde.HasValue) q = q.Where(c => c.Fecha >= desde.Value);
if (hasta.HasValue) q = q.Where(c => c.Fecha < hasta.Value.AddDays(1));
if (!string.IsNullOrEmpty(estado)) q = q.Where(c => c.Estado == estado);

var filas = await q
    .OrderBy(c => c.Fecha)
    .Select(c => new FilaInscripcionesViewModel
    {
        Carrera = c.Nombre,
        Fecha = c.Fecha,
        Estado = c.Estado,
        Inscritos = c.Resultados.Count,
        Recaudado = c.Resultados.Count * c.PrecioInscripcion,
        PorCategoria = c.Categorias.Select(cat => new ConteoCategoria
        {
            Rango = cat.EdadMinima + "–" + cat.EdadMaxima,
            Inscritos = cat.Resultados.Count
        }).ToList()
    })
    .ToListAsync();
```
- Las **columnas de categoría** de la tabla = los rangos distintos de todas las filas (`filas.SelectMany(f => f.PorCategoria).Select(p => p.Rango).Distinct()`). Si una carrera no tiene ese rango, se muestra `0`.
- Los **totales se calculan en el controlador** (o en el ViewModel), **no** en la vista. Hoy la vista de ejemplo los calcula ahí: al conectarla, muevan esos cálculos al ViewModel.
- Formato de plata: `valor.ToString("N0", new CultureInfo("es-CL"))`.

### Reporte 2: Rendimiento por categoría o equipo (`/Reportes/Rendimiento`)
**Para qué sirve:** comparar el rendimiento de categorías o de equipos en una carrera terminada (quién ganó más podios y qué grupo es más rápido).
**Filtros:** `carrera` (solo las **terminadas** de esa organización, en un `<select>`) y `agrupar` (`categoria` | `equipo`).
**Totales:** finalistas, mejor tiempo (con nombre), tiempo promedio, ritmo promedio y fila de totales.

```csharp
var resultados = await _db.Resultados
    .Where(r => r.IdCarrera == carreraId && r.Tiempo != null)
    .Select(r => new
    {
        r.Tiempo, r.Ranking,
        Corredor = r.Corredor.Nombre,
        Equipo = r.Corredor.Equipo != null ? r.Corredor.Equipo.Nombre : "Sin equipo",
        Categoria = r.Categoria.EdadMinima + "–" + r.Categoria.EdadMaxima
    })
    .ToListAsync();   // se traen a memoria: el promedio de TimeSpan se calcula en C#

var filas = resultados
    .GroupBy(r => agrupar == "equipo" ? r.Equipo : r.Categoria)
    .Select(g => new FilaRendimientoViewModel
    {
        Grupo = g.Key,
        Finalistas = g.Count(),
        Podios = g.Count(r => r.Ranking <= 3),
        MejorTiempo = g.Min(r => r.Tiempo!.Value),
        MejorNombre = g.OrderBy(r => r.Tiempo).First().Corredor,
        TiempoPromedio = TimeSpan.FromSeconds(g.Average(r => r.Tiempo!.Value.TotalSeconds))
    })
    .OrderByDescending(f => f.Podios).ThenBy(f => f.TiempoPromedio)
    .ToList();
```
En el README hay que describir los 2 reportes y para qué le sirven al usuario (copien los "Para qué sirve" de acá).

---

## 13. Validaciones y mensajes

### Con DataAnnotations en los ViewModels (los mensajes en español, claros)
```csharp
[Required(ErrorMessage = "El nombre es obligatorio.")]
[StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
public string Nombre { get; set; } = "";

[Range(0.1, 500, ErrorMessage = "La distancia debe estar entre 0,1 y 500 km.")]
public decimal Distancia { get; set; }

[EmailAddress(ErrorMessage = "Correo no válido.")]
public string Email { get; set; } = "";

[Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
public string ConfirmarContrasena { get; set; } = "";
```

### Reglas que van en el controlador (`ModelState.AddModelError`)
| Formulario | Regla |
|---|---|
| Registro | Email no repetido (en Corredores **ni** en Organizaciones). Corredor: fecha de nacimiento en el pasado y edad ≥ 5. Contraseña ≥ 8 caracteres. |
| Carrera | Fecha futura. Precio ≥ 0. Al menos 1 categoría. `EdadMinima <= EdadMaxima`. Rangos **sin superponerse**. Solo se edita si está Abierta. |
| Actividad | Distancia > 0. Tiempo con formato `h:mm:ss` y > 0. Fecha no futura. |
| Vínculo | Organizadora: fecha de inicio y tamaño ≥ 1. Sponsor: monto > 0. La misma organización no se vincula dos veces con el mismo rol. |
| Equipo | Nombre obligatorio y único. |

### Mensajes de confirmación
Después de **toda** acción exitosa: `TempData["Exito"] = "…";`. Para errores de reglas que no son de un campo (por ejemplo "la carrera ya terminó"): `TempData["Error"] = "…";`. El layout los muestra solos.

### Validación del lado del cliente
Las vistas con formularios ya incluyen `_ValidationScriptsPartial`. Funciona sola cuando usan `asp-for` y `asp-validation-for`.

---

## 14. Cómo usar la IA sin romper el proyecto

Está bien usar IA. **Pero eres responsable de lo que subes y te lo van a preguntar.**

**Antes de pedirle algo:**
1. Pásale **este archivo completo** y `DER_actualizado.md`.
2. Pásale **solo los archivos que va a tocar** (el controlador, la vista, el modelo).
3. Pide **una cosa chica a la vez** ("crea el ViewModel y conecta la vista de Equipos/Index"), no "haz todo el proyecto".

**Copia y pega este prompt al empezar:**
> Estoy trabajando en un proyecto ASP.NET Core MVC (.NET 10) con EF Core. Te paso collab.md con las reglas del proyecto: síguelas al pie de la letra. En especial: no cambies el HTML ni las clases CSS de las vistas, no agregues Bootstrap ni estilos inline, no pongas lógica en las vistas, usa ViewModels, usa las entidades y el DbContext tal como están definidos, y después de un POST usa RedirectToAction con TempData["Exito"]. Solo modifica los archivos que te pido. La tarea es: ______

**Después de que responda:**
- [ ] `dotnet build` compila sin errores.
- [ ] Abriste la pantalla en el navegador y funciona (incluido el caso de error).
- [ ] La vista se ve **igual** que antes (no te cambió clases ni estructura).
- [ ] No agregó paquetes NuGet, archivos raros ni código que no se usa.
- [ ] **Puedes explicar cada línea en voz alta.** Si no, pregúntale a la IA "explícame esta línea" hasta que puedas. En la presentación no vas a tener IA.

**Señales de que la IA se fue por las ramas (rechaza la respuesta):** reescribe la vista entera, agrega Bootstrap, crea un "Repository pattern" con 5 interfaces, cambia nombres de entidades del DER, mete `style=` en el HTML, pone consultas a la base dentro de la `.cshtml`, o toca `site.css` sin que se lo pidas.

---

## 15. Turnos de trabajo, Git y registro de avance

### 15.1 Orden de trabajo
Se trabaja **por turnos, de a uno, en este orden**. Nadie empieza hasta que la persona anterior terminó, subió todo y completó su parte del registro (15.4).

| Turno | Quién | Qué deja listo |
|---|---|---|
| 1 | **Victor** | Base de datos completa: entidades, DbContext, migración, servicios auxiliares y datos de prueba (seed) |
| 2 | **Nico** | Cuentas (registro, login y roles), perfil del corredor, actividades, feed y amigos |
| 3 | **Harold** | Carreras del corredor (catálogo, detalle, inscripción y ranking), equipos, organizaciones, sponsors y perfil de la organización |
| 4 | **Mauricio** | Panel de la organización, crear/editar/eliminar/terminar carrera, los 2 reportes y la entrega final |

### 15.2 Al empezar y al terminar tu turno (todos)
**Al empezar:**
```bash
git pull
dotnet ef database update      # desde el turno 2 en adelante
dotnet run --project ISIRUN
```
1. Lee el **registro de avance (15.4)**: qué dejó hecho la persona anterior y qué te avisa.
2. Comprueba que la app compila y arranca **antes** de tocar nada. Si no arranca, avísale a la persona anterior; no lo "arregles" a ciegas.

**Mientras trabajas:**
- Trabajen directo en `main` (como no trabajan a la vez, no hacen falta ramas).
- **Un commit por cada tarea terminada** de tu lista, con un mensaje descriptivo. Como mínimo **5 commits** por persona (la nota individual de GitHub vale 10 pts).
- `dotnet build` antes de cada commit y `git push` al final de cada día. Si tu PC se rompe, el avance no se pierde.

**Al terminar:**
1. Revisa la "Definición de terminado" de tu turno: **todo** tiene que cumplirse.
2. Actualiza este archivo: marca tus `[x]`, completa tu fila del registro (15.4) y escribe tu nota para la siguiente persona.
3. Commit y push:
   ```bash
   git add .
   git commit -m "Actualiza collab.md: termina turno de <tu nombre>"
   git push
   ```
4. Avisa en el grupo que terminaste.

**Ejemplos de mensajes de commit:**
- ✅ `Agrega entidades Carrera, CategoriaEdad y Resultado según el DER`
- ✅ `Implementa inscripción a carrera con asignación de categoría por edad`
- ❌ `cambios`, `fix`, `asdf`, `subo mi parte`, `update`
- Configura tu nombre real **antes del primer commit**, para que se vea quién hizo qué:
  ```bash
  git config --global user.name "Nombre Apellido"
  git config --global user.email "el-mismo-de-tu-github@correo.com"
  ```
- **Nunca** `git push --force`.

---

### 15.3 Tareas por turno

#### 🟦 Turno 1: Victor · Base de datos
Lee primero las secciones **7** y **11** completas.

- [x] Instalar los paquetes de EF Core y la herramienta `dotnet-ef` (7.1)
- [x] Crear las 11 entidades en `Models/` según `DER_actualizado.md` (7.2), con `[Required]`, `[StringLength]` y la navegación (`ICollection<...>`)
- [x] Crear `EstadosCarrera` con las constantes `Abierta` y `Terminada` (7.2)
- [x] Crear `Data/IsirunContext.cs` **copiando** la configuración de 7.3 (claves compuestas, emails únicos y `NoAction`)
- [x] Agregar la cadena de conexión a LocalDB y registrar el DbContext en `Program.cs` (7.4)
- [x] Crear la migración `Inicial` y comprobar que `dotnet ef database update` crea la base sin errores (7.5)
- [x] Crear la carpeta `Helpers/` con: `FormatoTiempo`, `FormatoRitmo` (11.3), `Edad(nacimiento, dia)` (11.1) e `Iniciales(nombre)` (sección 10, paso 5)
- [x] Crear `Services/SimulacionCarreraService.cs` con el método `Terminar` de 11.2 y registrarlo en `Program.cs` (`builder.Services.AddScoped<SimulacionCarreraService>()`)
- [x] Crear `Data/SeedData.cs` (7.6): 3 equipos, ~15 corredores, 3 organizaciones, 4 carreras (2 abiertas con inscritos y 2 **terminadas usando `SimulacionCarreraService`**), categorías, sponsors, co-organizadoras, actividades, likes y amistades. Las contraseñas se guardan con `PasswordHasher` (sección 8) y **todas** son `Prueba123!`
- [x] Ejecutar el seed al arrancar la app **solo si la base está vacía**
- [x] Crear `README.md` en la raíz con: nombre y descripción del proyecto, requisitos y pasos para crear la base y ejecutar (sección 16). Los demás lo van completando.
- [x] Anotar en el registro (15.4) **un correo de corredor y uno de organización** del seed, para que los demás puedan probar

**Definición de terminado:**
- Una persona que clona el repo y ejecuta `dotnet ef database update` + `dotnet run` obtiene la base creada **con datos**.
- En SQL Server Object Explorer (Visual Studio) se ven las 11 tablas con datos, y las 2 carreras terminadas tienen `Tiempo` y `Ranking` llenos.
- Las vistas siguen mostrando datos de ejemplo (eso está bien: conectarlas es trabajo de los turnos siguientes).

---

#### 🟩 Turno 2: Nico · Cuentas, perfil, actividades, feed y amigos
Lee primero las secciones **8**, **10** y **13**.

**Cuentas**
- [ ] Configurar la autenticación por cookies en `Program.cs`, con `UseAuthentication` antes de `UseAuthorization` (sección 8)
- [ ] Crear la extensión `User.ObtenerId()` (sección 8) y usarla en todos lados
- [ ] `LoginViewModel` + login real (busca en Corredores y luego en Organizaciones; el mismo mensaje de error para ambos casos)
- [ ] `RegistroViewModel` + registro de los 2 tipos, con validaciones (13): email único en **ambas** tablas, contraseñas que coinciden, edad ≥ 5. Inicia sesión automáticamente y redirige según el tipo
- [ ] `Salir` real (`SignOutAsync`)
- [ ] Poner `[Authorize(Roles = ...)]` en **todos** los controladores, según la sección 8
- [ ] `_Layout`: elegir la barra según el rol cuando la vista no define `Menu`. `_NavCorredor` y `_NavOrganizacion`: mostrar las iniciales reales. Login: el selector de equipos del registro sale de la base

**Corredor**
- [ ] `Corredor/Perfil` con `PerfilViewModel` (stats, equipo, inscripciones pendientes, resultados y últimas actividades)
- [ ] `Corredor/Ver/{id}`: el perfil público de otro corredor
- [ ] Actividades: crear, editar y eliminar, con `ActividadViewModel`, sus validaciones y el ajuste de `KmsTotales` (11.4). Solo el dueño puede editar o eliminar
- [ ] `Feed` + like/unlike (11.5)
- [ ] `Amigos`: búsqueda, agregar y quitar (no a uno mismo ni duplicado)

**Definición de terminado:**
- Puedes registrar un corredor nuevo y una organización nueva, cerrar sesión y volver a entrar con los dos.
- Como corredor, **no** puedes abrir `/Organizacion/Panel` (te manda al login), y viceversa.
- Crear, editar y eliminar una actividad cambia los km de "Mi perfil".
- Ninguna de **tus** vistas tiene ya un bloque `DATOS DE EJEMPLO`.
- Los formularios con datos inválidos muestran el mensaje en rojo debajo del campo.

---

#### 🟨 Turno 3: Harold · Carreras, equipos, organizaciones y sponsors
Lee primero las secciones **9**, **10** y **11.1**.

**Carreras (lado corredor)**
- [ ] `Carreras/Index`: listado real con la pestaña Abiertas/Terminadas, filtro por distancia y búsqueda `q` por nombre de carrera u organización
- [ ] `Carreras/Detalle`: datos reales, categorías, organizadoras y sponsors. Si la carrera está Terminada, redirige a `Resultados`
- [ ] `Inscribirse`: implementarlo **exactamente** como en 11.1. `CancelarInscripcion`: solo si la carrera está Abierta. La vista muestra "Inscrito · pendiente" o el botón según corresponda
- [ ] `Carreras/Resultados`: ranking real por categoría, podio y "Tu resultado" (solo si el usuario corrió). Si la carrera está Abierta, redirige a `Detalle`
- [ ] `Home/Index`: las 3 próximas carreras abiertas

**Equipos y organizaciones**
- [ ] `Equipos/Index`, `Equipos/Detalle`, crear (nombre único), unirse y salir
- [ ] `Organizaciones/Index`: lista y detalle con lo que organiza y patrocina

**Lado organización (lo simple)**
- [ ] `Organizacion/Perfil`: editar nombre y actividad
- [ ] `Organizacion/Vinculos`: lista real, agregar co-organizadora o sponsor (validaciones de 13) y quitar (la creadora no se puede quitar)

**Definición de terminado:**
- Con un corredor del seed: te inscribes en una carrera abierta, el perfil la muestra como pendiente, cancelas y desaparece.
- Una carrera terminada del seed muestra el ranking real, y en ella **no** aparece el botón de inscribirse.
- Un corredor sin categoría para su edad ve el mensaje de error y **no** queda inscrito.
- Ninguna de **tus** vistas tiene ya un bloque `DATOS DE EJEMPLO`.

---

#### 🟥 Turno 4: Mauricio · Panel de la organización, reportes y entrega
Lee primero las secciones **11.2**, **12**, **13** y **16**.

**Gestión de carreras**
- [ ] `Organizacion/Panel`: carreras propias abiertas y terminadas, con número de inscritos
- [ ] `Organizacion/Carrera/{id}`: inscritos por categoría y, si está terminada, el ranking con tiempo y ritmo
- [ ] Crear y editar carrera con `CarreraFormViewModel` + lista de categorías: agregar y quitar filas, y todas las validaciones de 13 (rangos sin superponerse, fecha futura, etc.). Al crear, se guarda `OrgCarrera` con `EsCreadora = true`
- [ ] `EliminarCarrera`: solo la creadora y solo si está Abierta (cascada)
- [ ] `TerminarCarrera`: usar `SimulacionCarreraService` (hecho por Victor), sumar los km a cada inscrito y mostrar el mensaje

**Reportes (15 pts)**
- [ ] Reporte 1, Inscripciones y recaudación, con consulta real, los 3 filtros y los totales **calculados en el controlador o ViewModel, no en la vista** (12)
- [ ] Reporte 2, Rendimiento, con consulta real y los filtros por carrera terminada y agrupación (12)

**Entrega final**
- [ ] Probar el recorrido completo de la presentación (registro → inscripción → terminar carrera → ranking → reportes) con los datos del seed
- [ ] Buscar en todo el proyecto `DATOS DE EJEMPLO` y `TODO`: **no debe quedar ninguno**. Si queda alguno de otro turno, avísale al dueño o resuélvelo
- [ ] Borrar código sin uso y verificar que compile **sin warnings**
- [ ] Completar el README: integrantes con su usuario de GitHub, descripción de los 2 reportes y usuarios de prueba
- [ ] Exportar el DER actualizado a imagen (mermaid.live) y juntarlo con el DER original y la tabla de cambios
- [ ] **Último commit: borrar `collab.md`** (ver el recuadro al final de este archivo)
- [ ] Armar el ZIP **sin** `bin/`, `obj/`, `.vs/` ni `collab.md`, desde el último commit

**Definición de terminado:** toda la checklist de la sección 16 está marcada.

---

### 15.4 Registro de avance
Cada uno completa **su fila** al terminar su turno. Si algo quedó a medias, se escribe acá, no se esconde.

| Turno | Quién | Estado | Fecha | Último commit | Nota para el siguiente |
|---|---|---|---|---|---|
| 0 | Mateo | ✅ Terminado | 2026-10-26 | Crea proyecto MVC con vistas, navegación y guía del equipo | Están listas todas las vistas, controladores con `TODO`, CSS, DER actualizado y esta guía. Victor: empieza por la sección 7. |
| 1 | Victor | ✅ Implementación terminada; falta commit/push manual | 2026-10-26 | Pendiente: Victor hará los commits manualmente | Base, migración Inicial, helpers, simulación, seed y README listos y verificados. Tras recibir los cambios publicados por Victor, Nico ejecuta dotnet ef database update --project ISIRUN y dotnet run --project ISIRUN. Los helpers están en IsirunHelpers; Terminar no suma km ni guarda: lo hace el llamador (ver SeedData). Las vistas conservan ejemplos. |
| 2 | Nico | ⏳ Pendiente | | | |
| 3 | Harold | ⏳ Pendiente | | | |
| 4 | Mauricio | ⏳ Pendiente | | | |

**Usuarios de prueba del seed** (creados por Victor; el login real se implementa en el turno de Nico):
- Corredor: `ana.rojas@isirun.test` / `Prueba123!`
- Organización: `club.norte@isirun.test` / `Prueba123!`

**Problemas conocidos / cosas a medias** (cualquiera agrega acá):
- _(nada por ahora)_

---

## 16. Checklist de entrega

**README.md** (en la raíz del repo, lo pide la rúbrica):
- [ ] Nombre y descripción del proyecto
- [ ] Integrantes y su usuario de GitHub
- [ ] Requisitos (.NET 10 SDK, SQL Server LocalDB) y pasos para ejecutar
- [ ] Cómo crear la base: `dotnet ef database update`
- [ ] Descripción de los **2 reportes** y su utilidad (sección 12)
- [ ] Un usuario corredor y uno de organización **de prueba** (del seed) para que el profe pueda entrar

**DER:**
- [ ] DER original (la imagen del proyecto anterior)
- [ ] DER actualizado: exportar el diagrama de `DER_actualizado.md` a imagen (pegarlo en https://mermaid.live → descargar PNG)
- [ ] Explicación de los cambios: la tabla de `DER_actualizado.md`
- [ ] **Revisar que el DER coincida con las entidades finales.** Si cambian algo del modelo, actualicen el DER.

**Código:**
- [ ] Compila sin errores ni advertencias
- [ ] No queda ningún `DATOS DE EJEMPLO` ni ningún `TODO` (búsquenlos)
- [ ] No hay código comentado viejo ni archivos sin uso
- [ ] No hay contraseñas, API keys ni cadenas de conexión con contraseña
- [ ] La base arranca con datos del seed

**ZIP:**
- [ ] Sin las carpetas `bin/`, `obj/` ni `.vs/` (en ningún proyecto)
- [ ] Es la **misma versión** que el último commit de `main`
- [ ] **Sin `collab.md`** ← ver abajo

**Presentación:**
- [ ] Todos saben demostrar: registro de los dos tipos, inscripción, terminar carrera, ranking y los 2 reportes
- [ ] Cada uno puede explicar: qué es un ViewModel, cómo se relacionan Carrera, CategoriaEdad y Resultado, por qué la clave de Resultado es compuesta, cómo funciona la simulación y qué hace `[ValidateAntiForgeryToken]`

---

> ## ⚠️ RECORDATORIO FINAL: BORRAR ESTE ARCHIVO
> Último paso antes de entregar:
> ```bash
> git rm collab.md
> git commit -m "Elimina guía interna collab.md"
> git push
> ```
> Y verifiquen que **no** esté dentro del ZIP.
