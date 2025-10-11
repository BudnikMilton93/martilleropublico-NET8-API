using APITemplate.Services;
using Microsoft.AspNetCore.Mvc;


namespace APITemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheTestController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor del controlador <c>CacheTestController</c>.
        /// Recibe mediante inyección de dependencias una instancia de <see cref="ICacheService"/>
        /// para permitir el almacenamiento y recuperación de datos en caché.
        /// </summary>
        /// <param name="cacheService">
        /// Servicio de caché que implementa las operaciones de guardado, obtención y eliminación
        /// de datos de forma asíncrona.
        /// </param>
        public CacheTestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }


        /// <summary>
        /// Endpoint que almacena en caché un mensaje de bienvenida por 1 minuto.
        /// Utiliza el servicio de caché configurado (_cacheService) para guardar
        /// el valor de forma asíncrona y devuelve un mensaje JSON indicando si la 
        /// operación fue exitosa.
        /// </summary>
        /// <returns>
        /// Devuelve un resultado HTTP 200 con un mensaje en formato JSON:
        /// </returns>
        [HttpGet("store")]
        public async Task<IActionResult> StoreInCache()
        {
            string key = "welcome_message";
            string value = "¡Bienvenido a APITemplate!";
            bool stored = await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1));

            return Ok(new { message = stored ? "Guardado en caché" : "No se pudo guardar" });
        }


        /// <summary>
        /// Endpoint que obtiene un valor de la caché utilizando la clave "welcome_message".
        /// Si el valor existe, lo devuelve en un objeto JSON; de lo contrario, responde con
        /// un estado HTTP 404 indicando que no hay datos almacenados.
        /// </summary>
        /// <returns>
        /// Devuelve un resultado HTTP 200 con el valor almacenado:
        /// { "cachedValue": "valor guardado" } si existe,
        /// o un resultado HTTP 404 con el mensaje:
        /// { "message": "No hay valor en caché" } si no existe.
        /// </returns>
        [HttpGet("retrieve")]
        public async Task<IActionResult> RetrieveFromCache()
        {
            string key = "welcome_message";
            var value = await _cacheService.GetAsync<string>(key);

            if (value == null)
                return NotFound(new { message = "No hay valor en caché" });

            return Ok(new { cachedValue = value });
        }
    }
}
