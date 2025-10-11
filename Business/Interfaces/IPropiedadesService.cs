using APITemplate.Business.DTOs.Barrios;
using APITemplate.Business.DTOs.Propiedades;
using APITemplate.Business.DTOs.TiposPropiedad;


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
