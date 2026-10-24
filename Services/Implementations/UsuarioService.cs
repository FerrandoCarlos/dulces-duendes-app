using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace DulcesDuendesApp.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repositorio;
        private readonly IAlmacenArchivos _almacen;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public UsuarioService(IUsuarioRepository repositorio, IAlmacenArchivos almacen)
        {
            _repositorio = repositorio;
            _almacen = almacen;
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

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            return await _repositorio.ObtenerPorId(id);
        }
        public async Task<Usuario> ActualizarPerfil(int id, string nombre, string apellido, string email)
        {
            nombre = nombre?.Trim() ?? "";
            apellido = apellido?.Trim() ?? "";
            email = email?.Trim() ?? "";

            if (nombre.Length == 0 || apellido.Length == 0 || email.Length == 0) throw new AppException("Nombre, apellido y email son Obligatorios.");

            var usuario = await ObtenerExistente(id);

            if (await _repositorio.ExisteEmail(email, id)) throw new AppException("Ya existe un usuario con ese email.");

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Email = email;

            await _repositorio.Actualizar(usuario);
            return usuario;
        }

        public async Task<Usuario> CambiarAvatar(int id, Stream imagen)
        {
            var usuario = await ObtenerExistente(id);
            var urlVieja = usuario.AvatarUrl;

            usuario.AvatarUrl = await _almacen.GuardarImagen(imagen, "avatares");
            await _repositorio.Actualizar(usuario);

            _almacen.Eliminar(urlVieja);
            return usuario;
        }

        public async Task<Usuario> QuitarAvatar(int id)
        {
            var usuario = await ObtenerExistente(id);
            var urlVieja = usuario.AvatarUrl;
            if (string.IsNullOrEmpty(urlVieja)) return usuario;

            usuario.AvatarUrl = null;
            await _repositorio.Actualizar(usuario);

            _almacen.Eliminar(urlVieja);

            return usuario;
        }
        private async Task<Usuario> ObtenerExistente(int id)
        {
            return await _repositorio.ObtenerPorId(id) ?? throw new AppException("El usuario no existe.");
        }
    }
}
