using DulcesDuendesApp.Models;
using DulcesDuendesApp.Common.Pagination;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmail(string email);
        Task<int> ObtenerCantidad();
        Task<int> Alta(Usuario usuario);
        Task<Usuario?> ObtenerPorId(int id, bool incluirInactivos = false);
        Task<bool> ExisteEmail(string email, int idExcluido);
        Task Actualizar(Usuario usuario);
        Task<PagedResult<Usuario>> ObtenerPaginado(string? texto, PageRequest page);
        Task<IReadOnlyList<Rol>> ObtenerRoles();
        Task<int> ContarAdministradoresActivos();
    }
}
