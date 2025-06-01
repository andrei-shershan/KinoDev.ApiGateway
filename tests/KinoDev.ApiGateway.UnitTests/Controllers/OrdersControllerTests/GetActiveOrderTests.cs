using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class GetActiveOrderTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetActiveOrder_ReturnsNotFound_WhenNoCookieExists()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns((string)null);

            // Act
            var result = await _controller.GetActiveOrder();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetActiveOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetActiveOrderQuery>(), default))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetActiveOrder();

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<GetActiveOrderQuery>(q =>
                q.OrderId == Guid.Parse(orderId)), default), Times.Once);
        }

        [Fact]
        public async Task GetActiveOrder_ReturnsOkWithResponse_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderResponse = new OrderSummary()
            {
                Id = orderId,
            };

            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId.ToString());
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetActiveOrderQuery>(), default))
                .ReturnsAsync(orderResponse);

            // Act
            var result = await _controller.GetActiveOrder();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderResponse, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<GetActiveOrderQuery>(q =>
                q.OrderId == orderId), default), Times.Once);
        }
    }
}
