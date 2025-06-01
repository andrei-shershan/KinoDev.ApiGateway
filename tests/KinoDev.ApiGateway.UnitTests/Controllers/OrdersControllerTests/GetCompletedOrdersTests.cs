using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class GetCompletedOrdersTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetCompletedOrders_ReturnsEmptyArray_WhenNoCookieExists()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns((string)null);

            // Act
            var result = await _controller.GetCompletedOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Guid[]>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetCompletedOrders_ReturnsNotFound_WhenNoCompletedOrdersExist()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var cookieValue = $"{orderId1};{orderId2}";
            
            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns(cookieValue);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrdersCommand>(), default))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetCompletedOrders();

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrdersCommand>(q => 
                q.OrderIds.Contains(orderId1) && q.OrderIds.Contains(orderId2)), default), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrders_ReturnsOkWithOrders_WhenCompletedOrdersExist()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var cookieValue = $"{orderId1};{orderId2}";
            
            var orders = new List<OrderSummary> 
            { 
                new OrderSummary { Id = orderId1, },
                new OrderSummary { Id = orderId2, }
            };

            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns(cookieValue);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrdersCommand>(), default))
                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetCompletedOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrdersCommand>(q => 
                q.OrderIds.Contains(orderId1) && q.OrderIds.Contains(orderId2)), default), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrders_ParsesEncodedSemicolons_InCookieValue()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var cookieValue = $"{orderId1}%3B{orderId2}";  // %3B is encoded semicolon
            
            var orders = new List<OrderSummary>() 
            { 
                new OrderSummary { Id = orderId1, },
                new OrderSummary { Id = orderId2, }
            };
            
            _requestCookiesMock.Setup(c => c[ResponseCookies.PaidOrderId]).Returns(cookieValue);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrdersCommand>(), default))
                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetCompletedOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrdersCommand>(q => 
                q.OrderIds.Contains(orderId1) && q.OrderIds.Contains(orderId2)), default), Times.Once);
        }
    }
}
