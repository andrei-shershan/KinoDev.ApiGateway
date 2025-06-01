using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.ShowTimes;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.ShowTimes
{
    public class GetShowTimeDetailsQuery : IRequest<ShowTimeDetailsDto>
    {
        public int ShowTimeId { get; set; }
    }

    public class GetShowTimeDetailsQueryHandler : IRequestHandler<GetShowTimeDetailsQuery, ShowTimeDetailsDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetShowTimeDetailsQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<ShowTimeDetailsDto> Handle(GetShowTimeDetailsQuery query, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetShowTimeDetailsAsync(query.ShowTimeId);
        }
    }
}