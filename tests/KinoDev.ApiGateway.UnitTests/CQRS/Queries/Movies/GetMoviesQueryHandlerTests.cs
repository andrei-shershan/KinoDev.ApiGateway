using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.Movies;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.CQRS.Queries.Movies
{
    public class GetMoviesQueryHandlerTests
    {
        private readonly Mock<IDomainServiceClient> _mockDomainServiceClient = new();
        private readonly GetMoviesQueryHandler _handler;

        public GetMoviesQueryHandlerTests()
        {
           _handler = new GetMoviesQueryHandler(_mockDomainServiceClient.Object);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenNoMovies()
        {
            // Arrange

            _mockDomainServiceClient.Setup(client => client.GetMoviesAsync())
                .ReturnsAsync(() => null);

            var query = new GetMoviesQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ReturnsListOfMovies()
        {
            // Arrange
            var movies = new List<MovieDto>
            {
                new MovieDto { Id = 1, Name = "Movie 1" },
                new MovieDto { Id = 2, Name = "Movie 2" }
            };

            _mockDomainServiceClient.Setup(client => client.GetMoviesAsync())
                .ReturnsAsync(movies);

            var query = new GetMoviesQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Contains(result, m => m.Id == 1 && m.Name == "Movie 1");
            Assert.Contains(result, m => m.Id == 2 && m.Name == "Movie 2");
        }
    }
}
