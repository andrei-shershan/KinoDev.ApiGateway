namespace KinoDev.ApiGateway.Infrastructure.Constants
{
    public class DomainApiEndpoints
    {
        public class Movies
        {
            public const string GetMovies = "api/movies";

            public const string GetShowingMovies = "api/movies/showing";
        }

        public class ShowTimes
        {
            public const string GetShowTimeDetails = "api/showtimes/details";

            public const string GetShowTimeSeats = "api/showtimes/seats";
        }

        public class Orders
        {
            public static string CompleteOrder(Guid id) => $"api/orders/{id}/complete"; 
            public const string CreateOrder = "api/orders";
            public const string GetOrderSummary = "api/orders/summary";
            public const string GetOrder = "api/orders";

            public const string GetCompletedOrders = "api/orders/completed";

            public static string UpdateOrderEmail(Guid id) => $"api/orders/{id}/email";

            public static string DeleteActiveOrder(Guid id) => $"api/orders/{id}";
        }
    }
}
