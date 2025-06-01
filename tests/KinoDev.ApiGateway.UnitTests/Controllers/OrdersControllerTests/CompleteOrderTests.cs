using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using KinoDev.ApiGateway.WebApi.Models;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class CompleteOrderTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task CompleteOrder_ReturnsBadRequest_WhenNoCookieExists()
        {
            // Arrange
            var model = new CompleteOrderModel { PaymentIntentId = "pi_12345" };
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns((string)null);

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("OrderId is required", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CompleteOrder_ReturnsBadRequest_WhenPaymentIntentIdIsEmpty()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var model = new CompleteOrderModel { PaymentIntentId = "" };
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("PaymentIntentId is required", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CompleteOrder_ReturnsBadRequest_WhenPaymentIntentIdIsWhitespace()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var model = new CompleteOrderModel { PaymentIntentId = "   " };
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("PaymentIntentId is required", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CompleteOrder_ReturnsBadRequest_WhenOrderCompletionFails()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var model = new CompleteOrderModel { PaymentIntentId = "pi_12345" };
            
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CompleteOrderCommand>(), default))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Order not found or already completed", badRequestResult.Value.ToString());
            
            _mediatorMock.Verify(m => m.Send(It.Is<CompleteOrderCommand>(c => 
                c.OrderId == Guid.Parse(orderId) && 
                c.PaymentIntentId == model.PaymentIntentId), default), Times.Once);
        }

        [Fact]
        public async Task CompleteOrder_ReturnsOkResult_WhenOrderCompletedSuccessfully()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var model = new CompleteOrderModel { PaymentIntentId = "pi_12345" };
            
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns((string)null);
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CompleteOrderCommand>(), default))
                .ReturnsAsync(new OrderDto());

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            Assert.IsType<OkResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.Is<CompleteOrderCommand>(c => 
                c.OrderId == Guid.Parse(orderId) && 
                c.PaymentIntentId == model.PaymentIntentId), default), Times.Once);
            
            _cookieResponseServiceMock.Verify(s => s.ResetCookie(
                _responseCookiesMock.Object,
                ResponseCookies.CookieOrderId,
                orderId), Times.Once);
            
            _cookieResponseServiceMock.Verify(s => s.AppendToCookieResponse(
                _responseCookiesMock.Object,
                ResponseCookies.PaidOrderId,
                orderId,
                It.Is<DateTime>(dt => dt > DateTime.UtcNow)), Times.Once);
        }

        [Fact]
        public async Task CompleteOrder_AppendsToExistingPaidOrderCookie_WhenPaidOrderCookieExists()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var existingPaidOrderId = Guid.NewGuid().ToString();
            var model = new CompleteOrderModel { PaymentIntentId = "pi_12345" };
            
            _requestCookiesMock.Setup(c => c[ResponseCookies.CookieOrderId]).Returns(orderId);
            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns(existingPaidOrderId);
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CompleteOrderCommand>(), default))
                .ReturnsAsync(new OrderDto());

            // Act
            var result = await _controller.CompleteOrder(model);

            // Assert
            Assert.IsType<OkResult>(result);
            
            _cookieResponseServiceMock.Verify(s => s.AppendToCookieResponse(
                _responseCookiesMock.Object,
                ResponseCookies.PaidOrderId,
                $"{existingPaidOrderId};{orderId}",
                It.Is<DateTime>(dt => dt > DateTime.UtcNow)), Times.Once);
        }
    }
}
