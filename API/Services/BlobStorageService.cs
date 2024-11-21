using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace API.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");
            var blobServiceClient = new BlobServiceClient(_connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            _containerClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = await GetUniqueFileNameAsync(file.FileName);
            var blobClient = _containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: false);
            }

            return blobClient.Uri.ToString();
        }

        public async Task<FileDto> GetFileAsync(string fileName)
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
