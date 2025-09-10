namespace ECommerceAPI.Services
{
    /// <summary>
    /// Define las operaciones básicas para interactuar con un sistema de caché,
    /// permitiendo guardar, recuperar y eliminar datos de forma asíncrona.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Almacena un valor en la caché bajo una clave específica.
        /// </summary>
        /// <typeparam name="T">Tipo del valor a almacenar.</typeparam>
        /// <param name="key">Clave única que identifica el valor en la caché.</param>
        /// <param name="value">Valor a almacenar en la caché.</param>
        /// <param name="expiration">Tiempo opcional de expiración del valor en la caché.</param>
        /// <returns>
        /// <c>true</c> si el valor se almacenó correctamente; de lo contrario, <c>false</c>.
        /// </returns>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);


        /// <summary>
        /// Recupera un valor desde la caché utilizando la clave especificada.
        /// </summary>
        /// <typeparam name="T">Tipo del valor esperado.</typeparam>
        /// <param name="key">Clave única asociada al valor en la caché.</param>
        /// <returns>
        /// El valor deserializado si existe; de lo contrario, el valor por defecto para <typeparamref name="T"/>.
        /// </returns>
        Task<T?> GetAsync<T>(string key);


        /// <summary>
        /// Elimina un valor de la caché utilizando la clave especificada.
        /// </summary>
        /// <param name="key">Clave del valor a eliminar.</param>
        /// <returns>
        /// <c>true</c> si la clave existía y fue eliminada; de lo contrario, <c>false</c>.
        /// </returns>
        Task<bool> RemoveAsync(string key);
    }
}
