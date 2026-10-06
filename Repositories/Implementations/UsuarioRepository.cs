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

        public Usuario? ObtenerPorEmail(string email)
        {
            return _context.Usuarios
             .Include(u => u.Rol)
             .FirstOrDefault(u => u.Email == email && u.Activo);
        }

        public int Alta(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return usuario.Id;
        }

        public int ObtenerCantidad()
        {
            return _context.Usuarios.Count();
        }
    }
}
