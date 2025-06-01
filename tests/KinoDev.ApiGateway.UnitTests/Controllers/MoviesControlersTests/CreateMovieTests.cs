using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Movies;
using KinoDev.Shared.DtoModels.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class CreateMovieTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task CreateMovieAsync_ReturnsCreatedResult_WhenMovieCreatedSuccessfully()
        {
            // Arrange
            var createdMovie = new MovieDto { Id = 1, Name = "New Movie" };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMovieCommand>(), default))
                         .ReturnsAsync(createdMovie);
            
            // Create a mock for HttpRequest to set up the Form property
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.Request.Form = formCollection;

            // Act
            var result = await _controller.CreateMovieAsync();

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<MovieDto>(createdAtActionResult.Value);
            
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("New Movie", returnValue.Name);
            
            // Verify that the command was sent with the correct form data
            _mediatorMock.Verify(m => m.Send(It.Is<CreateMovieCommand>(c => c.Form == formCollection), default), Times.Once);
        }        [Fact]
        public async Task CreateMovieAsync_ReturnsBadRequest_WhenMovieCreationFails()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMovieCommand>(), default))
                         .ReturnsAsync((MovieDto)null);
            
            // Create a mock for HttpRequest to set up the Form property
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.Request.Form = formCollection;

            // Act
            var result = await _controller.CreateMovieAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to create movie.", badRequestResult.Value);
        }        [Fact]
        public async Task CreateMovieAsync_HandlesMediatrException()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMovieCommand>(), default))
                         .ThrowsAsync(new Exception("Test exception"));
            
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.Request.Form = formCollection;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateMovieAsync());
            
            // We can't easily verify logger usage without additional setup for logging
        }
    }
}
