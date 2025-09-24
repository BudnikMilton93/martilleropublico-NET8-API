using APITemplate.Data.Interefaces;
using APITemplate.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APITemplate.Data.Repositories
{
    public class PropiedadesRepository : BaseRepository<PROPIEDADES>, IPropiedadesRepository
    {
        /// <summary>
        /// Constructor: Hereda de BaseRepository y le pasa el contexto
        /// </summary>
        public PropiedadesRepository(AppDbContext context) : base(context)
        {
        }

        public Task<PROPIEDADES> GetPropiedadConDetallesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene todas las propiedades usando SQL Raw
        /// </summary>
        public async Task<DataTable> GetPropiedadesAsync()
        {
            var sql = @"
            SELECT 
                P.Id_propiedad,
                P.Id_tipo,
                B.Nombre as BarrioNombre,
                B.Ciudad,
                B.Provincia,
                P.Id_barrio,
                P.Titulo,
                P.Descripcion,
                P.Direccion,
                P.Superficie_terreno,
                P.Superficie_construida,
                P.Antiguedad,
                P.Habitaciones,
                P.Sanitario,
                P.Cochera,
                P.EsDestacada
            FROM PROPIEDADES AS P
            INNER JOIN BARRIOS AS B ON P.Id_barrio = B.Id_barrio
            ORDER BY P.Id_propiedad DESC";

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
