using APITemplate.Business.DTOs.Auth;

namespace APITemplate.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Autentica un usuario con email y password.
        /// Valida credenciales, genera tokens y actualiza refresh token en BD.
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>LoginResponseDTO con tokens y datos de usuario, o null si falla</returns>
        Task<LoginResponseDTO?> LoginAsync(string email, string password);

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="email">Email</param>
        /// <param name="password">Contraseña en texto plano (se hasheará)</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="apellido">Apellido</param>
        /// <param name="idRol">ID del rol</param>
        /// <returns>ID del usuario creado, o null si falla</returns>
        Task<int?> RegisterAsync(RegisterRequestDTO request);

        /// <summary>
        /// Obtiene el perfil de un usuario por su ID
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>UserProfileDTO con datos del usuario (sin password)</returns>
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);

        /// <summary>
        /// Refresca un Access Token usando un Refresh Token válido
        /// </summary>
        /// <param name="accessToken">Access token expirado o próximo a expirar</param>
        /// <param name="refreshToken">Refresh token válido</param>
        /// <returns>Nuevos tokens o null si el refresh token es inválido</returns>
        Task<TokensDTO?> RefreshTokenAsync(string accessToken, string refreshToken);

        /// <summary>
        /// Revoca el refresh token de un usuario (logout)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si se revocó exitosamente</returns>
        Task<bool> RevokeRefreshTokenAsync(int userId);

        /// <summary>
        /// Valida si un email ya está registrado
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Valida si un username ya está registrado
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);
    }
}
