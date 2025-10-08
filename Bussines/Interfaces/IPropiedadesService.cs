using APITemplate.Bussines.DTOs.Barrios;
using APITemplate.Bussines.DTOs.Propiedades;
using APITemplate.Bussines.DTOs.TiposPropiedad;

namespace APITemplate.Bussines.Interfaces
{
    public interface IPropiedadesService
    {
        Task<IEnumerable<PropiedadesDTO>> GetPropiedadesAsync();
        Task<IEnumerable<TiposPropiedadDTO>> GetTiposPropiedadAsync();
        Task<IEnumerable<LocalidadesDTO>> GetLocalidadesAsync();
        Task<IEnumerable<PropiedadesDTO>> GuardarPropiedadAsync(PropiedadesDTO propiedad);

    }
}
