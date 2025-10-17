using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Business.DTOs.Propiedades;
using APITemplate.Business.Interfaces;
using APITemplate.Bussines.Services;
using APITemplate.Data.Interfaces;
using APITemplate.Data.Repositories;
using APITemplate.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

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

        #region Peticiones
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FotosPropiedadDTO>> GetFotosPropiedadAsync()
        {
            var dataTable = await _fotosRepository._GetFotosPropiedadAsync();

            //  Aplicar lógica de negocio y transformar a DTO
            var fotosPropiedadDto = new List<FotosPropiedadDTO>();
            fotosPropiedadDto = MapearFotosPropiedad(dataTable);

            return fotosPropiedadDto;

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPropiedad"></param>
        /// <param name="fotosJson"></param>
        /// <param name="archivos"></param>
        /// <returns></returns>
        public async Task<bool> ActualizarFotosPropiedadAsync(int idPropiedad, string fotosJson, List<IFormFile>? archivos)
        {
            try
            {
                // Obtener las fotos actuales de la base
                var fotosExistentes = (await _fotosRepository.FindAsync(f => f.Id_propiedad == idPropiedad && f.Activo)).ToList();

                // Parsear el JSON recibido desde el front
                var fotosRecibidas = string.IsNullOrEmpty(fotosJson)
                                    ? new List<FotosPropiedadDTO>()
                                    : JsonSerializer.Deserialize<List<FotosPropiedadDTO>>(fotosJson,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                // Detectar fotos eliminadas
                var idsRecibidos = fotosRecibidas.Where(f => f.Id > 0).Select(f => f.Id).ToList();
                var fotosAEliminar = fotosExistentes.Where(f => !idsRecibidos.Contains(f.Id_foto)).ToList();

                foreach (var foto in fotosAEliminar)
                {
                    try
                    {
                        // Eliminar de S3
                        var key = foto.Ruta_archivo.Contains("amazonaws.com")
                            ? foto.Ruta_archivo.Substring(foto.Ruta_archivo.IndexOf(".com/") + 5)
                            : foto.Ruta_archivo;

                        await _s3Service.EliminarFotoAsync(key);

                        // Eliminar de la base
                        await _fotosRepository.DeleteAsync(foto.Id_foto);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error eliminando foto {foto.Ruta_archivo}: {ex.Message}");
                    }
                }

                // Si hay nuevas fotos, guardarlas con tu método existente
                if (archivos != null && archivos.Any())
                {
                    var nuevasFotos = fotosRecibidas.Where(f => f.Id == 0).ToList();
                    if (nuevasFotos.Any())
                    {
                        string nuevasFotosJson = System.Text.Json.JsonSerializer.Serialize(nuevasFotos);
                        await GuardarFotosPropiedadAsync(idPropiedad, nuevasFotosJson, archivos);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarFotosPropiedadAsync: {ex.Message}");
                return false;
            }
        }
        #endregion


        #region Mapeado de datos
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablaFotos"></param>
        /// <returns></returns>
        private List<FotosPropiedadDTO> MapearFotosPropiedad(DataTable tablaFotos)
        {
            var fotosList = tablaFotos.AsEnumerable()
            .Select(row => new FotosPropiedadDTO
            {
                Id = Convert.ToInt32(row["Id_foto"]),
                IdPropiedad = Convert.ToInt32(row["Id_propiedad"]),
                NombreArchivo = row["Nombre_archivo"]?.ToString() ?? string.Empty,
                RutaArchivo = row["Ruta_archivo"]?.ToString() ?? string.Empty,
                EsPrincipal = row["Es_principal"] != DBNull.Value && Convert.ToBoolean(row["Es_principal"]),
                OrdenVisualizacion = row["Orden_visualizacion"] != DBNull.Value ? Convert.ToInt32(row["Orden_visualizacion"]) : 1,
                Descripcion = row["Descripcion"]?.ToString(),
                FechaSubida = row["Fecha_subida"] != DBNull.Value ? Convert.ToDateTime(row["Fecha_subida"]) : DateTime.UtcNow,
                Activo = row["Activo"] != DBNull.Value && Convert.ToBoolean(row["Activo"])
            })
            .ToList();

            return fotosList;
        }
        #endregion

    }
}
