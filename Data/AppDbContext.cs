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
    }
}
