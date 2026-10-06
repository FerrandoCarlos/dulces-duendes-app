using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmail(string email);
        Task<int> ObtenerCantidad();
        Task<int> Alta(Usuario usuario);
    }
}
