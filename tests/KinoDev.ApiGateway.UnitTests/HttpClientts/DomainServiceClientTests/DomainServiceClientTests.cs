using Moq.Protected;
using Moq;
using System.Net;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.Shared.DtoModels.Movies;

namespace KinoDev.ApiGateway.UnitTests.HttpClientts.DomainServiceClientTests
{
    public class DomainServiceClientTests : DomainServiceClientBaseTests
    {
        [Fact]
        public async Task GetMoviesAsync_ReturnsMovies()
        {
            // Arrange
            var movies = new List<MovieDto>
            {
                new MovieDto { Id = 1, Name = "Movie 1" },
                new MovieDto { Id = 2, Name = "Movie 2" }
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) =>
                {
                    _capturedRequest = request;
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(movies))
                });

            // Act
            var result = await _domainServiceClient.GetMoviesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Id == 1 && m.Name == "Movie 1");
            Assert.Contains(result, m => m.Id == 2 && m.Name == "Movie 2");

            // Validate the called URI
            Assert.NotNull(_capturedRequest);
            Assert.Equal($"{_baseUrl}{DomainApiEndpoints.Movies.GetMovies}", _capturedRequest.RequestUri.ToString());
        }
    }
}
