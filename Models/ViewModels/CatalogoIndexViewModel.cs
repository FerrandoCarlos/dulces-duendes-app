using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models.Dtos;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class CatalogoIndexViewModel
    {
        public string? Texto { get; set; }
        public PagedResult<ProductoResponse> Resultado { get; set; } = new();
    }
}
