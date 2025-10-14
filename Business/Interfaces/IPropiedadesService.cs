using APITemplate.Business.DTOs.Barrios;
using APITemplate.Business.DTOs.Propiedades;
using APITemplate.Business.DTOs.TiposPropiedad;
using Microsoft.AspNetCore.Mvc;


namespace APITemplate.Bussines.Interfaces
{
    public interface IPropiedadesService
    {
        Task<IEnumerable<PropiedadesDTO>> GetPropiedadesAsync();
        Task<IEnumerable<TiposPropiedadDTO>> GetTiposPropiedadAsync();
        Task<IEnumerable<LocalidadesDTO>> GetLocalidadesAsync();
        Task<bool> GuardarPropiedadAsync(PropiedadesDTO propiedad, [FromForm] string fotos, [FromForm] List<IFormFile> archivos);

    }
}
