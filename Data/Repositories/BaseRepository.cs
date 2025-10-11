using APITemplate.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using APITemplate.Data.Interfaces;

namespace APITemplate.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor: Inicializa el contexto y el DbSet genérico para la entidad T
        /// </summary>
        public BaseRepository(AppDbContext context)
        {
            _context = context; // Contexto de base de datos
            _dbSet = context.Set<T>(); // DbSet genérico que representa la tabla de la entidad T
        }

        /// <summary>
        /// Obtiene una entidad por su ID (clave primaria).
        /// Retorna null si no encuentra la entidad
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id); // FindAsync es optimizado para búsqueda por clave primaria
        }

        /// <summary>
        /// Obtiene todas las entidades de la tabla.
        /// CUIDADO: Puede ser costoso con tablas grandes
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Crea una nueva entidad en la base de datos
        /// Retorna la entidad creada (con ID asignado si es autoincremental)
        /// </summary>
        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // Marca la entidad para inserción
            await _context.SaveChangesAsync(); // Ejecuta el INSERT en la BD
            return entity; // Retorna la entidad con el ID asignado
        }

        /// <summary>
        /// Actualiza una entidad existente.
        /// Entity Framework rastrea los cambios automáticamente
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Elimina una entidad por su ID
        /// Retorna true si se eliminó, false si no existía
        /// </summary>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Busca entidades que cumplan una condición específica.
        /// Ejemplo de uso: FindAsync(u => u.IsActive == true)
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Obtiene la primera entidad que cumple una condición, o null si no encuentra.
        /// Útil para búsquedas únicas: FirstOrDefaultAsync(u => u.Email == "test@test.com")
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Verifica si existe al menos una entidad que cumple la condición
        /// Más eficiente que contar cuando solo necesitas saber si existe
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Cuenta el total de registros en la tabla
        /// </summary>
        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

    }
}
