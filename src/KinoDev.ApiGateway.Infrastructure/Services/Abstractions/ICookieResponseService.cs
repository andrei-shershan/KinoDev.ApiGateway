using Microsoft.AspNetCore.Http;

namespace KinoDev.ApiGateway.Infrastructure.Services.Abstractions;

public interface ICookieResponseService
{
    void AppendToCookieResponse(IResponseCookies cookies, string key, string value, DateTime expiresAt);

    void ResetCookie(IResponseCookies cookies, string key, string value);
}