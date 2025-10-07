using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace APITemplate.Models
{
    [Table("PROPIEDADES")]
    public class PROPIEDADES
    {
        [Key]
        public int Id_propiedad { get; set; }

        [Required]
        public int Id_tipo { get; set; }

        [Required]
        public int Id_barrio { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Titulo { get; set; }

        [MaxLength(200)]
        public string? Subtitulo { get; set; }

        [MaxLength(300)]
        public string? Descripcion { get; set; }

        [MaxLength(200)]
        public string? Direccion { get; set; }

        
        // Datos específicos de inmuebles
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Superficie_terreno { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Superficie_construida { get; set; }

        public int? Antiguedad { get; set; }

        public int? Habitaciones { get; set; }

        public int? Sanitario { get; set; }

        public int? Cochera { get; set; }


        // Datos específicos de vehículos
        [MaxLength(100)]
        public string? Marca { get; set; }

        [MaxLength(100)]
        public string? Modelo { get; set; }

        public int? Fabricacion { get; set; }

        public int? Kilometraje { get; set; }

        [MaxLength(20)]
        public string? Patente { get; set; }

        public bool EsDestacada { get; set; } = false;


        // Alquiler
        [MaxLength(50)]
        public string? Servicios_incluidos { get; set; }

        // Relaciones
        public virtual BARRIOS? Barrio { get; set; }

        public virtual ICollection<CARACTERISTICAS_PROPIEDAD>? Caracteristicas { get; set; }

        public virtual ICollection<FOTOS_PROPIEDAD>? Fotos { get; set; }
    }
}
