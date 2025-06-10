using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Extensions;
using KinoDev.Shared.Helpers;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders
{
    public class GetCompletedOrderIdsByCodeVerifiedEmail : IRequest<IEnumerable<Guid>?>
    {
        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;
    }

    public class GetCompletedOrderIdsByCodeVerifiedEmailHandler : IRequestHandler<GetCompletedOrderIdsByCodeVerifiedEmail, IEnumerable<Guid>?>
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

        public async Task<IEnumerable<Guid>?> Handle(GetCompletedOrderIdsByCodeVerifiedEmail request, CancellationToken cancellationToken)
        {
            var hashedCode = HashHelper.CalculateSha256Hash(request.Email, request.Code);
            var cacheKey = _cacheKeyService.GetCacheKey(CacheConstants.EmailVerificationCode, hashedCode);
            if (_memoryCache.TryGetValue(cacheKey, out _))
            {
                var completedOrders = await _domainServiceClient.GetCompletedOrdersByEmail(request.Email);
                if (!completedOrders.IsNullOrEmptyCollection())
                {
                    return completedOrders.Select(x => x.Id);
                }
                else
                {
                    return new List<Guid>();
                }
            }

            return null;
        }
    }
}