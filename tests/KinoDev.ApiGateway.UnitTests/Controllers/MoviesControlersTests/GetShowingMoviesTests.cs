using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class GetShowingMoviesTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task GetShowingMoviesAsync_ReturnsOk_WhenMoviesExist()
        {
            // Arrange
            var date = DateTime.Now;
            var movies = new List<ShowingMovie>()
            {
                new ShowingMovie()
                {
                    Id = 1,
                    Name = "Movie 1",
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetShowingMoviesQuery>(), default))
                         .ReturnsAsync(movies);

            // Act
            var result = await _controller.GetShowingMoviesAsync(date);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(movies, okResult.Value);
        }

        [Theory]
        [MemberData(nameof(ShowingMoviesNullOrEmptyData))]
        public async Task GetShowingMoviesAsync_ReturnsNotFound_WhenNoMoviesExist(List<ShowingMovie> showingMovies)
        {
            // Arrange
            var date = DateTime.Now;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetShowingMoviesQuery>(), default))
                         .ReturnsAsync(showingMovies);

            // Act
            var result = await _controller.GetShowingMoviesAsync(date);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        public static IEnumerable<object[]> ShowingMoviesNullOrEmptyData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { new List<ShowingMovie> { } }
            };
    }
}
