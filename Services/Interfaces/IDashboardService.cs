using DulcesDuendesApp.Models.ViewModels;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> Obtener(bool esAdministrador);
    }
}
