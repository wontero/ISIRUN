using Microsoft.AspNetCore.Mvc;

namespace ISIRUN.Controllers;

// Login, registro y cierre de sesión (corredores y organizaciones).
public class CuentaController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string contrasena)
    {
        // TODO: buscar el email en Corredor y en Organizacion, comparar el hash de la contraseña,
        // iniciar sesión con cookie (claims: id, nombre, rol "Corredor" u "Organizacion").
        // Si falla: ModelState.AddModelError("", "Correo o contraseña incorrectos.") y return View();
        // Si es organización: return RedirectToAction("Panel", "Organizacion");
        return RedirectToAction("Perfil", "Corredor");
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(string tipo)
    {
        // TODO: reemplazar los parámetros por un RegistroViewModel con validaciones,
        // verificar que el email no exista, guardar el hash de la contraseña e iniciar sesión.
        TempData["Exito"] = "Cuenta creada. ¡Bienvenido a ISIRUN!";
        return tipo == "Organizacion"
            ? RedirectToAction("Panel", "Organizacion")
            : RedirectToAction("Perfil", "Corredor");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Salir()
    {
        // TODO: HttpContext.SignOutAsync(...)
        return RedirectToAction("Index", "Home");
    }
}
