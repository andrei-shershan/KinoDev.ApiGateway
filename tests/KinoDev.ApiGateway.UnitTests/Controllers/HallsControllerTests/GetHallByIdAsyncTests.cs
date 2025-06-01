using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls;
using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class GetHallByIdAsyncTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task GetHallByIdAsync_PassesCorrectId_ToMediator()
        {
            // Arrange
            var hallId = 5;
            var hall = new HallDto { Id = hallId, Name = "Test Hall" };

            _mediatorMock.Setup(m => m.Send(It.Is<GetHallQuery>(q => q.Id == hallId), default))
                         .ReturnsAsync(hall)
                         .Verifiable();

            // Act
            await _controller.GetHallByIdAsync(hallId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetHallQuery>(q => q.Id == hallId), default), Times.Once);
        }

        [Fact]
        public async Task GetHallByIdAsync_ReturnsOk_WhenHallExists()
        {
            // Arrange
            var hallId = 1;
            var hall = new HallDto { Id = hallId, Name = "Test Hall" };

            _mediatorMock.Setup(m => m.Send(It.Is<GetHallQuery>(q => q.Id == hallId), default))
                         .ReturnsAsync(hall);

            // Act
            var result = await _controller.GetHallByIdAsync(hallId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<HallDto>(okResult.Value);

            Assert.Equal(hallId, returnValue.Id);
            Assert.Equal("Test Hall", returnValue.Name);
        }

        [Fact]
        public async Task GetHallByIdAsync_ReturnsNotFound_WhenHallDoesNotExist()
        {
            // Arrange
            var hallId = 999;

            _mediatorMock.Setup(m => m.Send(It.Is<GetHallQuery>(q => q.Id == hallId), default))
                         .ReturnsAsync((HallDto)null!);

            // Act
            var result = await _controller.GetHallByIdAsync(hallId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
