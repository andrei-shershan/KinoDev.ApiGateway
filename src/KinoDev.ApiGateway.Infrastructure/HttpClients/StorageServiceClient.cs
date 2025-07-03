using System.Text;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.Infrastructure.Models.RequestModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class StorageServiceClient : IStorageServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StorageServiceClient> _logger;

        public StorageServiceClient(
            HttpClient httpClient,
            ILogger<StorageServiceClient> logger
        )
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task Up()
        {
            await _httpClient.GetAsync("up");
        }

        public async Task<string> UploadFileAsync(string fileName, byte[] bytes)
        {
            var requestUri = "api/files";

            var data = new FileUploadRequest
            {
                FileName = fileName,
                Base64Contents = Convert.ToBase64String(bytes)
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(requestUri, requestContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}