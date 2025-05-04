namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Orders
{
    using System.Threading;
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients;
    using KinoDev.Shared.DtoModels.Orders;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class GetActiveOrderQuery : IRequest<OrderSummary>
    {
        public Guid OrderId { get; set; }
    }

    public class GetActiveOrderQueryHandler : IRequestHandler<GetActiveOrderQuery, OrderSummary>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly ILogger<GetActiveOrderQueryHandler> _logger;

        public GetActiveOrderQueryHandler(IDomainServiceClient domainServiceClient, ILogger<GetActiveOrderQueryHandler> logger)
        {
            _domainServiceClient = domainServiceClient;
            _logger = logger;
        }

        public async Task<OrderSummary> Handle(GetActiveOrderQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
            {
                _logger.LogError("Invalid OrderId provided: {OrderId}", request.OrderId);
                return null;
            }

            return await _domainServiceClient.GetOrderSummaryAsync(request.OrderId);
        }
    }
}