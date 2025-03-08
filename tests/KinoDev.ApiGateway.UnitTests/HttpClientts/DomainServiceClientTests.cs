using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels;
using Moq.Protected;
using Moq;
using System.Net;
using KinoDev.ApiGateway.Infrastructure.Constants;

namespace KinoDev.ApiGateway.UnitTests.HttpClientts
{
    public class DomainServiceClientTests
    {
        private HttpRequestMessage _capturedRequest = null;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        private const string _baseUrl = "https://localhost/";

        private readonly HttpClient _httpClient;
        private readonly DomainServiceClient _domainServiceClient;

        public DomainServiceClientTests()
        {
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            _domainServiceClient = new DomainServiceClient(_httpClient);
        }

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
