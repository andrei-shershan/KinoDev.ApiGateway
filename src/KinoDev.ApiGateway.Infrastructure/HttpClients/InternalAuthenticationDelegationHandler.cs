using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Models;
using KinoDev.ApiGateway.Infrastructure.Services;
using System.Net.Http.Headers;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class InternalAuthenticationDelegationHandler : DelegatingHandler
    {
        private readonly IAuthenticationClient _authenticationClient;
        private readonly ICacheProvider _cacheProvider;
        private TokenModel _token = null;

        public InternalAuthenticationDelegationHandler(
            IAuthenticationClient authenticationClient,
            ICacheProvider cacheProvider)
        {
            _authenticationClient = authenticationClient;
            _cacheProvider = cacheProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _token = GetNotExpiredTokenFromCache();
            if (_token == null)
            {
                var tokenResponse = await _authenticationClient.GetClientTokenAsync();
                _token = new TokenModel
                {
                    AccessToken = tokenResponse.AccessToken,
                    ExpiredAt = tokenResponse.ExpiredAt
                };

                _cacheProvider.Set<TokenModel>(
                    CacheConstants.TokenKey,
                    tokenResponse,
                    (tokenResponse.ExpiredAt - DateTime.UtcNow - TimeSpan.FromMinutes(1))
                    );
            }

            // Add the token to the request headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

            // Call the inner handler to continue processing
            return await base.SendAsync(request, cancellationToken);
        }

        private TokenModel GetNotExpiredTokenFromCache()
        {
            var cachedToken = _cacheProvider.Get<TokenModel>(CacheConstants.TokenKey);
            if (cachedToken != null && DateTime.UtcNow < cachedToken.ExpiredAt)
            {
                return cachedToken;
            }

            return null;
        }
    }
}
