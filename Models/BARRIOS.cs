using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("BARRIOS")]
    public class BARRIOS
    {
        [Key]
        public int Id_barrio { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Ciudad { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Provincia { get; set; }

        public bool Activo { get; set; } = true;

    }
}
