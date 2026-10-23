namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IAlmacenArchivos
    {
        Task<string> GuardarImagen(Stream contenido, string carpeta);
        void Eliminar(string? urlRelativa);
    }
}
