using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> ValidarCredenciales(string email, string passwordPlano);
        Task<int> ObtenerCantidad();
        Task<int> Alta(Usuario usuario, string passwordPlano);
        Task<Usuario?> ObtenerPorId(int id);
        Task<Usuario> ActualizarPerfil(int id, string nombre, string apellido, string email);
        Task<Usuario> CambiarAvatar(int id, Stream imagen);
        Task<Usuario> QuitarAvatar(int id);
    }
}
