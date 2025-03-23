using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");
            //var blobServiceClient = new BlobServiceClient(_connectionString);
            //_containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            //_containerClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                var fileName = await GetUniqueFileNameAsync(file.FileName);
                var blobClient = _containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: false);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom uploadovanja file-a {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RemoveFileAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                return await blobClient.DeleteIfExistsAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom brisanja file-a {ex.Message}");
                throw;
            }
        }

        public async Task<FileDto> GetFileAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                {
                    return new FileDto(null, null, null);
                }

                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;

                string mimeType = "application/octet-stream";
                if (fileName.EndsWith(".pdf"))
                {
                    mimeType = "application/pdf";
                }
                return new FileDto(memoryStream.ToArray(), mimeType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom dohvaćanja file-a {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetUniqueFileNameAsync(string originalFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);
            var newFileName = originalFileName;
            int counter = 1;
            var blobClient = _containerClient.GetBlobClient(newFileName);

            while (await blobClient.ExistsAsync())
            {
                newFileName = $"{fileName}({counter++}){extension}";
                blobClient = _containerClient.GetBlobClient(newFileName);
            }

            return newFileName;
        }
    }


    public class FileDto
    {
        public FileDto(byte[] fileContent, string mimeType, string fileName)
        {
            FileContent = fileContent;
            MimeType = mimeType;
            FileName = fileName;
        }

        public byte[] FileContent { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
