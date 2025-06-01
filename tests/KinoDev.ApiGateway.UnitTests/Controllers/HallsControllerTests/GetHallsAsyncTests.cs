using KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls;
using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.ApiGateway.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class GetHallsAsyncTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task GetHallsAsync_ReturnsOkResult_WhenHallsExist()
        {
            // Arrange
            var halls = new List<HallDto> 
            { 
                new HallDto { Id = 1, Name = "Hall 1" }, 
                new HallDto { Id = 2, Name = "Hall 2" } 
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetHallsQuery>(), default)).ReturnsAsync(halls);

            // Act
            var result = await _controller.GetHallsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<HallDto>>(okResult.Value);
            var hallsList = returnValue.ToList();

            Assert.Equal(2, hallsList.Count);            Assert.Contains(hallsList, x => x.Id == 1 && x.Name == "Hall 1");
            Assert.Contains(hallsList, x => x.Id == 2 && x.Name == "Hall 2");
        }

        [Theory]
        [MemberData(nameof(HallsNullOrEmptyData))]
        public async Task GetHallsAsync_ReturnsNotFoundResult_WhenNoHallsExist(List<HallDto> halls)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetHallsQuery>(), default)).ReturnsAsync(halls);

            // Act
            var result = await _controller.GetHallsAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No halls found.", notFoundResult.Value);
        }        public static IEnumerable<object[]> HallsNullOrEmptyData =>
            new List<object[]>
            {
                new object[] { (IEnumerable<HallDto>)null! },
                new object[] { new List<HallDto> { } }
            };
    }
}
