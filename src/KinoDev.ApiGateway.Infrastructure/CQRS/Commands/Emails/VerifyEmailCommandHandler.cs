using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Models.Enums;
using KinoDev.ApiGateway.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Emails
{
    public class VerifyEmailCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public VerifyEmailReason Reason { get; set; }
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheKeyService _cacheKeyService;

        public VerifyEmailCommandHandler(
            IMemoryCache memoryCache,
            ICacheKeyService cacheKeyService)
        {
            _memoryCache = memoryCache;
            _cacheKeyService = cacheKeyService;
        }

        // This is emulator for the email verification process.
        // In a real application, you would send an email with a verification code to the user.
        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            if (request.Reason == VerifyEmailReason.ConpletedOrdersRequest)
            {
                var cacheKey = _cacheKeyService.GetCacheKey(CacheConstants.EmailVerificationCode, request.Email, EmailVerificationCodes.CompletedOrderRequest);
                if (_memoryCache.TryGetValue(cacheKey, out string verificationCode))
                {
                    return true;
                }
                else
                {
                    _memoryCache.Set(cacheKey, EmailVerificationCodes.CompletedOrderRequest, TimeSpan.FromMinutes(10));
                    return true;
                }
            }

            return false;
        }
    }
}