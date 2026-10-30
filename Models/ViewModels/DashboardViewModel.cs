namespace DulcesDuendesApp.Models.ViewModels
{
    public record ProductoAReponerViewModel(int Id, string Descripcion, int Stock);
    public record DashboardAdminViewModel(int UsuariosActivos, int UsuariosInactivos, int Administradores, decimal ValorInventario, decimal GananciaEstimada);
    public record DashboardViewModel
    {
        public int ProductosActivos { get; init; }
        public int StockBajo { get; init; }
        public int SinStock { get; init; }
        public IReadOnlyList<ProductoAReponerViewModel> AReponer { get; init; } = [];
        public DashboardAdminViewModel? Admin { get; init; }
    }

}
