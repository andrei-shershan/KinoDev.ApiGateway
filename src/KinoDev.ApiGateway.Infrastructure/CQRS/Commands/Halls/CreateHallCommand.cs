using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.Hall;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Halls
{
    public class CreateHallCommand : IRequest<HallDto>
    {
        public string Name { get; set; } = null!;
        
        public int RowsCount { get; set; }

        public int SeatsCount { get; set; }
    }

    public class CreateHallCommandHandler : IRequestHandler<CreateHallCommand, HallDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public CreateHallCommandHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<HallDto> Handle(CreateHallCommand request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.CreateHallAsync(request.Name, request.RowsCount, request.SeatsCount);
        }
    }
}