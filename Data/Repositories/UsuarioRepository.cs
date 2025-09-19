using APITemplate.Data.Interefaces;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace APITemplate.Data.Repositories
{
    public class UsuarioRepository : BaseRepository<USUARIOS>, IUsuarioRepository
    {
        /// <summary>
        /// Constructor: Hereda de BaseRepository y le pasa el contexto
        /// </summary>
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Busca un usuario por su email.
        /// Útil para autenticación y validaciones de unicidad
        /// </summary>
        public async Task<USUARIOS?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Busca un usuario por su nombre de usuario.
        /// Útil para login y validaciones de unicidad
        /// </summary>
        public async Task<USUARIOS?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Verifica si ya existe un email en la base de datos.
        /// Más eficiente que GetByEmailAsync cuando solo necesitas validar existencia
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verifica si ya existe un username en la base de datos.
        /// Útil para validaciones antes de crear o actualizar usuarios
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Obtiene solo los usuarios activos (IsActive = true).
        /// Útil para listados administrativos y reportes
        /// </summary>
        public async Task<IEnumerable<USUARIOS>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.IsActive).ToListAsync();
        }

    }
}
