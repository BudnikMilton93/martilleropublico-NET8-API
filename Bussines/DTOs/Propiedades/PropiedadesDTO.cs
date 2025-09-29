using APITemplate.Bussines.DTOs.FotosPropiedad;
using APITemplate.Models;

namespace APITemplate.Bussines.DTOs.Propiedades
{
    public class PropiedadesDTO
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public string BarrioNombre { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string BarrioCompleto { get; set; } = string.Empty; 
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public decimal? SuperficieTerreno { get; set; }
        public decimal? SuperficieConstruida { get; set; }
        public string SuperficieResumen { get; set; } = string.Empty; 
        public int? Antiguedad { get; set; }
        public int? Habitaciones { get; set; }
        public int? Sanitarios { get; set; }
        public int? Cocheras { get; set; }
        public string AmbientesResumen { get; set; } = string.Empty; 
        public bool EsDestacada { get; set; }

        public List<FotosPropiedadDTO> Fotos { get; set; } = new();
        public List <String> Tags { get; set; } = new();
    }
}
