using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace APITemplate.Bussines.Services
{
    public class S3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            var awsAccessKey = configuration["AWS:AccessKey"];
            var awsSecretKey = configuration["AWS:SecretKey"];
            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
            _bucketName = configuration["AWS:BucketName"];

            _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, region);
        }

        public async Task<string> SubirFotosAsync(Stream fileStream, string fileName, string contentType)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileName,
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private // Mantener privado para seguridad
            };

            await fileTransferUtility.UploadAsync(uploadRequest);

            // Retornamos la URL pública si querés exponerla vía CloudFront más adelante
            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }

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
