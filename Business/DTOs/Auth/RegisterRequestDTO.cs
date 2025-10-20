using System.ComponentModel.DataAnnotations;

namespace APITemplate.Business.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        [MaxLength(50, ErrorMessage = "El username no puede exceder 50 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(50)]
        public string Apellido { get; set; }

        public int IdRol { get; set; } = 2; // Valor por defecto: usuario normal
    }
}
