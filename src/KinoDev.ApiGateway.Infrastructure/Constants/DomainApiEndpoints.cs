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
            public const string CreateOrder = "api/orders";
        }
    }
}
