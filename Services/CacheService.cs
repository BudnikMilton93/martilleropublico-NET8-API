using StackExchange.Redis;
using System.Text.Json;

namespace ECommerceAPI.Services
{

    /// <summary>
    /// Implementación del servicio de caché que utiliza Redis como backend de almacenamiento.
    /// Permite guardar, recuperar y eliminar datos en memoria de forma asíncrona.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;

        /// <summary>
        /// Constructor de la clase <c>CacheService</c>.
        /// Recibe una conexión a Redis y obtiene una instancia de <see cref="IDatabase"/>
        /// para realizar operaciones de lectura/escritura en caché.
        /// </summary>
        /// <param name="redis">
        /// Conexión multiplexada a Redis, provista por <see cref="IConnectionMultiplexer"/>.
        /// </param>
        public CacheService(IConnectionMultiplexer redis)
        {
            _cacheDb = redis.GetDatabase();
        }


        /// <summary>
        /// Recupera datos de Redis por clave y los deserializa al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de dato esperado al deserializar el valor.</typeparam>
        /// <param name="key">Clave bajo la cual se almacenó el valor.</param>
        /// <returns>
        /// El valor deserializado si existe; de lo contrario, el valor por defecto para el tipo <typeparamref name="T"/>.
        /// </returns>
        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }


        /// <summary>
        /// Guarda datos en Redis serializándolos a JSON.
        /// </summary>
        /// <typeparam name="T">Tipo del valor a almacenar.</typeparam>
        /// <param name="key">Clave bajo la cual se guardará el valor.</param>
        /// <param name="value">Objeto a serializar y almacenar.</param>
        /// <param name="expiration">Tiempo de expiración opcional del valor en caché.</param>
        public async Task SetDataAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _cacheDb.StringSetAsync(key, json, expiration);
        }


        /// <summary>
        /// Elimina datos almacenados en Redis bajo la clave especificada.
        /// </summary>
        /// <param name="key">Clave a eliminar de la caché.</param>
        public async Task RemoveDataAsync(string key)
        {
            await _cacheDb.KeyDeleteAsync(key);
        }


        /// <summary>
        /// Guarda datos en Redis y devuelve un valor booleano que indica si la operación fue exitosa.
        /// </summary>
        /// <typeparam name="T">Tipo del valor a almacenar.</typeparam>
        /// <param name="key">Clave bajo la cual se guardará el valor.</param>
        /// <param name="value">Objeto a serializar y almacenar.</param>
        /// <param name="expiration">Tiempo de expiración opcional del valor en caché.</param>
        /// <returns><c>true</c> si el valor se almacenó correctamente; de lo contrario, <c>false</c>.</returns>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            return await _cacheDb.StringSetAsync(key, json, expiration);
        }


        /// <summary>
        /// Obtiene un valor de Redis y lo deserializa al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de dato esperado.</typeparam>
        /// <param name="key">Clave a buscar en Redis.</param>
        /// <returns>
        /// El valor deserializado si existe; de lo contrario, el valor por defecto para el tipo <typeparamref name="T"/>.
        /// </returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }


        /// <summary>
        /// Elimina un valor de Redis y devuelve un booleano indicando si fue eliminado.
        /// </summary>
        /// <param name="key">Clave a eliminar.</param>
        /// <returns><c>true</c> si la clave existía y fue eliminada; de lo contrario, <c>false</c>.</returns>
        public async Task<bool> RemoveAsync(string key)
        {
            return await _cacheDb.KeyDeleteAsync(key);
        }
    }
}
