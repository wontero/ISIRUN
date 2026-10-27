using System.Security.Claims;

namespace ISIRUN.Helpers;

public static class UsuarioExtensions
{
    // Id del usuario logueado (IdCorredor o IdOrganizacion, según su rol).
    // Se guarda en la cookie como el claim NameIdentifier al iniciar sesión.
    public static int ObtenerId(this ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("No hay un usuario logueado.");
        return int.Parse(id);
    }
}
