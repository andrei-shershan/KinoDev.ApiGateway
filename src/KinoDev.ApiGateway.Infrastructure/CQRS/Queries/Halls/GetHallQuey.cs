using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.Hall;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Halls
{
    public class GetHallQuery : IRequest<HallDto>
    {
        public int Id { get; set; }
    }

    public class GetHallQueryHandler : IRequestHandler<GetHallQuery, HallDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetHallQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<HallDto> Handle(GetHallQuery request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetHallByIdAsync(request.Id);
        }
    }
    
}