using APITemplate.Business.DTOs.Auth;
using APITemplate.Data.Interfaces;
using APITemplate.Helpers;
using APITemplate.Models;
using APITemplate.Services.Interfaces;

namespace APITemplate.Business.Services
{
    /// <summary>
    /// Servicio de autenticación que orquesta la lógica de negocio para login, registro y gestión de tokens
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUsuarioRepository usuarioRepository, IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDTO?> LoginAsync(string email, string password)
        {
            // 1. Buscar usuario por email
            var user = await _usuarioRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            // 2. Verificar contraseña
            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            // 3. Verificar que esté activo
            if (!user.IsActive)
                return null;

            // 4. Generar tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var (refreshToken, refreshExpiryTime) = _jwtService.GenerateRefreshToken();

            // 5. Actualizar refresh token en BD
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpiryTime;
            await _usuarioRepository.UpdateAsync(user);

            // 6. Calcular fecha de expiración del access token
            var jwtSettings = _jwtService.GetJwtSettings();
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(jwtSettings["AccessTokenMinutes"] ?? "60")
            );

            // 7. Construir DTO de respuesta
            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshExpiryTime,
                User = new UserBasicInfoDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    IdRol = user.Id_Rol
                }
            };
        }

        public async Task<int?> RegisterAsync(RegisterRequestDTO request)
        {
            // 1. Verificar si ya existe el email
            if (await _usuarioRepository.EmailExistsAsync(request.Email))
                return null;

            // 2. Verificar si ya existe el username
            if (await _usuarioRepository.UsernameExistsAsync(request.Username))
                return null;

            // 3. Hashear contraseña
            var hashedPassword = PasswordHasher.HashPassword(request.Password);

            // 4. Crear entidad
            var user = new USUARIOS
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Id_Rol = request.IdRol,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 5. Guardar en BD
            var createdUser = await _usuarioRepository.CreateAsync(user);
            return createdUser.Id;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            var user = await _usuarioRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
                return null;

            return new UserProfileDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                IdRol = user.Id_Rol,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }

        public async Task<TokensDTO?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            // 1. Extraer userId del access token (aunque esté expirado)
            var userId = _jwtService.GetUserIdFromToken(accessToken);
            if (userId == null)
                return null;

            // 2. Buscar usuario
            var user = await _usuarioRepository.GetByIdAsync(userId.Value);
            if (user == null || !user.IsActive)
                return null;

            // 3. Verificar que el refresh token coincida y no esté expirado
            if (user.RefreshToken != refreshToken)
                return null;

            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            // 4. Generar nuevos tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var (newRefreshToken, newRefreshExpiryTime) = _jwtService.GenerateRefreshToken();

            // 5. Actualizar refresh token en BD
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newRefreshExpiryTime;
            await _usuarioRepository.UpdateAsync(user);

            // 6. Calcular expiración del access token
            var jwtSettings = _jwtService.GetJwtSettings();
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(jwtSettings["AccessTokenMinutes"] ?? "60")
            );

            return new TokensDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = newRefreshExpiryTime
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(int userId)
        {
            var user = await _usuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Limpiar refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _usuarioRepository.UpdateAsync(user);

            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _usuarioRepository.EmailExistsAsync(email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _usuarioRepository.UsernameExistsAsync(username);
        }
       
    }
}