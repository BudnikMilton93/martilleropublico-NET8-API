using APITemplate.Data;
using APITemplate.Helpers;
using APITemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APITemplate.Data.Interfaces;

namespace APITemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsuarioRepository _usuarioRepository;

        /// <summary>
        /// Constructor que recibe inyección de dependencias para configuración y contexto de base de datos
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        public AuthController(IConfiguration config, AppDbContext context, IUsuarioRepository usuarioRepository)
        {
            _config = config;
            _usuarioRepository = usuarioRepository; // Inyectamos el repositorio
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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _usuarioRepository.GetByEmailAsync(request.Email);


                if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Credenciales incorrectas" });

                if (!user.IsActive)
                    return Unauthorized(new { message = "Usuario inactivo" });

                var token = GenerateJwtToken(user);
                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                // Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }


        /// <summary>
        /// Devuelve el usuario logueado
        /// </summary>
        /// <returns></returns>
        [HttpGet("perfil")]
        [Authorize] //Esto hace que requiera un JWT válido
        public async Task<IActionResult> GetProfileAsync()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                    return Unauthorized(new { message = "Token inválido" });

                // Usamos el método del repositorio
                var user = await _usuarioRepository.GetByIdAsync(userIdInt);

                if (user == null || !user.IsActive)
                    return NotFound(new { message = "Usuario no encontrado" });

                // Retornamos solo los datos necesarios (sin password)
                return Ok(new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    nombre = user.Nombre,
                    apellido = user.Apellido,
                    idRol = user.Id_Rol,
                    createdAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }

        }


        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">
        /// Objeto <see cref="USUARIOS"/> que contiene:
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
        public async Task<IActionResult> Register([FromBody] USUARIOS request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar si ya existe username o email
            if (await _usuarioRepository.EmailExistsAsync(request.Email))
                return Conflict(new { message = "Ya existe un usuario con este email." });

            if (await _usuarioRepository.UsernameExistsAsync(request.Username))
                return Conflict(new { message = "Ya existe un usuario con este username." });


            // Hashear la contraseña
            string hashedPassword = PasswordHasher.HashPassword(request.PasswordHash);

            var user = new USUARIOS
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

            var createdUser = await _usuarioRepository.CreateAsync(user);
            
            return Ok(new
            {
                message = "Usuario registrado correctamente",
                userId = createdUser.Id
            });
        }


        /// <summary>
        /// Genera el token JWT con las configuraciones y claims necesarios
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(USUARIOS user)
        {
            // Leer la sección de configuración JwtSettings desde appsettings.json
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT SecretKey no configurada");

            // Crear la clave simétrica usando el secreto para firmar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Definir las credenciales de firma usando algoritmo HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definir los claims que se incluyen dentro del token
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.Username),
                    new Claim("role", user.Id_Rol.ToString() ?? "user"),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var expires = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(jwtSettings["ExpiresInMinutes"] ?? "60"));

            // Crear el token JWT con todos los parámetros definidos
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            // Serializar el token a string para enviarlo al cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
