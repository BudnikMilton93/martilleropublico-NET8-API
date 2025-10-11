using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APITemplate.Business.Services
{
    /// <summary>
    /// Servicio responsable de la generación de tokens JWT para la autenticación y autorización de usuarios.
    /// Utiliza la configuración de <c>JwtSettings</c> definida en <c>appsettings.json</c> para obtener
    /// parámetros como clave secreta, emisor, audiencia y tiempo de expiración.
    /// </summary>
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor de la clase <c>AuthService</c>.
        /// Recibe la configuración de la aplicación para acceder a los valores definidos en <c>JwtSettings</c>.
        /// </summary>
        /// <param name="configuration">
        /// Objeto de configuración (<see cref="IConfiguration"/>) usado para leer parámetros como
        /// la clave secreta, emisor, audiencia y expiración del token.
        /// </param>
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Genera un token JWT firmado con algoritmo HMAC-SHA256, que incluye las reclamaciones (claims)
        /// de correo electrónico y rol del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario a incluir en el token.</param>
        /// <param name="role">Rol del usuario a incluir en el token (por defecto "User").</param>
        /// <returns>
        /// Una cadena de texto que representa el token JWT generado.
        /// </returns>
        public string GenerateToken(string email, string role = "User")
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
