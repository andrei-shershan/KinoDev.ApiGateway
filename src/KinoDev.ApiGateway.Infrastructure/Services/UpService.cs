namespace KinoDev.ApiGateway.Infrastructure.Services.Abstractions
{
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

    /// <summary>
    /// Interface for UP service.
    /// </summary>
    public class UpService : IUpService
    {
        private readonly IEmailServiceClient _emailServiceClient;
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly IStorageServiceClient _storageServiceClient;

        public UpService(
                    IEmailServiceClient emailServiceClient,
                    IDomainServiceClient domainServiceClient,
                    IStorageServiceClient storageServiceClient)
        {
            _emailServiceClient = emailServiceClient;
            _domainServiceClient = domainServiceClient;
            _storageServiceClient = storageServiceClient;
        }

        public async Task Up()
        {
            await _emailServiceClient.Up();
            await _domainServiceClient.Up();
            await _storageServiceClient.Up();
        }
    }
}