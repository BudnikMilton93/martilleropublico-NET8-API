using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Business.DTOs.Propiedades;

namespace APITemplate.Business.Interfaces
{
    public interface IFotosPropiedadService
    {
        Task<bool> GuardarFotosPropiedadAsync(int idPropiedad, string fotosJson, List<IFormFile> archivos);

        Task<IEnumerable<FotosPropiedadDTO>> GetFotosPropiedadAsync();

        Task<bool> ActualizarFotosPropiedadAsync(int idPropiedad, string fotosJson, List<IFormFile>? archivos);
    }
}
