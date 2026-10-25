using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Validation;
using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Common.Pagination;
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
            usuario.Email = usuario.Email?.Trim() ?? "";
            usuario.Nombre = usuario.Nombre?.Trim() ?? "";
            usuario.Apellido = usuario.Apellido?.Trim() ?? "";

            if (usuario.Nombre.Length == 0 || usuario.Apellido.Length == 0) throw new AppException("Nombre y apellido son obligatorios.");

            ValidarPassword(passwordPlano);
            await ValidarEmailYRol(usuario.Email, usuario.RolId, 0);

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
        public Task<PagedResult<Usuario>> ObtenerPaginado(string? texto, PageRequest page) => _repositorio.ObtenerPaginado(texto, page);
        public Task<Usuario?> ObtenerParaAdmin(int id) => _repositorio.ObtenerPorId(id, incluirInactivos: true);
        public Task<IReadOnlyList<Rol>> ObtenerRoles() => _repositorio.ObtenerRoles();
        public async Task Modificar(int id, string nombre, string apellido, string email, int rolId, int idActor)
        {
            nombre = nombre?.Trim() ?? "";
            apellido = apellido?.Trim() ?? "";
            email = email?.Trim() ?? "";

            if (nombre.Length == 0 || apellido.Length == 0 || email.Length == 0) throw new AppException("Nombre, apellido y email son obligatorios.");

            var usuario = await ObtenerParaAdmin(id) ?? throw new AppException("El usuario no existe.");
            await ValidarEmailYRol(email, rolId, id);

            if (rolId != usuario.RolId)
            {
                if (id == idActor) throw new AppException("No podes cambiar tu propio rol.");
                await ProtegerUltimoAdministrador(usuario);
            }

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Email = email;
            usuario.RolId = rolId;

            await _repositorio.Actualizar(usuario);
        }
        public async Task CambiarEstado(int id, bool activo, int idActor)
        {
            if (id == idActor && !activo) throw new AppException("No podes desactivar tu propia cuenta.");
            var usuario = await ObtenerParaAdmin(id) ?? throw new AppException("El usuario no existe.");
            if (usuario.Activo == activo) return;

            if (!activo) await ProtegerUltimoAdministrador(usuario);

            usuario.Activo = activo;
            await _repositorio.Actualizar(usuario);
        }

        public async Task CambiarPassword(int id, string passwordActual, string passwordNueva)
        {
            if (string.IsNullOrEmpty(passwordActual) || string.IsNullOrEmpty(passwordNueva))
                throw new AppException("Completá la contraseña actual y la nueva.");

            if (passwordNueva.Length < LimitesUsuario.PasswordMinima || passwordNueva.Length > LimitesUsuario.PasswordMaxima)
                throw new AppException($"La nueva contraseña debe tener entre {LimitesUsuario.PasswordMinima} y {LimitesUsuario.PasswordMaxima} caracteres.");

            var usuario = await ObtenerExistente(id);

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordActual);
            if (resultado == PasswordVerificationResult.Failed)
                throw new AppException("La contraseña actual es incorrecta.");

            if (passwordActual == passwordNueva)
                throw new AppException("La nueva contraseña debe ser distinta de la actual.");

            usuario.PasswordHash = _hasher.HashPassword(usuario, passwordNueva);
            await _repositorio.Actualizar(usuario);
        }

        private async Task<Usuario> ObtenerExistente(int id)
        {
            return await _repositorio.ObtenerPorId(id) ?? throw new AppException("El usuario no existe.");
        }

        private static void ValidarPassword(string? password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < LimitesUsuario.PasswordMinima || password.Length > LimitesUsuario.PasswordMaxima)
                throw new AppException($"La contraseña debe tener entre {LimitesUsuario.PasswordMinima} y {LimitesUsuario.PasswordMaxima} caracteres.");
        }
        private async Task ValidarEmailYRol(string email, int rolId, int idExcluido)
        {
            if (await _repositorio.ExisteEmail(email, idExcluido)) throw new AppException("Ya existe un usuario con este email.");

            var roles = await _repositorio.ObtenerRoles();
            if (!roles.Any(r => r.Id == rolId)) throw new AppException("El rol seleccionado no existe.");
        }
        private async Task ProtegerUltimoAdministrador(Usuario usuario)
        {
            var esAdministrador = usuario.Activo && usuario.Rol?.Nombre == NombresRol.Administrador;
            if (esAdministrador && await _repositorio.ContarAdministradoresActivos() <= 1) throw new AppException("Debe quedar al menos un administrador activo.");
        }
    }
}
