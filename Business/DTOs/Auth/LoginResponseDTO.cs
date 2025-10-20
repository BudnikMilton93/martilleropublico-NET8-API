namespace APITemplate.Business.DTOs.Auth
{
    /// <summary>
    /// DTO que contiene toda la información de respuesta al hacer login
    /// </summary>
    public class LoginResponseDTO
    {
        /// <summary>
        /// Access Token JWT (válido por minutos configurados)
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh Token (válido por días configurados)
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Fecha de expiración del Access Token
        /// </summary>
        public DateTime AccessTokenExpiresAt { get; set; }

        /// <summary>
        /// Fecha de expiración del Refresh Token
        /// </summary>
        public DateTime RefreshTokenExpiresAt { get; set; }

        /// <summary>
        /// Información básica del usuario autenticado
        /// </summary>
        public UserBasicInfoDTO User { get; set; }
    }
}
