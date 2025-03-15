using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.Orders;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public int ShowTimeId { get; set; }

        public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public CreateOrderCommandHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var createOrderDto = new CreateOrderDto
            {
                ShowTimeId = request.ShowTimeId,
                SelectedSeatIds = request.SelectedSeatIds
            };

            return await _domainServiceClient.CreateOrderAsync(createOrderDto);
        }
    }
}