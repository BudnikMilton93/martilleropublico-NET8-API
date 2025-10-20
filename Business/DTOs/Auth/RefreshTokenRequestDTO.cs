using System.ComponentModel.DataAnnotations;

namespace APITemplate.Business.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "El access token es obligatorio")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "El refresh token es obligatorio")]
        public string RefreshToken { get; set; }
    }
}
