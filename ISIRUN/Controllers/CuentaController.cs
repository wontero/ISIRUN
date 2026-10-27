using System.Security.Claims;
using ISIRUN.Data;
using ISIRUN.Helpers;
using ISIRUN.Models;
using ISIRUN.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Controllers;

// Login, registro y cierre de sesión (corredores y organizaciones).
public class CuentaController(IsirunContext db) : Controller
{
    private const int EdadMinimaCorredor = 5;

    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var email = NormalizarEmail(vm.Email);

        // 1) Se busca primero entre los corredores...
        var corredor = await db.Corredores.FirstOrDefaultAsync(c => c.Email == email);
        if (corredor != null && ContrasenaCorrecta(corredor, corredor.ContrasenaHash, vm.Contrasena))
        {
            await IniciarSesionAsync(corredor.IdCorredor, corredor.Nombre, Roles.Corredor);
            return RedirectToAction("Perfil", "Corredor");
        }

        // 2) ...y después entre las organizaciones.
        var organizacion = await db.Organizaciones.FirstOrDefaultAsync(o => o.Email == email);
        if (organizacion != null && ContrasenaCorrecta(organizacion, organizacion.ContrasenaHash, vm.Contrasena))
        {
            await IniciarSesionAsync(organizacion.IdOrganizacion, organizacion.Nombre, Roles.Organizacion);
            return RedirectToAction("Panel", "Organizacion");
        }

        // Mismo mensaje si no existe el correo o si la contraseña está mal:
        // así no se le dice a un extraño qué correos están registrados.
        ModelState.AddModelError("", "Correo o contraseña incorrectos.");
        return View(vm);
    }

    // /Cuenta/Registro?tipo=Organizacion abre con "Organización" marcado (link del Inicio).
    public async Task<IActionResult> Registro(string? tipo = null)
    {
        var vm = new RegistroViewModel
        {
            Tipo = tipo == Roles.Organizacion ? Roles.Organizacion : Roles.Corredor
        };
        await CargarEquiposAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel vm)
    {
        ValidarCamposDelTipo(vm);

        var email = NormalizarEmail(vm.Email);
        if (await EmailRegistradoAsync(email))
            ModelState.AddModelError(nameof(vm.Email), "Ya existe una cuenta con ese correo.");

        if (vm.IdEquipo.HasValue && !await db.Equipos.AnyAsync(e => e.IdEquipo == vm.IdEquipo))
            ModelState.AddModelError(nameof(vm.IdEquipo), "El equipo elegido no existe.");

        if (!ModelState.IsValid)
        {
            await CargarEquiposAsync(vm);
            return View(vm);
        }

        if (vm.Tipo == Roles.Corredor)
        {
            var corredor = new Corredor
            {
                Nombre = vm.Nombre!.Trim(),
                Email = email,
                FechaNac = vm.FechaNac!.Value.Date,
                FechaRegistro = DateTime.Now,
                IdEquipo = vm.IdEquipo
            };
            corredor.ContrasenaHash = new PasswordHasher<Corredor>().HashPassword(corredor, vm.Contrasena);
            db.Corredores.Add(corredor);
            await db.SaveChangesAsync();

            await IniciarSesionAsync(corredor.IdCorredor, corredor.Nombre, Roles.Corredor);
            TempData["Exito"] = "Cuenta creada. ¡Bienvenido a ISIRUN!";
            return RedirectToAction("Perfil", "Corredor");
        }

        var organizacion = new Organizacion
        {
            Nombre = vm.NombreOrganizacion!.Trim(),
            Actividad = vm.Actividad!.Trim(),
            Email = email
        };
        organizacion.ContrasenaHash = new PasswordHasher<Organizacion>().HashPassword(organizacion, vm.Contrasena);
        db.Organizaciones.Add(organizacion);
        await db.SaveChangesAsync();

        await IniciarSesionAsync(organizacion.IdOrganizacion, organizacion.Nombre, Roles.Organizacion);
        TempData["Exito"] = "Cuenta creada. ¡Bienvenido a ISIRUN!";
        return RedirectToAction("Panel", "Organizacion");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // Guarda en la cookie quién es (id), cómo se llama y su rol.
    private async Task IniciarSesionAsync(int id, string nombre, string rol)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, nombre),
            new(ClaimTypes.Role, rol)
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
    }

    // Compara la contraseña escrita con el hash guardado (nunca se guarda en texto plano).
    private static bool ContrasenaCorrecta<T>(T usuario, string hash, string contrasena) where T : class
    {
        return new PasswordHasher<T>().VerifyHashedPassword(usuario, hash, contrasena)
            != PasswordVerificationResult.Failed;
    }

    // El correo no puede estar en ninguna de las dos tablas.
    private async Task<bool> EmailRegistradoAsync(string email)
    {
        return await db.Corredores.AnyAsync(c => c.Email == email)
            || await db.Organizaciones.AnyAsync(o => o.Email == email);
    }

    // Obligatorios según el tipo de cuenta elegido (los del otro tipo no se envían).
    private void ValidarCamposDelTipo(RegistroViewModel vm)
    {
        if (vm.Tipo == Roles.Corredor)
        {
            if (string.IsNullOrWhiteSpace(vm.Nombre))
                ModelState.AddModelError(nameof(vm.Nombre), "El nombre es obligatorio.");

            if (vm.FechaNac == null)
                ModelState.AddModelError(nameof(vm.FechaNac), "La fecha de nacimiento es obligatoria.");
            else if (vm.FechaNac.Value.Date >= DateTime.Today)
                ModelState.AddModelError(nameof(vm.FechaNac), "La fecha de nacimiento tiene que ser en el pasado.");
            else if (IsirunHelpers.Edad(vm.FechaNac.Value, DateTime.Today) < EdadMinimaCorredor)
                ModelState.AddModelError(nameof(vm.FechaNac), $"Tienes que tener al menos {EdadMinimaCorredor} años.");
        }
        else if (vm.Tipo == Roles.Organizacion)
        {
            if (string.IsNullOrWhiteSpace(vm.NombreOrganizacion))
                ModelState.AddModelError(nameof(vm.NombreOrganizacion), "El nombre de la organización es obligatorio.");
            if (string.IsNullOrWhiteSpace(vm.Actividad))
                ModelState.AddModelError(nameof(vm.Actividad), "La actividad es obligatoria.");
        }
    }

    private async Task CargarEquiposAsync(RegistroViewModel vm)
    {
        vm.Equipos = await db.Equipos
            .OrderBy(e => e.Nombre)
            .Select(e => new SelectListItem(e.Nombre, e.IdEquipo.ToString()))
            .ToListAsync();
    }

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
