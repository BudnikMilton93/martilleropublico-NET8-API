using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Models;

namespace APITemplate.Business.DTOs.Propiedades
{
    public class PropiedadesDTO
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public int IdBarrio { get; set; } 
        public string BarrioNombre { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string BarrioCompleto { get; set; } = string.Empty; 
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? DireccionMaps { get; set; }
        public bool EsDestacada { get; set; }
        public DateTime FechaAlta { get; set; }


        // Casa / Alquiler / Terreno
        public string? SuperficieTerreno { get; set; } = "0";
        public string? SuperficieConstruida { get; set; } = "0";
        public string SuperficieResumen { get; set; } = string.Empty; 
        public int? Antiguedad { get; set; }
        public int? Habitaciones { get; set; }
        public int? Sanitarios { get; set; }
        public int? Cocheras { get; set; }
        public string AmbientesResumen { get; set; } = string.Empty; 


        // Datos específicos de vehículos
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Fabricacion { get; set; }
        public int? Kilometraje { get; set; }
        public string? Patente { get; set; }
        public string VehiculoResumen { get; set; } = string.Empty;
        
        // Alquiler
        public string? ServiciosIncluidos { get; set; }
        public string? AlquilerResumen { get; set; }

        public List<FotosPropiedadDTO> Fotos { get; set; } = new();
        public List <String> Tags { get; set; } = new();
    }
}
