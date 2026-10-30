using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IEstadisticasRepository
    {
        Task<int> ContarProductosActivos();
        Task<int> ContarStockBajo(int umbral);
        Task<int> ContarSinStock();
        Task<IReadOnlyList<Producto>> ObtenerAReponer(int umbral, int max);
        Task<decimal> ValorInventario();
        Task<decimal> GananciaEstimada();
        Task<int> ContarUsuarios(bool activos);
        Task<int> ContarAdministradoresActivos();
    }
}
