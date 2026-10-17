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

        public async Task<Usuario?> ValidarCredenciales(string email, string passwordPlano)
        {
            var usuario = await _repositorio.ObtenerPorEmail(email);
            if (usuario == null) return null;

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordPlano);
            if (resultado == PasswordVerificationResult.Failed) return null;

            return usuario;
        }

        public async Task<int> ObtenerCantidad()
        {
            return await _repositorio.ObtenerCantidad();
        }

        public async Task<int> Alta(Usuario usuario, string passwordPlano)
        {
            usuario.PasswordHash = _hasher.HashPassword(usuario, passwordPlano);
            return await _repositorio.Alta(usuario);
        }


    }
}
