using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class CancelActiveOrderTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task CancelActiveOrder_ReturnsNotFound_WhenNoCookieExists()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns((string)null);

            // Act
            var result = await _controller.CancelActiveOrder();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CancelActiveOrder_ReturnsBadRequest_WhenOrderCancellationFails()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelActiveOrderCommand>(), default))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CancelActiveOrder();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Order not found or already completed", badRequestResult.Value.ToString());
            
            _mediatorMock.Verify(m => m.Send(It.Is<CancelActiveOrderCommand>(c => 
                c.OrderId == Guid.Parse(orderId)), default), Times.Once);
        }

        [Fact]
        public async Task CancelActiveOrder_ReturnsOkResult_WhenOrderCancelledSuccessfully()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelActiveOrderCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CancelActiveOrder();

            // Assert
            Assert.IsType<OkResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.Is<CancelActiveOrderCommand>(c => 
                c.OrderId == Guid.Parse(orderId)), default), Times.Once);
            
            _cookieResponseServiceMock.Verify(s => s.ResetCookie(
                _responseCookiesMock.Object,
                ResponseCookies.CookieOrderId,
                orderId), Times.Once);
        }
    }
}
