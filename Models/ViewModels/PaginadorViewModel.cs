namespace DulcesDuendesApp.Models.ViewModels
{
    public record PaginadorViewModel(int Pagina, int TotalPaginas, Func<int, string?> UrlPagina);
}
