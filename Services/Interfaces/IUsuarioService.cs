using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> ValidarCredenciales(string email, string passwordPlano);
        Task<int> ObtenerCantidad();
        Task<int> Alta(Usuario usuario, string passwordPlano);
    }
}
