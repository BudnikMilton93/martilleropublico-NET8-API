using APITemplate.Models;
using System.Security.Claims;

namespace APITemplate.Services.Interfaces
{
    /// <summary>
    /// Responsable SOLO de operaciones con tokens
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Genera un Access Token (JWT) para un usuario autenticado
        /// </summary>
        /// <param name="user">Usuario para el cual se genera el token</param>
        /// <returns>String con el token JWT</returns>
        string GenerateAccessToken(USUARIOS user);

        /// <summary>
        /// Genera un Refresh Token aleatorio y seguro
        /// </summary>
        /// <returns>Tupla con el token y su fecha de expiración</returns>
        (string Token, DateTime ExpiryTime) GenerateRefreshToken();

        /// <summary>
        /// Valida si un token JWT es válido (firma, expiración, etc.)
        /// </summary>
        /// <param name="token">Token a validar</param>
        /// <returns>True si es válido, False si no</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Extrae los Claims de un token JWT válido
        /// </summary>
        /// <param name="token">Token del cual extraer claims</param>
        /// <returns>ClaimsPrincipal con la información del token</returns>
        ClaimsPrincipal? GetPrincipalFromToken(string token);

        /// <summary>
        /// Extrae el UserId del token (del claim Sub o NameIdentifier)
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID del usuario o null si no se puede extraer</returns>
        int? GetUserIdFromToken(string token);

        /// <summary>
        /// Obtiene la configuración de JWT Settings
        /// </summary>
        IConfigurationSection GetJwtSettings();
    }
}
