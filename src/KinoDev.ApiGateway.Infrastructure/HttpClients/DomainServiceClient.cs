namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public interface IDomainServiceClient
    {
        public Task<string> TestCall();
    }

    public class DomainServiceClient : IDomainServiceClient
    {
        private readonly HttpClient _httpClient;

        public DomainServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> TestCall()
        {
            var response = await _httpClient.GetAsync("api/test/auth");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
