using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.Dtos;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface ITokenService
    {
        TokenResponse Generar(Usuario usuario);
    }
}
