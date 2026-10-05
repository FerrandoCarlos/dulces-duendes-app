using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace DulcesDuendesApp.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repositorio;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public UsuarioService(IUsuarioRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public Usuario? ValidarCredenciales(string email, string passwordPlano)
        {
            var usuario = _repositorio.ObtenerPorEmail(email);
            if (usuario == null) return null;

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordPlano);
            if (resultado == PasswordVerificationResult.Failed) return null;

            return usuario;
        }

    }
}
