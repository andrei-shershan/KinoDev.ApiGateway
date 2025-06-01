using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies;
using KinoDev.Shared.DtoModels.Movies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class GetMovieByIdTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task GetMovieByIdAsync_PassesCorrectId_ToMediator()
        {
            // Arrange
            var movieId = 5;
            var movie = new MovieDto { Id = movieId, Name = "Test Movie" };
            
            _mediatorMock.Setup(m => m.Send(It.Is<GetMovieByIdQuery>(q => q.Id == movieId), default))
                         .ReturnsAsync(movie)
                         .Verifiable();

            // Act
            await _controller.GetMovieByIdAsync(movieId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetMovieByIdQuery>(q => q.Id == movieId), default), Times.Once);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ReturnsOk_WhenMovieExists()
        {
            // Arrange
            var movieId = 1;
            var movie = new MovieDto { Id = movieId, Name = "Test Movie" };
            
            _mediatorMock.Setup(m => m.Send(It.Is<GetMovieByIdQuery>(q => q.Id == movieId), default))
                         .ReturnsAsync(movie);

            // Act
            var result = await _controller.GetMovieByIdAsync(movieId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<MovieDto>(okResult.Value);
            
            Assert.Equal(movieId, returnValue.Id);
            Assert.Equal("Test Movie", returnValue.Name);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            // Arrange
            var movieId = 999;
            
            _mediatorMock.Setup(m => m.Send(It.Is<GetMovieByIdQuery>(q => q.Id == movieId), default))
                         .ReturnsAsync((MovieDto)null);

            // Act
            var result = await _controller.GetMovieByIdAsync(movieId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
