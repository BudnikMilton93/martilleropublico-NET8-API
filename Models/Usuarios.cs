using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("USUARIOS")]
    public class USUARIOS
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [MaxLength(50)]
        public required string Nombre { get; set; }

        [MaxLength(50)]
        public required string Apellido { get; set; }

        public int Id_Rol { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
