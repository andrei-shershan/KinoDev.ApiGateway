using KinoDev.ApiGateway.Infrastructure.HttpClients;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries
{
    public class GetInternalTestDataQuery: IRequest<string>
    {

    }

    public class GetInternalTestDataQueryHandler : IRequestHandler<GetInternalTestDataQuery, string>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetInternalTestDataQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<string> Handle(GetInternalTestDataQuery request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.TestCall();
        }
    }

}
