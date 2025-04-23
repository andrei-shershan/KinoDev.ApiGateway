using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.Orders;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    public class GetCompletedOrdersCommand : IRequest<IEnumerable<OrderSummary>>
    {
        public IEnumerable<Guid> OrderIds { get; set; }

        public string Email { get; set; }
    }

    public class GetCompletedOrdersCommandHandler : IRequestHandler<GetCompletedOrdersCommand, IEnumerable<OrderSummary>>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetCompletedOrdersCommandHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<IEnumerable<OrderSummary>> Handle(GetCompletedOrdersCommand request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetCompletedOrdersAsync(request.OrderIds, request.Email);
        }
    }
}
