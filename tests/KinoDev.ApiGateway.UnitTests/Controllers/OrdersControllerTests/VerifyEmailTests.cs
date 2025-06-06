using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Emails;
using KinoDev.ApiGateway.Infrastructure.Models.Enums;
using KinoDev.ApiGateway.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class VerifyEmailTests : OrdersControllerBaseTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task VerifyEmail_ReturnsOkResult_WhenVerificationCalled(bool isSuccess)
        {
            // Arrange
            var model = new VerifyEmailModel { Email = "test@example.com" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<VerifyEmailCommand>(), default))
                .ReturnsAsync(isSuccess);

            // Act
            var result = await _controller.VerifyEmail(model);

            // Assert
            Assert.IsType<OkResult>(result);

            _mediatorMock.Verify(m => m.Send(It.Is<VerifyEmailCommand>(c =>
                c.Email == model.Email &&
                c.Reason == VerifyEmailReason.ConpletedOrdersRequest), default), Times.Once);
        }
    }
}
