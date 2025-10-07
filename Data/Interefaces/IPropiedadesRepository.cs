using APITemplate.Models;
using System.Data;

namespace APITemplate.Data.Interefaces
{
    public interface IPropiedadesRepository : IBaseRepository<PROPIEDADES>
    {
        Task<DataTable> _GetPropiedadesAsync();
        Task<DataTable> _GetTiposPropiedadAsync();
        Task<DataTable> _GetLocalidadesAsync();
    }
}
