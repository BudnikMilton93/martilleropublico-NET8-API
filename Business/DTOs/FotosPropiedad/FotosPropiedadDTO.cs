namespace APITemplate.Business.DTOs.FotosPropiedad
{
    public class FotosPropiedadDTO
    {
        public int Id { get; set; }
        public int IdPropiedad { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
        public int OrdenVisualizacion { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaSubida { get; set; }
        public bool Activo { get; set; }
    }
}
