namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    using KinoDev.ApiGateway.Infrastructure.HttpClients;
    using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
    using KinoDev.ApiGateway.Infrastructure.Services;
    using KinoDev.Shared.DtoModels.Orders;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CompleteOrderCommand : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public string PaymentIntentId { get; set; }
    }

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, OrderDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly IPaymentClient _paymentClient;
        private readonly ILogger<CompleteOrderCommandHandler> _logger;
        private readonly IMessageBrokerService _messageBrokerService;

        private readonly MessageBrokerSettings _messageBrokerSettings;

        public CompleteOrderCommandHandler(
            IDomainServiceClient domainServiceClient,
            IPaymentClient paymentClient,
            ILogger<CompleteOrderCommandHandler> logger,
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerOptions)
        {
            _domainServiceClient = domainServiceClient;
            _paymentClient = paymentClient;
            _logger = logger;
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerOptions.Value;
        }

        public async Task<OrderDto> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
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

            if (!paymentIntent.Metadata.ContainsKey("orderId"))
            {
                _logger.LogError("PaymentIntent does not contain orderId in metadata. PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
                return null;
            }

            // TODO Move to constants
            if (paymentIntent.Metadata["orderId"] != order.Id.ToString())
            {
                _logger.LogError("PaymentIntent orderId does not match the order. PaymentIntentId: {PaymentIntentId}, OrderId: {OrderId}", request.PaymentIntentId, request.OrderId);
                return null;
            }

            await _paymentClient.CompletePayment(request.PaymentIntentId);

            var completedOrder = await _domainServiceClient.CompleteOrderAsync(request.OrderId);

            if (completedOrder != null)
            {
                try
                {
                    _logger.LogInformation(_messageBrokerService == null ? "Message broker service is null" : "Message broker service is not null");
                    _logger.LogInformation("Message broker settings: {@MessageBrokerSettings}", _messageBrokerSettings);
                    _logger.LogInformation("Publishing order completion event for OrderId: {OrderId}", completedOrder.Id);
                    _logger.LogInformation("Order completed event data: {@EventData}", new
                    {
                        OrderId = completedOrder.Id,
                        Email = completedOrder.Email,
                        CompletedAt = DateTime.UtcNow
                    });
                    // Publish order completed event to RabbitMQ
                    await _messageBrokerService.PublishAsync(
                        new
                        {
                            OrderId = completedOrder.Id,
                            Email = completedOrder.Email,
                            CompletedAt = DateTime.UtcNow
                        },
                        _messageBrokerSettings.Topics.OrderCompleted
                    );

                    _logger.LogInformation("Published order completion event for OrderId: {OrderId}", completedOrder.Id);
                }
                catch (Exception ex)
                {
                    // Log but don't fail the operation if message publishing fails
                    _logger.LogError(ex, "Failed to publish order completion event for OrderId: {OrderId}", completedOrder.Id);
                }
            }

            return completedOrder;
        }
    }
}