using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IUsuarioService
    {
        Usuario? ValidarCredenciales(string email, string passwordPlano);
        int ObtenerCantidad();
        int Alta(Usuario usuario, string passwordPlano);
    }
}
