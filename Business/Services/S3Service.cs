using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace APITemplate.Bussines.Services
{
    public class S3Service
    {
        private readonly Cloudinary _cloudinary;
        private readonly IDatabase? _redisDb;

        public S3Service(IConfiguration configuration, IConnectionMultiplexer? redis = null)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary no está configurado. Verificá Cloudinary:CloudName, Cloudinary:ApiKey y Cloudinary:ApiSecret.");
            }

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            _cloudinary.Api.Secure = true;

            _redisDb = redis?.GetDatabase();
        }

        public async Task<string?> SubirFotosAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var extension = Path.GetExtension(fileName);
                var publicId = string.IsNullOrWhiteSpace(extension)
                    ? fileName
                    : fileName[..^extension.Length];

                var uploadRequest = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    PublicId = publicId,
                    UseFilename = false,
                    UniqueFilename = false,
                    Overwrite = true,
                    Invalidate = true
                };

                var response = await _cloudinary.UploadAsync(uploadRequest);
                if (response.StatusCode is not System.Net.HttpStatusCode.OK ||
                    string.IsNullOrWhiteSpace(response.SecureUrl?.ToString()))
                {
                    return null;
                }

                return response.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Cloudinary al subir {fileName}: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ObtenerUrlPublicaAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            if (Uri.TryCreate(key, UriKind.Absolute, out _))
                return key;

            var cacheKey = $"img:url:{key}";

            try
            {
                if (_redisDb != null)
                {
                    var cached = await _redisDb.StringGetAsync(cacheKey);
                    if (!string.IsNullOrWhiteSpace(cached))
                        return cached!;
                }

                var url = _cloudinary.Api.UrlImgUp.Secure(true).BuildUrl(key);

                if (_redisDb != null && !string.IsNullOrWhiteSpace(url))
                    await _redisDb.StringSetAsync(cacheKey, url, TimeSpan.FromHours(1));

                return url;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando URL de Cloudinary para {key}: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<bool> EliminarFotoAsync(string key)
        {
            try
            {
                var publicId = ExtraerPublicId(key);
                if (string.IsNullOrWhiteSpace(publicId))
                    return false;

                var response = await _cloudinary.DestroyAsync(new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image,
                    Type = "upload",
                    Invalidate = true
                });

                if (!string.Equals(response.Result, "ok", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(response.Result, "not found", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (_redisDb != null)
                    await _redisDb.KeyDeleteAsync($"img:url:{publicId}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando en Cloudinary {key}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarCarpetaFotosAsync(string carpetaKey)
        {
            // Compatibilidad con código legado: la eliminación real se hace por foto.
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ProbarConexionAsync()
        {
            try
            {
                var testPublicId = $"healthcheck/{Guid.NewGuid()}";
                var testContent = "Conexión exitosa con Cloudinary desde API .NET";
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testContent));

                var upload = await _cloudinary.UploadAsync(new RawUploadParams
                {
                    File = new FileDescription("healthcheck.txt", stream),
                    PublicId = testPublicId,
                    UseFilename = false,
                    UniqueFilename = false,
                    Overwrite = true
                });

                if (upload.StatusCode is not System.Net.HttpStatusCode.OK)
                    return false;

                await _cloudinary.DestroyAsync(new DeletionParams(testPublicId)
                {
                    ResourceType = ResourceType.Raw,
                    Type = "upload",
                    Invalidate = true
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Cloudinary: {ex.Message}");
                return false;
            }
        }

        private static string ExtraerPublicId(string keyOrUrl)
        {
            if (string.IsNullOrWhiteSpace(keyOrUrl))
                return string.Empty;

            if (!Uri.TryCreate(keyOrUrl, UriKind.Absolute, out var uri))
                return QuitarExtension(keyOrUrl.TrimStart('/'));

            var path = uri.AbsolutePath.TrimStart('/');
            var uploadIndex = path.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
            if (uploadIndex >= 0)
                path = path[(uploadIndex + "/upload/".Length)..];

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (segments.Count > 0 && Regex.IsMatch(segments[0], @"^v\d+$", RegexOptions.IgnoreCase))
                segments.RemoveAt(0);

            if (segments.Count == 0)
                return string.Empty;

            return QuitarExtension(string.Join('/', segments));
        }

        private static string QuitarExtension(string input)
        {
            var extension = Path.GetExtension(input);
            return string.IsNullOrWhiteSpace(extension) ? input : input[..^extension.Length];
        }
    }
}
