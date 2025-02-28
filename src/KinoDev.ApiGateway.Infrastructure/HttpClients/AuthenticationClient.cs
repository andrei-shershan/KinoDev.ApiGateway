using KinoDev.ApiGateway.Infrastructure.Models;
using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public interface IAuthenticationClient
    {
        Task<TokenModel> GetClientTokenAsync();
    }

    public class AuthenticationClient : IAuthenticationClient
    {
        private HttpClient _httpClient;
        private AuthenticationSettings _authenticationSettings;

        public AuthenticationClient(HttpClient httpClient, IOptions<AuthenticationSettings> options)
        {
            _authenticationSettings = options.Value;

            _httpClient = httpClient;
        }

        public async Task<TokenModel> GetClientTokenAsync()
        {
            var data = new
            {
                _authenticationSettings.ClientId,
                _authenticationSettings.ClientSecret
            };

            var bodyParams = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/authentication/client-token", bodyParams);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TokenModel>(content);
            }

            // TODO: Do smth with that!
            throw new Exception();
        }
    }
}
