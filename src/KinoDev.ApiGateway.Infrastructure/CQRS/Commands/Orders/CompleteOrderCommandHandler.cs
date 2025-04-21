namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Orders
{
    using KinoDev.ApiGateway.Infrastructure.HttpClients;
    using KinoDev.Shared.DtoModels.Orders;
    using MediatR;

    public class CompleteOrderCommand : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public string PaymentIntentId { get; set; }
    }

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, OrderDto>
    {

        private readonly IDomainServiceClient _domainServiceClient;
        private readonly IPaymentClient _paymentClient;

        public CompleteOrderCommandHandler(IDomainServiceClient domainServiceClient, IPaymentClient paymentClient)
        {
            _domainServiceClient = domainServiceClient;
            _paymentClient = paymentClient;
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
                return null;
            }

            if (order.State != Shared.Enums.OrderState.New)
            {
                return null;
            }

            if (!paymentIntent.Metadata.ContainsKey("orderId"))
            {
                return null;
            }

            if (paymentIntent.Metadata["orderId"] != order.Id.ToString())
            {
                return null;
            }

            await _paymentClient.CompletePayment(request.PaymentIntentId);

            return await _domainServiceClient.CompleteOrderAsync(request.OrderId);
        }
    }
}