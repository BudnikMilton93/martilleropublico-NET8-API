using APITemplate.Bussines.Services;
using APITemplate.Data.Interfaces;
using APITemplate.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> _GetFotosPropiedadAsync()
        {
            var sql = @"
                SELECT 
                    FP.Id_foto,
                    FP.Id_propiedad,
                    FP.Nombre_archivo,
                    FP.Ruta_archivo,
                    FP.Es_principal,
                    FP.Orden_visualizacion,
                    FP.Descripcion,
                    FP.Fecha_subida,
                    FP.Activo
                FROM FOTOS_PROPIEDAD AS FP
                WHERE FP.Activo = 1";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

    }
}
