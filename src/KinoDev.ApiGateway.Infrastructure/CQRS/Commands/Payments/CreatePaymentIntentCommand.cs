using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Payments
{
    public class CreatePaymentIntentCommand : IRequest<string?>
    {
        public string Email { get; set; } = null!;
        public Guid OrderId { get; set; }
    }

    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, string?>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IDomainServiceClient _domainServiceClient;

        public CreatePaymentIntentCommandHandler(IPaymentClient paymentClient, IDomainServiceClient domainServiceClient)
        {
            _paymentClient = paymentClient;
            _domainServiceClient = domainServiceClient;
        }

        public async Task<string?> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            var updatedOrder = await _domainServiceClient.UpdateOrderEmailAsync(request.OrderId, request.Email);
            if (updatedOrder == null)
            {
                System.Console.WriteLine("Failed to update order email.");
                return null;
            }

            var orderSummary = await _domainServiceClient.GetOrderSummaryAsync(request.OrderId);
            if (orderSummary == null || orderSummary.State != Shared.Enums.OrderState.New)
            {
                System.Console.WriteLine("Order not found or not in a valid state for payment.");
                return null;
            }

            var metadata = new Dictionary<string, string>();

            // TODO: Move to SHARED constants
            metadata[MetadataConstants.OrderId] = orderSummary.Id.ToString();
            metadata[MetadataConstants.Movie] = orderSummary.ShowTimeSummary.Movie.Name;

            foreach (var ticket in orderSummary.Tickets)
            {
                metadata[$"{ticket.Row}_{ticket.Number}"] = $"row: {ticket.Row}, number: {ticket.Number}";
            }

            return await _paymentClient.CreatePaymentIntentAsync(
                request.OrderId,
                orderSummary.Cost,
                "usd", // TODO: Move to SHARED constants
                metadata
                );
        }
    }
}