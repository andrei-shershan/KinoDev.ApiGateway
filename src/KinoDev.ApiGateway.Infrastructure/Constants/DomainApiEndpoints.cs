﻿namespace KinoDev.ApiGateway.Infrastructure.Constants
{
    public class DomainApiEndpoints
    {
        public class Movies
        {
            public const string GetMovies = "api/movies";

            public const string CreateMovies = "api/movies";

            public static string GetMovieById(int id) => $"api/movies/{id}";

            public const string GetShowingMovies = "api/movies/showing";

            public const string test = "api/movies/test";

            public const string test2 = "api/movies/test2";
        }

        public class ShowTimes
        {
            public const string GetShowTimes = "api/showtimes";

            public const string GetShowTimeDetails = "api/showtimes/details";

            public const string GetShowTimeSeats = "api/showtimes/seats";
        }

        public class Slots
        {
            public const string GetSlots = "api/slots";
        }

        public class Orders
        {
            public const string GetOrder = "api/orders";

            public const string CreateOrder = "api/orders";

            public static string CompleteOrder(Guid id) => $"api/orders/{id}/complete";

            public const string GetCompletedOrdersByEmail = $"api/orders/completed/email";
            
            public const string GetOrderSummary = "api/orders/summary";

            public const string GetCompletedOrders = "api/orders/completed";

            public static string UpdateOrderEmail(Guid id) => $"api/orders/{id}/email";

            public static string DeleteActiveOrder(Guid id) => $"api/orders/{id}";
        }

        public class Halls
        {
            public const string ApiHalls = "api/halls";
        }
    }
}
