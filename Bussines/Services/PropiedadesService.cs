using APITemplate.Bussines.DTOs.Barrios;
using APITemplate.Bussines.DTOs.Propiedades;
using APITemplate.Bussines.DTOs.TiposPropiedad;
using APITemplate.Bussines.Interfaces;
using APITemplate.Data.Interefaces;
using APITemplate.Data.Repositories;
using APITemplate.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace APITemplate.Bussines.Services
{
    public class PropiedadesService : IPropiedadesService
    {

        private readonly IPropiedadesRepository _propiedadesRepository;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PropiedadesService"/> con el repositorio inyectado.
        /// </summary>
        /// <param name="propiedadesRepository">Repositorio de propiedades utilizado para acceder a la base de datos.</param>
        public PropiedadesService(IPropiedadesRepository propiedadesRepository)
        {
            _propiedadesRepository = propiedadesRepository;
        }

        #region "Peticiones"

        /// <summary>
        /// Obtiene las Localidades disponibles
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LocalidadesDTO>> GetLocalidadesAsync()
        {
            var dataTable = await _propiedadesRepository._GetLocalidadesAsync();
            return MapearLocalidades(dataTable);

        }

        /// <summary>
        /// Obtiene todas las propiedades con lógica de negocio aplicada
        /// </summary>
        public async Task<IEnumerable<PropiedadesDTO>> GetPropiedadesAsync()
        {
            //  Obtener datos crudos del repository
            var dataTable = await _propiedadesRepository._GetPropiedadesAsync();

            // Agrupar por id
            var propiedadesAgrupadas = dataTable.AsEnumerable()
                .GroupBy(row => Convert.ToInt32(row["Id_propiedad"]))
                .ToList();

            //  Aplicar lógica de negocio y transformar a DTO
            var propiedadesDto = new List<PropiedadesDTO>();

            foreach (var grupo in propiedadesAgrupadas)
            {
                // Tomar la primera fila para datos de propiedad (son iguales en todas)
                var primeraFila = grupo.First();

                // Recopilar todos los tags de todas las filas del grupo
                var tags = grupo.Select(row => row["Tags"]?.ToString())
                               .Where(tag => !string.IsNullOrEmpty(tag))
                               .Distinct()
                               .ToList();

                var propiedadDto = MapearPropiedadesADto(primeraFila, tags);
                propiedadesDto.Add(propiedadDto);
            }

            return propiedadesDto;
        }

        /// <summary>
        /// Obtiene los Tipos de Propiedades existentes.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TiposPropiedadDTO>> GetTiposPropiedadAsync()
        {
            // Obtener datos crudos desde el repositorio
            var dataTable = await _propiedadesRepository._GetTiposPropiedadAsync();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return Enumerable.Empty<TiposPropiedadDTO>();

            return dataTable.AsEnumerable().Select(MapearTipoPropiedad).ToList();
        }

        #endregion


        #region "Mapeado de datos"
        
        /// <summary>
        /// Arma la entidad Popiedades en base a la tabla recibida.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private PropiedadesDTO MapearPropiedadesADto(DataRow row, List<string> tags)
        {
            // Datos base de ubicación
            var barrio = row["BarrioNombre"].ToString() ?? string.Empty;
            var ciudad = row["Ciudad"].ToString() ?? string.Empty;
            var provincia = row["Provincia"].ToString() ?? string.Empty;

            // Inmueble
            var superficieTerreno = row["Superficie_terreno"] != DBNull.Value ? Convert.ToDecimal(row["Superficie_terreno"]) : (decimal?)null;
            var superficieConstruida = row["Superficie_construida"] != DBNull.Value ? Convert.ToDecimal(row["Superficie_construida"]) : (decimal?)null;
            var habitaciones = row["Habitaciones"] != DBNull.Value ? Convert.ToInt32(row["Habitaciones"]) : (int?)null;
            var sanitarios = row["Sanitario"] != DBNull.Value ? Convert.ToInt32(row["Sanitario"]) : (int?)null;
            var cocheras = row["Cochera"] != DBNull.Value ? Convert.ToInt32(row["Cochera"]) : (int?)null;
            var esDestacada = row["EsDestacada"] != DBNull.Value && Convert.ToBoolean(row["EsDestacada"]);

            // Vehículo
            var marca = row["Marca"]?.ToString();
            var modelo = row["Modelo"]?.ToString();
            var fabricacion = row["Fabricacion"] != DBNull.Value ? Convert.ToInt32(row["Fabricacion"]) : (int?)null;
            var kilometraje = row["Kilometraje"] != DBNull.Value ? Convert.ToInt32(row["Kilometraje"]) : (int?)null;
            var patente = row["Patente"]?.ToString();

            // Alquiler
            var serviciosIncluidos = row["Servicios_incluidos"]?.ToString();

            return new PropiedadesDTO
            {
                Id = Convert.ToInt32(row["Id_propiedad"]),
                TipoId = Convert.ToInt32(row["Id_tipo"]),

                // Datos generales
                Titulo = row["Titulo"].ToString() ?? string.Empty,
                Subtitulo = row["Subtitulo"].ToString() ?? string.Empty,
                Descripcion = row["Descripcion"]?.ToString(),
                Direccion = row["Direccion"]?.ToString(),

                // Ubicación
                BarrioNombre = barrio,
                Ciudad = ciudad,
                Provincia = provincia,
                BarrioCompleto = $"{barrio}, {ciudad}, {provincia}".Trim(',', ' '),

                // Inmueble
                SuperficieTerreno = superficieTerreno,
                SuperficieConstruida = superficieConstruida,
                SuperficieResumen = $"{(superficieConstruida ?? 0)}m² / {(superficieTerreno ?? 0)}m²",
                Antiguedad = row["Antiguedad"] != DBNull.Value ? Convert.ToInt32(row["Antiguedad"]) : (int?)null,
                Habitaciones = habitaciones,
                Sanitarios = sanitarios,
                Cocheras = cocheras,
                AmbientesResumen = $"{(habitaciones ?? 0)} hab, {(sanitarios ?? 0)} baño{(sanitarios > 1 ? "s" : "")}",

                // Vehículo
                Marca = marca,
                Modelo = modelo,
                Fabricacion = fabricacion,
                Kilometraje = kilometraje,
                Patente = patente,

                // Alquiler
                ServiciosIncluidos = serviciosIncluidos,

                EsDestacada = esDestacada,
                Tags = tags
            };
        }


        /// <summary>
        /// Arma la entidad TiposPropiedad en base a la tabla recibida.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private TiposPropiedadDTO MapearTipoPropiedad(DataRow row)
        {
            return new TiposPropiedadDTO
            {
                TipoId = row["TipoId"] != DBNull.Value ? Convert.ToInt32(row["TipoId"]) : 0,
                Nombre = row["Nombre"]?.ToString() ?? string.Empty
            };
        }


        /// <summary>
        /// Mapea un DataTable de BARRIOS a IEnumerable<LocalidadesDTO> agrupando por ciudad
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<LocalidadesDTO> MapearLocalidades(DataTable dataTable)
        {
            // Convertimos a Enumerable y agrupamos por ciudad
            var grouped = dataTable.AsEnumerable()
                .GroupBy(row => row["Ciudad"]?.ToString() ?? string.Empty);

            var localidades = new List<LocalidadesDTO>();

            foreach (var grupo in grouped)
            {
                var localidad = new LocalidadesDTO
                {
                    Ciudad = grupo.Key,
                    Barrios = grupo.Select(row => new BARRIOS
                    {
                        Id_barrio = row["Id_barrio"] != DBNull.Value ? Convert.ToInt32(row["Id_barrio"]) : 0,
                        Nombre = row["Nombre"]?.ToString() ?? string.Empty,
                        Ciudad = row["Ciudad"]?.ToString() ?? string.Empty,
                        Provincia = row["Provincia"]?.ToString() ?? string.Empty
                    }).ToList()
                };

                localidades.Add(localidad);
            }

            return localidades;
        }

        #endregion

    }
}
