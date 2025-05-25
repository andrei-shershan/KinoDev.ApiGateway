using KinoDev.ApiGateway.WebApi.Controllers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class MoviesControllerBaseTests
    {
        protected readonly Mock<IMediator> _mediatorMock;
        protected readonly Mock<ILogger<MoviesController>> _loggerMock;
        protected readonly MoviesController _controller;

        public MoviesControllerBaseTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<MoviesController>>();
            _controller = new MoviesController(_loggerMock.Object, _mediatorMock.Object);
        }
    }
}
