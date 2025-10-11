using APITemplate.Data.Interfaces;
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

       

        /// <summary>
        /// Obtiene todas las propiedades usando SQL Raw
        /// </summary>
        public async Task<DataTable> _GetPropiedadesAsync()
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
                P.Subtitulo,
                P.Descripcion,
                P.Direccion,
                P.Superficie_terreno,
                P.Superficie_construida,
                P.Antiguedad,
                P.Habitaciones,
                P.Sanitario,
                P.Cochera,
                P.EsDestacada,
                P.Marca,
                P.Servicios_incluidos,
                P.Modelo,
                P.Fabricacion,
                P.Kilometraje,
                P.Patente,
                CP.Nombre AS Tags
            FROM
                PROPIEDADES AS P
            INNER JOIN BARRIOS AS B ON P.Id_barrio = B.Id_barrio
            INNER JOIN CARACTERISTICAS_PROPIEDAD AS CP ON P.Id_propiedad = CP.Id_propiedad
            ORDER BY P.Id_propiedad DESC";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        /// <summary>
        /// Obtiene los tipos de propiedades usando SQL Raw
        /// </summary>
        public async Task<DataTable> _GetTiposPropiedadAsync()
        {
            var sql = @"
            SELECT 
                TipoId
                ,Nombre
            FROM
                TIPOS_PROPIEDAD";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> _GetLocalidadesAsync()
        {
            var sql = @"
            SELECT 
                Id_barrio
                ,Nombre
                ,Ciudad
                ,Provincia
                ,Activo
            FROM 
                BARRIOS";

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
