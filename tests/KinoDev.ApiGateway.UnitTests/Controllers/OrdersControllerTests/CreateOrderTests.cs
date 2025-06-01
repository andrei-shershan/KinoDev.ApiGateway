using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using KinoDev.ApiGateway.WebApi.Models;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class CreateOrderTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenCommandFails()
        {
            // Arrange
            var model = new CreateOrderModel
            {
                ShowTimeId = 1,
                SelectedSeatIds = new List<int> { 1, 2, 3 }
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CreateOrder(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<CreateOrderCommand>(c => 
                c.ShowTimeId == model.ShowTimeId && 
                c.SelectedSeatIds == model.SelectedSeatIds), default), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ReturnsOkWithResponse_WhenOrderCreated()
        {
            // Arrange
            var model = new CreateOrderModel
            {
                ShowTimeId = 1,
                SelectedSeatIds = new List<int> { 1, 2, 3 }
            };
            
            var responseId = Guid.NewGuid();
            var response = new OrderSummary
            {
                Id = responseId,
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default))
                .ReturnsAsync(new OrderSummary());

            // Act
            var result = await _controller.CreateOrder(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateOrderCommand>(), default), Times.Once);
            
            _cookieResponseServiceMock.Verify(s => s.AppendToCookieResponse(
                It.IsAny<IResponseCookies>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()), Times.Once);
        }
    }
}
