using APITemplate.Models;

namespace APITemplate.Data.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<USUARIOS>
    {
        Task<USUARIOS?> GetByEmailAsync(string email);
        Task<USUARIOS?> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<IEnumerable<USUARIOS>> GetActiveUsersAsync();
    }
}
