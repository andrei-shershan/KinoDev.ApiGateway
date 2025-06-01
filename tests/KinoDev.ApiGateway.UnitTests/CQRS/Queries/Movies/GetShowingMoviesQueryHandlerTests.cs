using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.CQRS.Queries.Movies
{
    public class GetShowingMoviesQueryHandlerTests
    {
        private readonly Mock<IDomainServiceClient> _mockDomainServiceClient = new();
        private readonly GetShowingMoviesQueryHandler _handler;

        public GetShowingMoviesQueryHandlerTests()
        {
            _handler = new GetShowingMoviesQueryHandler(_mockDomainServiceClient.Object);
        }

        [Fact]
        public async Task Handle_ShouldResetTimeToMidnight_AndCallGetShowingMoviesAsync()
        {
            // Arrange
            var request = new GetShowingMoviesQuery { Date = new DateTime(2023, 10, 5, 15, 30, 0) };
            var cancellationToken = new CancellationToken();
            var expectedDate = new DateTime(2023, 10, 5);
            var showingMovies = new List<ShowingMovie> { new ShowingMovie { Id = 1, Name = "Movie 1" } };

            _mockDomainServiceClient
                .Setup(client => client.GetShowingMoviesAsync(expectedDate))
                .ReturnsAsync(showingMovies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(expectedDate, request.Date);
            Assert.Equal(showingMovies, result);
            _mockDomainServiceClient.Verify(client => client.GetShowingMoviesAsync(expectedDate), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenNoShoingMovies()
        {
            // Arrange
            var request = new GetShowingMoviesQuery { Date = new DateTime(2023, 10, 5, 15, 30, 0) };
            var cancellationToken = new CancellationToken();
            var expectedDate = new DateTime(2023, 10, 5);

            _mockDomainServiceClient
                .Setup(client => client.GetShowingMoviesAsync(expectedDate))
                .ReturnsAsync(() => null);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(expectedDate, request.Date);
            Assert.Null(result);
            _mockDomainServiceClient.Verify(client => client.GetShowingMoviesAsync(expectedDate), Times.Once);
        }
    }
}