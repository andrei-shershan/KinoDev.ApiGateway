using KinoDev.ApiGateway.Infrastructure.Models.RequestModels;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.ShowingMovies;
using KinoDev.Shared.DtoModels.ShowTimes;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

// TODO: Split into multiple clients if it grows too large
public interface IDomainServiceClient
{
    Task<IEnumerable<MovieDto>> GetMoviesAsync();

    Task<MovieDto> GetMovieByIdAsync(int id);

    Task<MovieDto> CreateMovieAsync(MovieDto movieDto);

    Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date);

    Task<IEnumerable<ShowTimeDetailsDto>> GetShowTimeDetailsAsync(DateTime startDate, DateTime endDate);

    Task<ShowTimeDetailsDto> GetShowTimeDetailsAsync(int showTimeId);

    Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int showTimeId);

    Task<OrderSummary> CreateOrderAsync(CreateOrderDto createOrderDto);

    Task<OrderDto> GetOrderAsync(Guid orderId);

    Task<OrderSummary> GetOrderSummaryAsync(Guid orderId);

    Task<OrderDto> CompleteOrderAsync(Guid orderId);

    Task<bool> DeleteActiveOrder(Guid orderId);

    Task<OrderDto> UpdateOrderEmailAsync(Guid orderId, string email);

    Task<IEnumerable<OrderSummary>> GetCompletedOrdersAsync(IEnumerable<Guid> orderIds);

    Task<IEnumerable<OrderSummary>> GetCompletedOrdersByEmail(string email);

    Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount);

    Task<HallSummary> GetHallByIdAsync(int id);

    Task<IEnumerable<HallSummary>> GetHallsAsync();

    Task<ShowTimeForDateDto> GetShowTimeForDateDtoAsync(DateTime date);

    Task<bool> CreateShowTimeAsync(int movieId, int hallId, DateTime time, decimal price);

    Task<bool> Test();

    Task<bool> Test2();
}

