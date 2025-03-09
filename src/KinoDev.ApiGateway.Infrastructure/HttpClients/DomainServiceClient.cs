using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using KinoDev.Shared.DtoModels;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Microsoft.AspNetCore.WebUtilities;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public interface IDomainServiceClient
    {
        Task<IEnumerable<MovieDto>> GetMoviesAsync();

        Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date);

        Task<string> TestCall();
    }

    public class DomainServiceClient : IDomainServiceClient
    {
        private readonly HttpClient _httpClient;

        public DomainServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync(DomainApiEndpoints.Movies.GetMovies);

            return await response.GetResponseAsync<IEnumerable<MovieDto>>();
        }

        public async Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date)
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "date", date.ToString() },
            };

            var requestUri = QueryHelpers.AddQueryString(DomainApiEndpoints.Movies.GetShowingMovies, queryParams);

            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<IEnumerable<ShowingMovie>>();
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
