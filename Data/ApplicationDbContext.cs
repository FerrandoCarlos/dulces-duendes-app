using DulcesDuendesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany()
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
