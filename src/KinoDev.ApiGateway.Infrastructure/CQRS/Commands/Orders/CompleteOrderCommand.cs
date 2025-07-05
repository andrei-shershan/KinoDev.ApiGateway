namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    using System.Text;
    using KinoDev.ApiGateway.Infrastructure.Constants;
    using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
    using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
    using KinoDev.ApiGateway.Infrastructure.Services.Abstractions;
    using KinoDev.Shared.DtoModels.Orders;
    using KinoDev.Shared.Services.Abstractions;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CompleteOrderCommand : IRequest<OrderDto?>
    {
        public Guid OrderId { get; set; }
        public string PaymentIntentId { get; set; } = null!;
    }

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, OrderDto?>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly IPaymentClient _paymentClient;
        private readonly ILogger<CompleteOrderCommandHandler> _logger;
        private readonly IMessageBrokerService _messageBrokerService;

        private readonly MessageBrokerSettings _messageBrokerSettings;

        private readonly IUpService _upService;

        public CompleteOrderCommandHandler(
            IDomainServiceClient domainServiceClient,
            IPaymentClient paymentClient,
            ILogger<CompleteOrderCommandHandler> logger,
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerOptions,
            IUpService upService)
        {
            _domainServiceClient = domainServiceClient;
            _paymentClient = paymentClient;
            _logger = logger;
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerOptions.Value;
            _upService = upService;
        }

        public async Task<OrderDto?> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var paymentIntentTask = _paymentClient.GetPaymentIntentAsync(request.PaymentIntentId);
            var orderTask = _domainServiceClient.GetOrderAsync(request.OrderId);

            await Task.WhenAll(paymentIntentTask, orderTask);

            var paymentIntent = await paymentIntentTask;
            var order = await orderTask;

            // TODO: Move to validation service
            if (order == null || paymentIntent == null)
            {
                _logger.LogError("Order or PaymentIntent not found. OrderId: {OrderId}, PaymentIntentId: {PaymentIntentId}", request.OrderId, request.PaymentIntentId);
                return null;
            }

            if (order.State != Shared.Enums.OrderState.New)
            {
                _logger.LogError("Order is not in a valid state to be completed. OrderId: {OrderId}, State: {State}", request.OrderId, order.State);
                return null;
            }

            if (!paymentIntent.Metadata.ContainsKey(MetadataConstants.OrderId))
            {
                _logger.LogError("PaymentIntent does not contain orderId in metadata. PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
                return null;
            }

            // TODO Move to constants
            if (paymentIntent.Metadata[MetadataConstants.OrderId] != order.Id.ToString())
            {
                _logger.LogError("PaymentIntent orderId does not match the order. PaymentIntentId: {PaymentIntentId}, OrderId: {OrderId}", request.PaymentIntentId, request.OrderId);
                return null;
            }

            await _paymentClient.CompletePayment(request.PaymentIntentId);

            var completedOrder = await _domainServiceClient.CompleteOrderAsync(request.OrderId);
            if (completedOrder != null)
            {
                var orderSummary = await _domainServiceClient.GetOrderSummaryAsync(completedOrder.Id);

                // TODO: Handle empty Url properly
                // Should be setup as empty to handle properly in functions
                orderSummary.FileUrl = string.Empty;

                try
                {
                    await _upService.Up();
                    await _messageBrokerService.SendMessageAsync(
                        _messageBrokerSettings.Queues.OrderCompleted,
                        orderSummary
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send OrderCompleted message to the message broker. OrderId: {OrderId}", completedOrder.Id);
                }
            }

            return completedOrder;
        }
    }
}