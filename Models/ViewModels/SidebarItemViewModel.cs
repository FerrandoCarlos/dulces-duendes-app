namespace DulcesDuendesApp.Models.ViewModels
{
    public record SidebarItemViewModel(
        string Texto,
        string Icono,
        string Controller,
        string Action,
        string? Rol = null,
        bool Activo = false
    );
}
