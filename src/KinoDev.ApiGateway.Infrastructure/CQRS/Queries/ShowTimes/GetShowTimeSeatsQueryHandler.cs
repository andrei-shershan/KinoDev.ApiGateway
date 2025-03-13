using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.ShowTimes;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.ShowTimes
{
    public class GetShowTimeSeatsQuery : IRequest<ShowTimeSeatsDto>
    {
        public int ShowTimeId { get; set; }
    }

    public class GetShowTimeSeatsQueryHandler : IRequestHandler<GetShowTimeSeatsQuery, ShowTimeSeatsDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetShowTimeSeatsQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<ShowTimeSeatsDto> Handle(GetShowTimeSeatsQuery query, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetShowTimeSeatsAsync(query.ShowTimeId);
        }
    }
}