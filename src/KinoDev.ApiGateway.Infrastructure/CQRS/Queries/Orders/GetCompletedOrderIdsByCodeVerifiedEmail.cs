using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.Services;
using KinoDev.Shared.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders
{
    public class GetCompletedOrderIdsByCodeVerifiedEmail : IRequest<IEnumerable<Guid>>
    {
        public string Email { get; set; }

        public string Code { get; set; }
    }

    public class GetCompletedOrderIdsByCodeVerifiedEmailHandler : IRequestHandler<GetCompletedOrderIdsByCodeVerifiedEmail, IEnumerable<Guid>>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IMemoryCache _memoryCache;

        public GetCompletedOrderIdsByCodeVerifiedEmailHandler(
            IDomainServiceClient domainServiceClient,
            ICacheKeyService cacheKeyService,
            IMemoryCache memoryCache
            )

        {
            _domainServiceClient = domainServiceClient;
            _cacheKeyService = cacheKeyService;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Guid>> Handle(GetCompletedOrderIdsByCodeVerifiedEmail request, CancellationToken cancellationToken)
        {
            var cacheKey = _cacheKeyService.GetCacheKey(CacheConstants.EmailVerificationCode, request.Email, request.Code);
            if (_memoryCache.TryGetValue(cacheKey, out string verificationCode))
            {
                var completedOrders = await _domainServiceClient.GetCompletedOrdersByEmail(request.Email);
                if (!completedOrders.IsNullOrEmptyCollection())
                {
                    return completedOrders.Select(x => x.Id);
                }
            }

            return null;
        }
    }
}