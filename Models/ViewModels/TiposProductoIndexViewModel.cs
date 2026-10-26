using DulcesDuendesApp.Common.Pagination;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class TiposProductoIndexViewModel
    {
        public string? Texto { get; init; }
        public PagedResult<TipoProducto> Resultado { get; init; } = new();
    }
}
