using Castle.Core.Logging;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.HttpClientts.DomainServiceClientTests
{
    public class DomainServiceClientBaseTests
    {
        protected HttpRequestMessage _capturedRequest = null;
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        protected const string _baseUrl = "https://localhost/";

        protected readonly HttpClient _httpClient;
        protected readonly DomainServiceClient _domainServiceClient;

        protected Mock<ILogger<DomainServiceClient>> _loggerMock = new();

        public DomainServiceClientBaseTests()
        {
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            _domainServiceClient = new DomainServiceClient(_httpClient, _loggerMock.Object);
        }
    }
}
