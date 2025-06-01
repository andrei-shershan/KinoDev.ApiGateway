using KinoDev.ApiGateway.Infrastructure.Services.Abstractions;
using KinoDev.ApiGateway.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.OrdersControllerTests
{
    public class OrdersControllerBaseTests
    {
        protected readonly Mock<IMediator> _mediatorMock;
        protected readonly Mock<ICookieResponseService> _cookieResponseServiceMock;
        protected readonly Mock<IMemoryCache> _memoryCacheMock;
        protected readonly OrdersController _controller;
        protected readonly Mock<HttpContext> _httpContextMock;
        protected readonly Mock<HttpRequest> _httpRequestMock;
        protected readonly Mock<HttpResponse> _httpResponseMock;
        protected readonly Mock<IResponseCookies> _responseCookiesMock;
        protected readonly Mock<IRequestCookieCollection> _requestCookiesMock;

        public OrdersControllerBaseTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _cookieResponseServiceMock = new Mock<ICookieResponseService>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpResponseMock = new Mock<HttpResponse>();
            _responseCookiesMock = new Mock<IResponseCookies>();
            _requestCookiesMock = new Mock<IRequestCookieCollection>();

            _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.Setup(x => x.Response).Returns(_httpResponseMock.Object);
            _httpResponseMock.Setup(x => x.Cookies).Returns(_responseCookiesMock.Object);
            _httpRequestMock.Setup(x => x.Cookies).Returns(_requestCookiesMock.Object);

            _controller = new OrdersController(_mediatorMock.Object, _cookieResponseServiceMock.Object, _memoryCacheMock.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }
    }
}
