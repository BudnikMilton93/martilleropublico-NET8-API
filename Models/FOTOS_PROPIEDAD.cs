using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("FOTOS_PROPIEDAD")]
    public class FOTOS_PROPIEDAD
    {
        [Key]
        public int Id_foto { get; set; }

        [Required]
        [ForeignKey(nameof(Propiedad))]
        public int Id_propiedad { get; set; }


        [Required]
        [MaxLength(255)]
        public required string Nombre_archivo { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Ruta_archivo { get; set; }

        public bool Es_principal { get; set; } = false;

        [Range(1, 10)]
        public int Orden_visualizacion { get; set; } = 1;

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        public DateTime Fecha_subida { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;
        public PROPIEDADES? Propiedad { get; set; }


    }
}
