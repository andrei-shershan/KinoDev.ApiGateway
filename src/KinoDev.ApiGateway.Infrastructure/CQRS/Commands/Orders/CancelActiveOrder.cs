using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    public class CancelActiveOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }

    public class CancelActiveOrderHandler : IRequestHandler<CancelActiveOrderCommand, bool>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly ILogger<CancelActiveOrderHandler> _logger;

        public CancelActiveOrderHandler(
            IPaymentClient paymentClient,
            IDomainServiceClient domainServiceClient,
            ILogger<CancelActiveOrderHandler> logger)
        {
            _paymentClient = paymentClient;
            _domainServiceClient = domainServiceClient;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelActiveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _domainServiceClient.GetOrderAsync(request.OrderId);
            if (order == null || order.State != OrderState.New)
            {
                _logger.LogError("Order with ID {OrderId} not found or not in a cancellable state.", request.OrderId);
                return false;
            }

            var paymentResult = await _paymentClient.CancelPendingOrderPayments(order.Id);
            if (!paymentResult)
            {
                _logger.LogError("Failed to cancel payment for order with ID {OrderId}.", request.OrderId);
                return false;
            }

            return await _domainServiceClient.DeleteActiveOrder(request.OrderId);
        }
    }
}