namespace APITemplate.Business.DTOs.Auth
{
    /// <summary>
    /// DTO que contiene únicamente los tokens (access + refresh)
    /// Se usa para operaciones de refresh token
    /// </summary>
    public class TokensDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
