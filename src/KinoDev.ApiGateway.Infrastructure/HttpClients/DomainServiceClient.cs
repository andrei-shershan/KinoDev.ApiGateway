using System.Text;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.Infrastructure.Models.RequestModels;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.ShowingMovies;
using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class DomainServiceClient : IDomainServiceClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<DomainServiceClient> _logger;

        public DomainServiceClient(HttpClient httpClient,
        ILogger<DomainServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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

            return await response.GetResponseAsync<ShowTimeSeatsDto>();
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

        public async Task<OrderDto> UpdateOrderEmailAsync(Guid orderId, string email)
        {
            var requestUri = DomainApiEndpoints.Orders.UpdateOrderEmail(orderId);
            var requestContent = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(requestUri, requestContent);

            return await response.GetResponseAsync<OrderDto>();
        }

        public async Task<IEnumerable<OrderSummary>> GetCompletedOrdersAsync(IEnumerable<Guid> orderIds)
        {
            var requestUri = DomainApiEndpoints.Orders.GetCompletedOrders;
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { orderIds }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, requestContent);

            return await response.GetResponseAsync<IEnumerable<OrderSummary>>();
        }

        public async Task<IEnumerable<OrderSummary>> GetCompletedOrdersByEmail(string email)
        {
            var requestUri = DomainApiEndpoints.Orders.GetCompletedOrdersByEmail;
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { email }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, requestContent);

            return await response.GetResponseAsync<IEnumerable<OrderSummary>>();
        }

        public async Task<MovieDto> GetMovieByIdAsync(int id)
        {
            var requestUri = $"{DomainApiEndpoints.Movies.GetMovieById(id)}";
            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<MovieDto>();
        }

        public async Task<MovieDto> CreateMovieAsync(MovieDto movieDto)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(movieDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(DomainApiEndpoints.Movies.CreateMovies, requestContent);

            return await response.GetResponseAsync<MovieDto>();
        }

        public async Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount)
        {
            var data = new
            {
                Name = hallName,
                RowsCount = rowsCount,
                SeatsCount = seatsCount
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(DomainApiEndpoints.Halls.ApiHalls, requestContent);

            return await response.GetResponseAsync<HallSummary>();
        }

        public async Task<HallSummary> GetHallByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{DomainApiEndpoints.Halls.ApiHalls}/{id}");

            return await response.GetResponseAsync<HallSummary>();
        }

        public async Task<IEnumerable<HallSummary>> GetHallsAsync()
        {
            var response = await _httpClient.GetAsync(DomainApiEndpoints.Halls.ApiHalls);
            return await response.GetResponseAsync<IEnumerable<HallSummary>>();
        }

        public async Task<IEnumerable<ShowTimeDetailsDto>> GetShowTimeDetailsAsync(DateTime startDate, DateTime endDate)
        {
            var requestUri = $"{DomainApiEndpoints.ShowTimes.GetShowTimes}/{startDate:yyyy-MM-ddTHH:mm:ss}/{endDate:yyyy-MM-ddTHH:mm:ss}";

            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<IEnumerable<ShowTimeDetailsDto>>();
        }

        public async Task<ShowTimeForDateDto> GetShowTimeForDateDtoAsync(DateTime date)
        {
            var requestUri = $"{DomainApiEndpoints.Slots.GetSlots}/{date:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(requestUri);

            return await response.GetResponseAsync<ShowTimeForDateDto>();
        }

        public async Task<bool> CreateShowTimeAsync(int movieId, int hallId, DateTime time, decimal price)
        {
            var requestUri = DomainApiEndpoints.ShowTimes.GetShowTimes;
            var requestContent = new StringContent(JsonConvert.SerializeObject(new
            {
                MovieId = movieId,
                HallId = hallId,
                Time = time,
                Price = price
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Test()
        {
            var response = await _httpClient.GetAsync(DomainApiEndpoints.Movies.test);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Content from test endpoint: {content}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Test2()
        {
            var response = await _httpClient.GetAsync(DomainApiEndpoints.Movies.test2);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Content from test endpoint: {content}");

            return response.IsSuccessStatusCode;
        }
    }
}
