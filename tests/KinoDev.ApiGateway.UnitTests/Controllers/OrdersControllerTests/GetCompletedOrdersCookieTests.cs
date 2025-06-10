using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class GetCompletedOrdersCookieTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetCompletedOrdersCookie_ReturnsNotFound_WhenNoOrdersFound()
        {
            // Arrange
            var model = new GetCompletedOrderIdsByCodeVerifiedEmail
            {
                Email = "test@example.com",
                Code = "123456"
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrderIdsByCodeVerifiedEmail>(), default))
                .ReturnsAsync((IEnumerable<Guid>)null);

            // Act
            var result = await _controller.GetCompletedOrdersCookie(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrderIdsByCodeVerifiedEmail>(q => 
                q.Email == model.Email && 
                q.Code == model.Code), default), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersCookie_ReturnsOk_WhenEmptyOrdersReturned()
        {
            // Arrange
            var model = new GetCompletedOrderIdsByCodeVerifiedEmail
            {
                Email = "test@example.com",
                Code = "123456"
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrderIdsByCodeVerifiedEmail>(), default))
                .ReturnsAsync(new List<Guid>());

            // Act
            var result = await _controller.GetCompletedOrdersCookie(model);

            // Assert
            Assert.IsType<OkResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrderIdsByCodeVerifiedEmail>(q => 
                q.Email == model.Email && 
                q.Code == model.Code), default), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersCookie_ReturnsOk_WhenOrdersFound()
        {
            // Arrange
            var model = new GetCompletedOrderIdsByCodeVerifiedEmail
            {
                Email = "test@example.com",
                Code = "123456"
            };
            
            var orderIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var expectedCookieValue = string.Join(";", orderIds.Select(id => id.ToString()));
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedOrderIdsByCodeVerifiedEmail>(), default))
                .ReturnsAsync(orderIds);

            // Act
            var result = await _controller.GetCompletedOrdersCookie(model);

            // Assert
            Assert.IsType<OkResult>(result);
            
            _mediatorMock.Verify(m => m.Send(It.Is<GetCompletedOrderIdsByCodeVerifiedEmail>(q => 
                q.Email == model.Email && 
                q.Code == model.Code), default), Times.Once);
            
            _cookieResponseServiceMock.Verify(s => s.AppendToCookieResponse(
                _responseCookiesMock.Object,
                ResponseCookies.PaidOrderId,
                expectedCookieValue,
                It.Is<DateTime>(dt => dt > DateTime.UtcNow)), Times.Once);
        }
    }
}
