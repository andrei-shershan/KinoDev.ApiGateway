namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies
{
    using System.Threading;
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
    using MediatR;

    public class GetTestQuery : IRequest<bool>
    {
        public class GetTestQueryHandler : IRequestHandler<GetTestQuery, bool>
        {
            private readonly IDomainServiceClient _domainServiceClient;

            public GetTestQueryHandler(IDomainServiceClient domainServiceClient)
            {
                _domainServiceClient = domainServiceClient;
            }

            public async Task<bool> Handle(GetTestQuery request, CancellationToken cancellationToken)
            {
                return await _domainServiceClient.Test();
            }
        }
    }
}