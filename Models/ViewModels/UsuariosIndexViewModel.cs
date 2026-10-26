using DulcesDuendesApp.Common.Pagination;

namespace DulcesDuendesApp.Models.ViewModels
{
    public record UsuarioFilaViewModel(int Id, string NombreCompleto, string Email, string Rol, bool Activo, bool EsActual);

    public class UsuariosIndexViewModel
    {
        public string? Texto { get; set; }
        public PagedResult<UsuarioFilaViewModel> Resultado { get; set; } = new();
    }
}
