using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Moq;
using Moq.Protected;
using System.Net;

namespace KinoDev.ApiGateway.UnitTests.HttpClientts.DomainServiceClientTests
{
    public class GetShowingMoviesTests : DomainServiceClientBaseTests
    {
        [Fact]
        public async Task GetMoviesAsync_ReturnsMovies()
        {
            var date = DateTime.UtcNow;

            // Arrange
            var showingMovies = new List<ShowingMovie>
            {
                new ShowingMovie()
                {
                    Description = "Movie 1",
                    Id = 1,
                    Name = "Movie 1",
                    Duration = 120,
                    Url = "https://movie1.com",
                    MovieShowTimeDetails = new List<MovieShowTimeDetails>()
                    {
                        new MovieShowTimeDetails()
                        {
                            HallId = 1,
                            HallName = "Hall 1",
                            Time = DateTime.UtcNow,
                            IsSellingAvailable = true,
                            Price = 10
                        }
                    }
                }
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
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(showingMovies))
                });

            // Act
            var result = await _domainServiceClient.GetShowingMoviesAsync(date);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, x => x.Id == 1 && x.Name == "Movie 1" && x.MovieShowTimeDetails.FirstOrDefault(st => st.HallName == "Hall 1") != null);


            // Validate the called URI
            Assert.NotNull(_capturedRequest);
            Assert.Equal($"{_baseUrl}{DomainApiEndpoints.Movies.GetShowingMovies}?date={date}", WebUtility.UrlDecode(_capturedRequest.RequestUri.ToString()));
        }
    }
}
