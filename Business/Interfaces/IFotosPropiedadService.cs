using APITemplate.Business.DTOs.Propiedades;

namespace APITemplate.Business.Interfaces
{
    public interface IFotosPropiedadService
    {
        Task<bool> GuardarFotosPropiedadAsync(int idPropiedad, string fotosJson, List<IFormFile> archivos);
    }
}
