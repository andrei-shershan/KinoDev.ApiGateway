using System.Text;
using System.Text.Json;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class EmailServiceClient : IEmailServiceClient
    {
        private readonly HttpClient _httpClient;

        public EmailServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            // TODO: Add proper model for email request
            var content = new StringContent(JsonSerializer.Serialize(new { to = email, subject, body }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/emails/send", content);

            return response.IsSuccessStatusCode;
        }

        public async Task Up()
        {
            await _httpClient.GetAsync("up");
        }
    }
}