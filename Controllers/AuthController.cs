using APITemplate.Business.DTOs.Auth;
using APITemplate.Models;
using APITemplate.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APITemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor que recibe inyección de dependencias del servicio de autenticación
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica a un usuario mediante email y contraseña.
        /// </summary>
        /// <param name="request">Objeto que contiene el email y la contraseña del usuario.</param>
        /// <returns>
        /// Token JWT con refresh token en caso de autenticación exitosa,
        /// o un código Unauthorized si falla la validación.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(request.Email, request.Password);

                if (result == null)
                    return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // TODO: Log del error (implementar logging)
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado
        /// </summary>
        /// <returns>Datos del perfil del usuario sin información sensible</returns>
        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                    return Unauthorized(new { message = "Token inválido" });

                var userProfile = await _authService.GetUserProfileAsync(userIdInt);

                if (userProfile == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                // TODO: Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">DTO con los datos del nuevo usuario</param>
        /// <returns>
        /// Código 200 (OK) si el usuario fue registrado exitosamente.
        /// Código 400 (BadRequest) si los datos no son válidos.
        /// Código 409 (Conflict) si ya existe un usuario con el mismo email o username.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Verificar duplicados antes de intentar registrar
                if (await _authService.EmailExistsAsync(request.Email))
                    return Conflict(new { message = "Ya existe un usuario con este email" });

                if (await _authService.UsernameExistsAsync(request.Username))
                    return Conflict(new { message = "Ya existe un usuario con este username" });

                var userId = await _authService.RegisterAsync(request);

                if (userId == null)
                    return StatusCode(500, new { message = "Error al crear el usuario" });

                return Ok(new
                {
                    message = "Usuario registrado correctamente",
                    userId
                });
            }
            catch (Exception ex)
            {
                // TODO: Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Refresca un access token usando un refresh token válido
        /// </summary>
        /// <param name="request">DTO con access token y refresh token</param>
        /// <returns>Nuevos tokens si el refresh es válido</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

                if (result == null)
                    return Unauthorized(new { message = "Refresh token inválido o expirado" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // TODO: Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cierra sesión revocando el refresh token del usuario
        /// </summary>
        /// <returns>Confirmación de logout</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                    return Unauthorized(new { message = "Token inválido" });

                var result = await _authService.RevokeRefreshTokenAsync(userIdInt);

                if (!result)
                    return StatusCode(500, new { message = "Error al cerrar sesión" });

                return Ok(new { message = "Sesión cerrada correctamente" });
            }
            catch (Exception ex)
            {
                // TODO: Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}