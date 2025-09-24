using APITemplate.Bussines.DTOs.Propiedades;
using APITemplate.Bussines.Services;
using APITemplate.Services;
using Microsoft.AspNetCore.Mvc;


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
        /// Devuelve una propiedad por Id
        /// (deberías armar un método en el Service/Repository si lo necesitás)
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PropiedadesDTO>> Get(int id)
        {
            // Por ahora no lo tenés implementado, pero lo dejamos planteado
            return NotFound("Método no implementado todavía");
        }
    }
}
