namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders
{
    using System.Threading;
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients;
    using KinoDev.Shared.DtoModels.Orders;
    using MediatR;

    public class GetActiveOrderQuery : IRequest<OrderSummary>
    {
        public Guid OrderId { get; set; }
    }

    public class GetActiveOrderQueryHandler : IRequestHandler<GetActiveOrderQuery, OrderSummary>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetActiveOrderQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<OrderSummary> Handle(GetActiveOrderQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
            {
                return null;
            }
            
            return await _domainServiceClient.GetActiveOrderSummaryAsync(request.OrderId);            
        }
    }
}