using DulcesDuendesApp.Common.Configuration;
using DulcesDuendesApp.Models.ViewModels;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DulcesDuendesApp.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private const int MaxAReponer = 5;
        private readonly IEstadisticasRepository _repo;
        private readonly int _umbral;

        public DashboardService(IEstadisticasRepository repo, IOptionsSnapshot<NegocioOptions> negocio)
        {
            _repo = repo;
            _umbral = negocio.Value.StockBajo;
        }
        public async Task<DashboardViewModel> Obtener(bool esAdministrador)
        {
            var aReponer = await _repo.ObtenerAReponer(_umbral, MaxAReponer);
            return new DashboardViewModel
            {
                ProductosActivos = await _repo.ContarProductosActivos(),
                StockBajo = await _repo.ContarStockBajo(_umbral),
                SinStock = await _repo.ContarSinStock(),
                AReponer = aReponer.Select(p => new ProductoAReponerViewModel(p.Id, p.Descripcion, p.Stock))
                                    .ToList(),
                Admin = esAdministrador ? await ObtenerAdmin() : null
            };
        }
        private async Task<DashboardAdminViewModel> ObtenerAdmin()
        {
            return new DashboardAdminViewModel(
                await _repo.ContarUsuarios(true),
                await _repo.ContarUsuarios(false),
                await _repo.ContarAdministradoresActivos(),
                await _repo.ValorInventario(),
                await _repo.GananciaEstimada()
            );
        }

    }
}
