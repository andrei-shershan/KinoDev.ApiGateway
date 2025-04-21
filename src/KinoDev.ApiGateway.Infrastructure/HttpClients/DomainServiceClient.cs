using System.Text;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using KinoDev.Shared.DtoModels;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.ShowingMovies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public interface IDomainServiceClient
    {
        Task<IEnumerable<MovieDto>> GetMoviesAsync();

        Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date);

        Task<ShowTimeDetailsDto> GetShowTimeDetailsAsync(int showTimeId);

        Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int showTimeId);

        Task<OrderSummary> CreateOrderAsync(CreateOrderDto createOrderDto);

        Task<OrderDto> GetOrderAsync(Guid orderId);

        Task<OrderSummary> GetOrderSummaryAsync(Guid orderId);

        Task<string> TestCall();

        Task<OrderDto> CompleteOrderAsync(Guid orderId);

        Task<bool> DeleteActiveOrder(Guid orderId);
    }

    public class CreateOrderDto
    {
        public int ShowTimeId { get; set; }

        public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();
    }

    public class DomainServiceClient : IDomainServiceClient
    {
        private readonly HttpClient _httpClient;

        public DomainServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderDto> CompleteOrderAsync(Guid orderId)
        {
            var requestUri = DomainApiEndpoints.Orders.CompleteOrder(orderId);

            var response = await _httpClient.PostAsync(requestUri, null);

            return await response.GetResponseAsync<OrderDto>();
        }

        public async Task<OrderSummary> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(createOrderDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(DomainApiEndpoints.Orders.CreateOrder, requestContent);

            return await response.GetResponseAsync<OrderSummary>();
        }

        public async Task<OrderSummary> GetOrderSummaryAsync(Guid orderId)
        {
            var requestUri = $"{DomainApiEndpoints.Orders.GetOrderSummary}/{orderId}";
            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<OrderSummary>();
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync(DomainApiEndpoints.Movies.GetMovies);

            return await response.GetResponseAsync<IEnumerable<MovieDto>>();
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId)
        {
            var requestUri = $"{DomainApiEndpoints.Orders.GetOrder}/{orderId}";
            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<OrderDto>();
        }

        public async Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date)
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "date", date.ToString() },
            };

            var requestUri = QueryHelpers.AddQueryString(DomainApiEndpoints.Movies.GetShowingMovies, queryParams);

            var response = await _httpClient.GetAsync($"{DomainApiEndpoints.Movies.GetShowingMovies}?date={date}");

            return await response.GetResponseAsync<IEnumerable<ShowingMovie>>();
        }

        public async Task<ShowTimeDetailsDto> GetShowTimeDetailsAsync(int showTimeId)
        {
            var response = await _httpClient.GetAsync($"{DomainApiEndpoints.ShowTimes.GetShowTimeDetails}/{showTimeId}");

            return await response.GetResponseAsync<ShowTimeDetailsDto>();
        }

        public async Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int showTimeId)
        {
            var response = await _httpClient.GetAsync($"{DomainApiEndpoints.ShowTimes.GetShowTimeSeats}/{showTimeId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.GetResponseAsync<ShowTimeSeatsDto>();
            }

            return null;
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

        public async Task<bool> DeleteActiveOrder(Guid orderId)
        {
            var requestUri = DomainApiEndpoints.Orders.DeleteActiveOrder(orderId);
            var response = await _httpClient.DeleteAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<bool>(result);
            }
            else
            {
                return false;
            }
        }
    }
}
