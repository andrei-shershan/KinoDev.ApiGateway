namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies
{
    using System.Threading;
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
    using MediatR;

    public class GetTest2Query : IRequest<bool>
    {
    }

    public class GetTest2QueryHandler : IRequestHandler<GetTest2Query, bool>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetTest2QueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<bool> Handle(GetTest2Query request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.Test2();
        }
    }

}