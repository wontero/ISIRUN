namespace ISIRUN.Helpers;

// Nombres de los dos roles. Se usan en el login y en [Authorize(Roles = ...)],
// así nunca se escribe "Corredor" u "Organizacion" suelto en varios lados.
public static class Roles
{
    public const string Corredor = "Corredor";
    public const string Organizacion = "Organizacion";
}
