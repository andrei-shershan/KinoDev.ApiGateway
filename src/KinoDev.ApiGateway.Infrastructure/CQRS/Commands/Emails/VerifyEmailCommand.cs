using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.Infrastructure.Models.Enums;
using KinoDev.ApiGateway.Infrastructure.Services;
using KinoDev.ApiGateway.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Helpers;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Emails
{
    public class VerifyEmailCommand : IRequest<bool>
    {
        public required string Email { get; set; }
        public VerifyEmailReason Reason { get; set; }
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEmailServiceClient _emailServiceClient;

        public VerifyEmailCommandHandler(
            IMemoryCache memoryCache,
            ICacheKeyService cacheKeyService,
            IEmailServiceClient emailServiceClient)
        {
            _memoryCache = memoryCache;
            _cacheKeyService = cacheKeyService;
            _emailServiceClient = emailServiceClient;
        }

        // TODO: Store code in database
        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            if (request.Reason == VerifyEmailReason.ConpletedOrdersRequest)
            {
                var verificationCode = GenerateVerificationCode();

                var hashedCode = HashHelper.CalculateSha256Hash(request.Email, verificationCode);

                var cacheKey = _cacheKeyService.GetCacheKey(CacheConstants.EmailVerificationCode, hashedCode);
                if (_memoryCache.TryGetValue(cacheKey, out _))
                {
                    _memoryCache.Remove(cacheKey);
                }

                var response = await _emailServiceClient.SendEmailAsync(
                    request.Email,
                    "Email Verification",
                    $"<p>Your verification code is:</p> <h2>{verificationCode}</h2>");

                if (!response)
                {
                    return false; // Email sending failed
                }

                _memoryCache.Set(cacheKey, verificationCode, TimeSpan.FromMinutes(10));
                return true;
            }

            return false;
        }

        private string GenerateVerificationCode()
        {
            // Generate a random 6-digit verification code
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}