using Microsoft.Extensions.Configuration;

namespace ECommerceAPI.Helpers
{
    public static class DbConnectionHelper
    {
        public static string GetDefaultConnectionString(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new Exception("La cadena de conexión 'DefaultConnection' no está configurada.");
            }
            return connection;
        }
    }
}
