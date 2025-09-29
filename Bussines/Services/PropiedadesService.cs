using APITemplate.Bussines.DTOs.Propiedades;
using APITemplate.Bussines.Interfaces;
using APITemplate.Data.Interefaces;
using APITemplate.Data.Repositories;
using System.Data;
using System.Text.RegularExpressions;

namespace APITemplate.Bussines.Services
{
    public class PropiedadesService : IPropiedadesService
    {

        private readonly IPropiedadesRepository _propiedadesRepository;

        public PropiedadesService(IPropiedadesRepository propiedadesRepository)
        {
            _propiedadesRepository = propiedadesRepository;
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
        /// Arma la entidad Popiedades en base a la tabla recibida.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private PropiedadesDTO MapearPropiedadesADto(DataRow row, List<string> tags)
        {
            // Datos base
            var barrio = row["BarrioNombre"].ToString() ?? string.Empty;
            var ciudad = row["Ciudad"].ToString() ?? string.Empty;
            var provincia = row["Provincia"].ToString() ?? string.Empty;
            var superficieTerreno = row["Superficie_terreno"] != DBNull.Value ? Convert.ToDecimal(row["Superficie_terreno"]) : (decimal?)null;
            var superficieConstruida = row["Superficie_construida"] != DBNull.Value ? Convert.ToDecimal(row["Superficie_construida"]) : (decimal?)null;
            var habitaciones = row["Habitaciones"] != DBNull.Value ? Convert.ToInt32(row["Habitaciones"]) : (int?)null;
            var sanitarios = row["Sanitario"] != DBNull.Value ? Convert.ToInt32(row["Sanitario"]) : (int?)null;
            var esDestacada = row["EsDestacada"] != DBNull.Value && Convert.ToBoolean(row["EsDestacada"]);

            return new PropiedadesDTO
            {
                Id = Convert.ToInt32(row["Id_propiedad"]),
                TipoId = Convert.ToInt32(row["Id_tipo"]),
                BarrioNombre = barrio,
                Ciudad = ciudad,
                Provincia = provincia,
                BarrioCompleto = $"{barrio}, {ciudad}, {provincia}".Trim(new char[] { ',', ' ' }),
                Titulo = row["Titulo"].ToString() ?? string.Empty,
                Subtitulo = row["Subtitulo"].ToString() ?? string.Empty,
                Descripcion = row["Descripcion"]?.ToString(),
                Direccion = row["Direccion"]?.ToString(),
                SuperficieTerreno = superficieTerreno,
                SuperficieConstruida = superficieConstruida,
                SuperficieResumen = $"{(superficieConstruida?.ToString() ?? "0")}m² / {(superficieTerreno?.ToString() ?? "0")}m²",
                Antiguedad = row["Antiguedad"] != DBNull.Value ? Convert.ToInt32(row["Antiguedad"]) : (int?)null,
                Habitaciones = habitaciones,
                Sanitarios = sanitarios,
                Cocheras = row["Cochera"] != DBNull.Value ? Convert.ToInt32(row["Cochera"]) : (int?)null,
                AmbientesResumen = $"{(habitaciones ?? 0)} hab, {(sanitarios ?? 0)} baño{(sanitarios > 1 ? "s" : "")}",
                EsDestacada = esDestacada,

                Tags = tags // Lista de strings
            };
        }

    }
}
