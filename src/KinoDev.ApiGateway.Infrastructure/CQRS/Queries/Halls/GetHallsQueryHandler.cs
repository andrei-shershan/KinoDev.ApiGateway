using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.Hall;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls
{
    public class GetHallsQuery : IRequest<IEnumerable<HallDto>>
    {
        // This class can be extended with parameters if needed in the future
    }

    public class GetHallsQueryHandler : IRequestHandler<GetHallsQuery, IEnumerable<HallDto>>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetHallsQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<IEnumerable<HallDto>> Handle(GetHallsQuery request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetHallsAsync();
        }
    }

    
}