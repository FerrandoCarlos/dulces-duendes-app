using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DulcesDuendesApp.Common.Configuration
{
    public static class AuthEventos
    {
        private static bool EsRutaJson(HttpRequest request) => request.Path.StartsWithSegments("/ajax") ||
            request.Path.StartsWithSegments("/api");

        public static Task AlRedirigirALogin(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            if (EsRutaJson(ctx.Request))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        }

        public static Task AlRedirigirAAccesoDenegado(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            if (EsRutaJson(ctx.Request))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        }
    }
}
