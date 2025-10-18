using Amazon.S3;
using Amazon.S3.Model;
using APITemplate.Bussines.Services;
using APITemplate.Data.Interfaces;
using APITemplate.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APITemplate.Data.Repositories
{
    public class PropiedadesRepository : BaseRepository<PROPIEDADES>, IPropiedadesRepository
    {
        private readonly S3Service _s3Service;

        /// <summary>
        /// Constructor: Hereda de BaseRepository y le pasa el contexto
        /// </summary>
        public PropiedadesRepository(AppDbContext context, S3Service s3Service) : base(context)
        {
            _s3Service = s3Service;
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
                P.Fecha_Alta
            FROM
                PROPIEDADES AS P
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


        /// <summary>
        /// Obtiene las localidades disponibles.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propiedad"></param>
        /// <returns></returns>
        public async Task<PROPIEDADES?> CreateAsync(PROPIEDADES propiedad)
        {
            _context.Propiedades.Add(propiedad);
            await _context.SaveChangesAsync();
            return propiedad; // al guardar, EF Core rellena el Id generado automáticamente
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeletePropiedadConFotosAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Buscar la propiedad junto con sus fotos
                var propiedad = await _context.Propiedades
                    .Include(p => p.Fotos)
                    .FirstOrDefaultAsync(p => p.Id_propiedad == id);

                if (propiedad == null)
                    return false;

                bool exito = await _s3Service.EliminarCarpetaFotosAsync($"propiedades/{id}/");
                if (!exito)
                    throw new Exception($"No se pudo eliminar la carpeta de fotos de la propiedad {id} en S3");

                // Eliminar registros en base
                _context.FotosPropiedad.RemoveRange(propiedad.Fotos);
                _context.Propiedades.Remove(propiedad);

                // Guardar cambios y confirmar transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error eliminando propiedad {id}: {ex.Message}");
                return false;
            }
        }
    }
}
