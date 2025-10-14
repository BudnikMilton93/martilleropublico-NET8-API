using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Business.Interfaces;
using APITemplate.Bussines.Services;
using APITemplate.Data.Interfaces;
using APITemplate.Models;

namespace APITemplate.Business.Services
{
    public class FotosPropiedadService : IFotosPropiedadService
    {
        private readonly S3Service _s3Service;
        private readonly IFotosPropiedadRepository _fotosRepository;

        public FotosPropiedadService(S3Service s3Service, IFotosPropiedadRepository fotosRepository)
        {
            _s3Service = s3Service;
            _fotosRepository = fotosRepository;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPropiedad"></param>
        /// <param name="fotosJson"></param>
        /// <param name="archivos"></param>
        /// <returns></returns>
        public async Task<bool> GuardarFotosPropiedadAsync(int idPropiedad, string fotosJson, List<IFormFile> archivos)
        {
            if (archivos == null || archivos.Count == 0)
                return false;

            var fotosMetadata = System.Text.Json.JsonSerializer.Deserialize<List<FotosPropiedadDTO>>(fotosJson);
            if (fotosMetadata == null || fotosMetadata.Count == 0)
                return false;

            bool todasOk = true;

            for (int i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                var meta = fotosMetadata.ElementAtOrDefault(i);
                if (meta == null) continue;

                try
                {
                    using var stream = archivo.OpenReadStream();

                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreUnico = $"propiedades/{idPropiedad}/{meta.OrdenVisualizacion:D2}_{Guid.NewGuid()}{extension}";

                    // Subir a S3
                    var rutaS3 = await _s3Service.SubirFotosAsync(stream, nombreUnico, archivo.ContentType);

                    // Solo guardar en base si la subida fue exitosa
                    if (!string.IsNullOrWhiteSpace(rutaS3))
                    {
                        var foto = new FOTOS_PROPIEDAD
                        {
                            Id_propiedad = idPropiedad,
                            Nombre_archivo = Path.GetFileName(nombreUnico), // solo el nombre
                            Ruta_archivo = rutaS3,                          // URL completa de S3
                            Es_principal = meta.EsPrincipal,
                            Orden_visualizacion = meta.OrdenVisualizacion,
                            Descripcion = meta.Descripcion,
                            Fecha_subida = DateTime.UtcNow,
                            Activo = true
                        };

                        var resultado = await _fotosRepository.CreateAsync(foto);
                    }
                    else
                    {
                        todasOk = false;
                        Console.WriteLine($"No se recibió ruta válida de S3 para {archivo.FileName}");
                    }
                }
                catch (Exception ex)
                {
                    todasOk = false;
                    Console.WriteLine($"Error subiendo {archivo.FileName}: {ex.Message}");
                }
            }

            return todasOk;
        }
    }
}
