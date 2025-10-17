using APITemplate.Business.DTOs.Barrios;
using APITemplate.Business.DTOs.Propiedades;
using APITemplate.Business.DTOs.TiposPropiedad;
using APITemplate.Business.Interfaces;
using APITemplate.Bussines.Interfaces;
using APITemplate.Models;
using APITemplate.Services;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;


namespace APITemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadesController : ControllerBase
    {
        private readonly IPropiedadesService _propiedadesService;

        public PropiedadesController(IPropiedadesService propiedadesService)
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

        /// <summary>
        /// Guarda una propiedad nueva con sus respectivas fotos
        /// </summary>
        /// <param name="propiedadNueva"></param>
        /// <param name="fotos"></param>
        /// <param name="archivos"></param>
        /// <returns></returns>
        [HttpPost("guardarPropiedad")]
        public async Task<IActionResult> GuardarPropiedad([FromForm] PropiedadesDTO propiedadNueva, [FromForm] string fotos, [FromForm] List<IFormFile> archivos)
        {
            if (propiedadNueva == null)
                return BadRequest("Datos de la propiedad inválidos.");
            try
            {
                var propiedadGuardada = await _propiedadesService.GuardarPropiedadAsync(propiedadNueva, fotos, archivos);
                return Ok(propiedadGuardada);
            }
            catch (Exception ex)
            {
                // Logueá el error si querés, o devolvelo para depurar
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una propiedad y sus fotos
        /// </summary>
        /// <param name="propiedadNueva"></param>
        /// <param name="fotos"></param>
        /// <param name="archivos"></param>
        /// <returns></returns>
        [HttpPost("actualizarPropiedad")]
        public async Task<IActionResult> ActualizarPropiedad([FromForm] PropiedadesDTO propiedadNueva, [FromForm] string fotos, [FromForm] List<IFormFile> archivos)
        {
            if (propiedadNueva == null)
                return BadRequest("Datos de la propiedad inválidos.");
            try
            {
                var propiedadGuardada = await _propiedadesService.ActualizarPropiedadAsync(propiedadNueva, fotos, archivos);
                return Ok(propiedadGuardada);
            }
            catch (Exception ex)
            {
                // Logueá el error si querés, o devolvelo para depurar
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
