using APITemplate.Bussines.DTOs.Barrios;
using APITemplate.Bussines.DTOs.Propiedades;
using APITemplate.Bussines.DTOs.TiposPropiedad;
using APITemplate.Bussines.Services;
using APITemplate.Services;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;


namespace APITemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadesController : ControllerBase
    {
        private readonly PropiedadesService _propiedadesService;

        public PropiedadesController(PropiedadesService propiedadesService)
        {
            _propiedadesService = propiedadesService;
        }

        /// <summary>
        /// Devuelve todas las propiedades con DTO
        /// </summary>
        [HttpGet("obtenerPropiedades")]
        public async Task<ActionResult<IEnumerable<PropiedadesDTO>>> ObtenerPropiedades()
        {
            var propiedades = await _propiedadesService.GetPropiedadesAsync();
            return Ok(propiedades);
        }

        /// <summary>
        /// Obtiene los tipos de propiedad
        /// </summary>
        /// <returns></returns>
        [HttpGet("obtenerTiposPropiedad")]
        public async Task<ActionResult<IEnumerable<TiposPropiedadDTO>>> ObtenerTiposPropiedad()
        {
            var tipos = await _propiedadesService.GetTiposPropiedadAsync();
            return Ok(tipos);
        }

        /// <summary>
        /// Obtiene las localidades existentes
        /// </summary>
        /// <returns></returns>
        [HttpGet("obtenerLocalidades")]
        public async Task<ActionResult<IEnumerable<LocalidadesDTO>>> ObtenerLocalidades()
        {
            var localidades = await _propiedadesService.GetLocalidadesAsync();
            return Ok(localidades);
        }

    }
}
