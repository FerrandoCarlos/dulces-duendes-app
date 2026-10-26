using DulcesDuendesApp.Common.Pagination;
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
        Task CambiarPassword(int id, string passwordActual, string passwordNueva);
        Task<PagedResult<Usuario>> ObtenerPaginado(string? texto, PageRequest page);
        Task<Usuario?> ObtenerParaAdmin(int id);
        Task<IReadOnlyList<Rol>> ObtenerRoles();
        Task Modificar(int id, string nombre, string apellido, string email, int rolId, int idActor);
        Task CambiarEstado(int id, bool Activo, int idActor);
    }
}
