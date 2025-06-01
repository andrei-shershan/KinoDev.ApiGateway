using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Halls;
using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class CreateHallAsyncTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task CreateHallAsync_ReturnsCreatedResult_WhenHallCreatedSuccessfully()
        {
            // Arrange
            var createHallCommand = new CreateHallCommand
            {
                Name = "New Hall",
                RowsCount = 10,
                SeatsCount = 20
            };
            var createdHall = new HallDto
            {
                Id = 1,
                Name = "New Hall"
            };

            _mediatorMock.Setup(m => m.Send(It.Is<CreateHallCommand>(c =>
                c.Name == createHallCommand.Name &&
                c.RowsCount == createHallCommand.RowsCount &&
                c.SeatsCount == createHallCommand.SeatsCount), default))
                .ReturnsAsync(createdHall);

            // Act
            var result = await _controller.CreateHallAsync(createHallCommand);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<HallDto>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("New Hall", returnValue.Name);
        }

        [Fact]
        public async Task CreateHallAsync_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Arrange
            CreateHallCommand? request = null;

            // Act
            var result = await _controller.CreateHallAsync(request!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request cannot be null.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateHallAsync_ReturnsBadRequest_WhenHallCreationFails()
        {
            // Arrange
            var createHallCommand = new CreateHallCommand
            {
                Name = "New Hall",
                RowsCount = 10,
                SeatsCount = 20
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateHallCommand>(), default))
              .ReturnsAsync((HallDto)null!);

            // Act
            var result = await _controller.CreateHallAsync(createHallCommand);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to create hall.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateHallAsync_HandlesMediatrException()
        {
            // Arrange
            var createHallCommand = new CreateHallCommand
            {
                Name = "New Hall",
                RowsCount = 10,
                SeatsCount = 20
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateHallCommand>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateHallAsync(createHallCommand));
        }
    }
}
