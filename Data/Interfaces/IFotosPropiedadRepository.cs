using APITemplate.Business.DTOs.FotosPropiedad;

namespace APITemplate.Data.Interfaces
{
    public interface IFotosPropiedadRepository
    {
        Task<IEnumerable<FotosPropiedadDTO>> GuardarPropiedadAsync(FotosPropiedadDTO fotos);
    }
}
