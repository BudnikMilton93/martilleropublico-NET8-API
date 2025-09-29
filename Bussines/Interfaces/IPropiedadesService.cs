using APITemplate.Bussines.DTOs.Propiedades;

namespace APITemplate.Bussines.Interfaces
{
    public interface IPropiedadesService
    {
        Task<IEnumerable<PropiedadesDTO>> GetPropiedadesAsync();
    }
}
