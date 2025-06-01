using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.ShowTimes
{
    public class CreateShowTimeCommand : IRequest<bool>
    {
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateShowTimeCommandHandler : IRequestHandler<CreateShowTimeCommand, bool>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        public CreateShowTimeCommandHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<bool> Handle(CreateShowTimeCommand request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.CreateShowTimeAsync(request.MovieId, request.HallId, request.Time, request.Price);
        }
    }
}