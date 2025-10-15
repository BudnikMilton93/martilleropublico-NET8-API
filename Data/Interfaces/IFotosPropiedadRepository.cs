using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Data.Repositories;
using APITemplate.Models;
using System.Data;

namespace APITemplate.Data.Interfaces
{
    public interface IFotosPropiedadRepository : IBaseRepository<FOTOS_PROPIEDAD>
    {
        Task<DataTable> _GetFotosPropiedadAsync();
    }
}
