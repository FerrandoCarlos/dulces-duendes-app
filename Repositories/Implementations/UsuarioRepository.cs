using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorEmail(string email)
        {
            return await _context.Usuarios
             .Include(u => u.Rol)
             .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
        }

        public async Task<int> Alta(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario.Id;
        }

        public async Task<int> ObtenerCantidad()
        {
            return await _context.Usuarios.CountAsync();
        }
    }
}
