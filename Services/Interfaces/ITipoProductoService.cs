using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface ITipoProductoService
    {
        Task<IReadOnlyList<TipoProducto>> Buscar(string? texto);
    }
}
