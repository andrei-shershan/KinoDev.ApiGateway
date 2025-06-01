using KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Emails;
using KinoDev.ApiGateway.Infrastructure.Models.Enums;
using KinoDev.ApiGateway.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class VerifyEmailTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task VerifyEmail_ReturnsBadRequest_WhenVerificationFails()
        {
            // Arrange
            var model = new VerifyEmailModel { Email = "test@example.com" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<VerifyEmailCommand>(), default))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.VerifyEmail(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email verification failed", badRequestResult.Value.ToString());
            
            _mediatorMock.Verify(m => m.Send(It.Is<VerifyEmailCommand>(c => 
                c.Email == model.Email && 
                c.Reason == VerifyEmailReason.ConpletedOrdersRequest), default), Times.Once);
        }

        [Fact]
        public async Task VerifyEmail_ReturnsOkResult_WhenVerificationSucceeds()
        {
            // Arrange
            var model = new VerifyEmailModel { Email = "test@example.com" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<VerifyEmailCommand>(), default))
                .ReturnsAsync(true);

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
