using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Data.Models;

namespace StoreBooksBlazorWASM.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<DetalleVentas> DetalleVentas { get; set; }
    }

    
}
