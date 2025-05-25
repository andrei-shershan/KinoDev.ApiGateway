using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.Hall;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Halls
{
    public class CreateHallCommand : IRequest<HallDto>
    {
        public HallDto Hall { get; set; }
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
            if (request.Hall == null)
            {
                throw new ArgumentNullException(nameof(request.Hall), "Hall data is required.");
            }

            return await _domainServiceClient.CreateHallAsync(request.Hall);
        }
    }
}