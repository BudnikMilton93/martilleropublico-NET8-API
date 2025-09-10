using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor que recibe inyección de dependencias para configuración y contexto de base de datos
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }


        /// <summary>
        /// Autentica a un usuario mediante email y contraseña.
        /// Verifica que el usuario exista, que la contraseña sea correcta y que el usuario esté activo.
        /// En caso de éxito, genera y retorna un token JWT para autenticación.
        /// </summary>
        /// <param name="request">Objeto que contiene el email y la contraseña del usuario.</param>
        /// <returns>
        /// Respuesta HTTP con el token JWT en caso de autenticación exitosa,
        /// o un código Unauthorized con mensaje si falla la validación.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest request)
        {
            // Buscar en la base de datos el usuario que coincide con el email recibido
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Usuario no encontrado");

            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            if (!user.IsActive)
                return Unauthorized("Usuario inactivo");

            // Generar el token JWT para el usuario autenticado
            var token = GenerateJwtToken(user);

            // Devolver el token al cliente
            return Ok(new { token });
        }


        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">
        /// Objeto <see cref="User"/> que contiene:
        /// - Username (nombre de usuario)
        /// - Email (correo electrónico)
        /// - FullName (nombre completo)
        /// - PasswordHash (contraseña en texto plano para ser hasheada)
        /// </param>
        /// <returns>
        /// Retorna un código 200 (OK) si el usuario fue registrado exitosamente.
        /// Retorna 400 (BadRequest) si los datos enviados no son válidos.
        /// Retorna 409 (Conflict) si ya existe un usuario con el mismo email.
        /// </returns>
        /// <remarks>
        /// La contraseña recibida se encripta antes de guardarse en la base de datos.
        /// La fecha de creación se almacena en formato UTC.
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar si ya existe username o email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return Conflict(new { message = "El usuario ya existe." });


            // Hashear la contraseña
            string hashedPassword = PasswordHasher.HashPassword(request.PasswordHash);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Id_Rol = request.Id_Rol
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente" });
        }


        [HttpGet("perfil")]
        [Authorize] //Esto hace que requiera un JWT válido
        public IActionResult GetProfile()
        {
            return Ok(new
            {
                message = "Acceso concedido. Este es un recurso protegido.",
                user = User.Identity?.Name
            });
        }


        /// <summary>
        /// Genera el token JWT con las configuraciones y claims necesarios
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(User user)
        {
            // Leer la sección de configuración JwtSettings desde appsettings.json
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"]));

            // Crear la clave simétrica usando el secreto para firmar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Definir las credenciales de firma usando algoritmo HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definir los claims que se incluyen dentro del token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Username)
            };

            // Crear el token JWT con todos los parámetros definidos
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            // Serializar el token a string para enviarlo al cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
