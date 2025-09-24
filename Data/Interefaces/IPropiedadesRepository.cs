using APITemplate.Models;
using System.Data;

namespace APITemplate.Data.Interefaces
{
    public interface IPropiedadesRepository : IBaseRepository<PROPIEDADES>
    {
        Task<PROPIEDADES> GetPropiedadConDetallesAsync();
        Task<DataTable> GetPropiedadesAsync();
    }
}
