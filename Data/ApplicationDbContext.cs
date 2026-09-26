using DulcesDuendesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Rol> Roles { get; set; }

}
