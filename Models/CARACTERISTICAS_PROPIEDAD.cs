using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("CARACTERISTICAS_PROPIEDAD")]
    public class CARACTERISTICAS_PROPIEDAD
    {
        [Key]
        public int Id_caracteristica { get; set; }

        [Required]
        public int Id_propiedad { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(300)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

    }
}
