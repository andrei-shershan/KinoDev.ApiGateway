using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public interface ICookieResponseService
    {
        void AppendToCookieResponse(IResponseCookies cookies, string key, string value, DateTime expiresAt);

        void ResetCookie(IResponseCookies cookies, string key, string value);
    }

    public class CookieResponseService : ICookieResponseService
    {
        private readonly CookieResponseSettings _cookieResponseSettings;

        public CookieResponseService(IOptions<CookieResponseSettings> options)
        {
            _cookieResponseSettings = options.Value;
        }

        public void AppendToCookieResponse(IResponseCookies cookies, string key, string value, DateTime expiresAt)
        {
            string domain = null;
            if (!string.IsNullOrWhiteSpace(_cookieResponseSettings.Domain))
            {
                domain = _cookieResponseSettings.Domain;
            }

            cookies.Append(
                key,
                value,
                new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = domain,
                    Path = "/",
                    Expires = expiresAt
                });
        }

        public void ResetCookie(IResponseCookies cookies, string key, string value)
        {
            AppendToCookieResponse(cookies, key, value, default(DateTime));
        }
    }
}