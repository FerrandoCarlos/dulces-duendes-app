using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Usuario? ObtenerPorEmail(string email);
        int ObtenerCantidad();
        int Alta(Usuario usuario);
    }
}
