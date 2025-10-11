using APITemplate.Business.DTOs.FotosPropiedad;
using APITemplate.Data.Interfaces;

namespace APITemplate.Business.Services
{
    public class FotosPropiedadService : IFotosPropiedadRepository
    {
        public Task<IEnumerable<FotosPropiedadDTO>> GuardarPropiedadAsync(FotosPropiedadDTO fotos)
        {
            throw new NotImplementedException();
        }
    }
}
