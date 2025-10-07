using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("TIPOS_PROPIEDAD")]
    public class TIPOS_PROPIEDAD
    {
        [Key]
        public int TipoId { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}
