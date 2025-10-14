using APITemplate.Data.Interfaces;
using APITemplate.Models;

namespace APITemplate.Data.Repositories
{
    public class FotosPropiedadRepository : BaseRepository<FOTOS_PROPIEDAD>, IFotosPropiedadRepository
    {
        /// <summary>
        /// Constructor: Hereda de BaseRepository y le pasa el contexto
        /// </summary>
        public FotosPropiedadRepository(AppDbContext context) : base(context)
        {
        }

    }
}
