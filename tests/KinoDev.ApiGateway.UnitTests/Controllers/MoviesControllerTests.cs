using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.ApiGateway.WebApi.Controllers;
using KinoDev.Shared.DtoModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers
{
    public class MoviesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new MoviesController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetMoviesAsync_ReturnsOkResult_WhenMoviesExist()
        {
            // Arrange
            var movies = new List<MovieDto> { new MovieDto { Id = 1, Name = "Movie 1" }, new MovieDto { Id = 2, Name = "Movie 2" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMoviesQuery>(), default)).ReturnsAsync(movies);

            // Act
            var result = await _controller.GetMoviesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<MovieDto>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);

            Assert.Contains(returnValue, x => x.Id == 1 && x.Name == "Movie 1");
            Assert.Contains(returnValue, x => x.Id == 2 && x.Name == "Movie 2");
        }

        [Theory]
        [MemberData(nameof(MoviesNullOrEmptyData))]
        public async Task GetMoviesAsync_ReturnsNotFoundResult_WhenNoMoviesExist(List<MovieDto> movies)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMoviesQuery>(), default)).ReturnsAsync(movies);

            // Act
            var result = await _controller.GetMoviesAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        public static IEnumerable<object[]> MoviesNullOrEmptyData =>
            new List<object[]>
            {
                            new object[] { null },
                            new object[] { new List<MovieDto> { } }
            };
    }
}
