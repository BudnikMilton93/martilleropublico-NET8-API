using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using StackExchange.Redis;

namespace APITemplate.Bussines.Services
{
    public class S3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;
        private readonly IDatabase? _redisDb;

        public S3Service(IConfiguration configuration, IConnectionMultiplexer? redis = null)
        {
            var awsAccessKey = configuration["AWS:AccessKey"];
            var awsSecretKey = configuration["AWS:SecretKey"];
            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
            _bucketName = configuration["AWS:BucketName"];

            _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, region);
            
            // Si Redis está disponible, obtenemos una instancia de base
            _redisDb = redis?.GetDatabase();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<string?> SubirFotosAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fileStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.Private // mantener las fotos privadas (puedes usar CloudFront más adelante)
                };

                await fileTransferUtility.UploadAsync(uploadRequest);

                // Verificación rápida del objeto subido (opcional, asegura éxito real)
                var metadata = await _s3Client.GetObjectMetadataAsync(_bucketName, fileName);
                if (metadata.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                // Retornamos la ruta final del archivo
                return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error AWS S3 al subir {fileName}: {ex.Message}");
                return null; // garantiza que el servicio de fotos no guarde en DB
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al subir {fileName}: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Obtiene la URL pública o pre-firmada de un archivo almacenado en Amazon S3,
        /// utilizando Redis como caché para evitar consultas repetidas.
        /// </summary>
        /// <param name="key">
        /// Nombre de archivo (clave) dentro del bucket de S3. 
        /// Generalmente coincide con la ruta relativa del archivo subido.
        /// </param>
        /// <returns>
        /// Devuelve la URL de acceso al archivo:
        /// <list type="bullet">
        /// <item>
        /// <description>Si el bucket o el objeto son públicos, devuelve la URL pública directa.</description>
        /// </item>
        /// <item>
        /// <description>Si el objeto es privado, genera y devuelve una URL pre-firmada válida por 1 hora.</description>
        /// </item>
        /// <item>
        /// <description>Si el archivo no existe o ocurre un error, devuelve una cadena vacía.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Este método realiza una verificación inicial con <see cref="GetObjectMetadataAsync"/>.
        /// Si el objeto no tiene acceso público, genera una URL pre-firmada usando <see cref="GetPreSignedUrlRequest"/>.
        /// </remarks>
        public async Task<string> ObtenerUrlPublicaAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return string.Empty;

                string cacheKey = $"s3:url:{key}";

                // Intentar obtener desde Redis primero
                if (_redisDb != null)
                {
                    var cachedUrl = await _redisDb.StringGetAsync(cacheKey);
                    if (!cachedUrl.IsNullOrEmpty)
                        return cachedUrl!;
                }

                // Verificar si el objeto existe
                var metadataResponse = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = key
                });

                string url;

                if (metadataResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Si el bucket es público
                    url = $"https://{_bucketName}.s3.amazonaws.com/{key}";
                }
                else
                {
                    // Si el objeto es privado, generamos una URL pre-firmada (1 hora)
                    var preSignedRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        Expires = DateTime.UtcNow.AddHours(1)
                    };
                    url = _s3Client.GetPreSignedURL(preSignedRequest);
                }

                // Guardar en Redis con expiración (1 hora)
                if (_redisDb != null && !string.IsNullOrEmpty(url))
                {
                    await _redisDb.StringSetAsync(cacheKey, url, TimeSpan.FromHours(1));
                }

                return url;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error al obtener URL pública/pre-firmada de {key}: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al obtener URL de S3: {ex.Message}");
                return string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> EliminarFotoAsync(string key)
        {
            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                });

                // También podrías eliminar del cache Redis
                if (_redisDb != null)
                    await _redisDb.KeyDeleteAsync($"s3:url:{key}");

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error AWS al eliminar {key}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar {key}: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ProbarConexionAsync()
        {
            try
            {
                // Creamos un archivo temporal en memoria
                var testKey = "test-connection.txt";
                var testContent = "Conexión exitosa con S3 desde API .NET";
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testContent));

                // Subimos el archivo al bucket
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = testKey,
                    InputStream = stream,
                    ContentType = "text/plain"
                };

                await _s3Client.PutObjectAsync(putRequest);

                // Verificamos que el archivo exista
                var response = await _s3Client.GetObjectMetadataAsync(_bucketName, testKey);
                bool existe = response.HttpStatusCode == System.Net.HttpStatusCode.OK;

                // Eliminamos el archivo de prueba
                await _s3Client.DeleteObjectAsync(_bucketName, testKey);

                return existe;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error AWS: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }
    }
}
