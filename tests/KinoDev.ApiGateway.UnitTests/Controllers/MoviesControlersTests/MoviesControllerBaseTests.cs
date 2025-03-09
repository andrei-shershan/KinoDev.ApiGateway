using KinoDev.ApiGateway.WebApi.Controllers;
using MediatR;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class MoviesControllerBaseTests
    {
        protected readonly Mock<IMediator> _mediatorMock;
        protected readonly MoviesController _controller;

        public MoviesControllerBaseTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new MoviesController(_mediatorMock.Object);
        }
    }
}
