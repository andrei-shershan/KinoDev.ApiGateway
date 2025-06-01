using KinoDev.ApiGateway.WebApi.Controllers;
using MediatR;
using Moq;

namespace KinoDev.ApiGateway.WebApi.UnitTests.Controllers.HallsControllerTests;

public class HallsControllerBaseTests
{
    protected readonly Mock<IMediator> _mediatorMock;
    protected readonly HallsController _controller;

    public HallsControllerBaseTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new HallsController(_mediatorMock.Object);
    }
}