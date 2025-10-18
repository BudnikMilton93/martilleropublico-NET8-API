using Microsoft.EntityFrameworkCore;
using APITemplate.Models;

namespace APITemplate.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {

        }

        public DbSet<USUARIOS> Usuarios { get; set; }
        public DbSet<PROPIEDADES> Propiedades { get; set; }
        public DbSet<FOTOS_PROPIEDAD> FotosPropiedad {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PROPIEDADES>()
                .HasMany(p => p.Fotos)
                .WithOne(f => f.Propiedad)
                .HasForeignKey(f => f.Id_propiedad)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
