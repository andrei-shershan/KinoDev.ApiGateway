using KinoDev.ApiGateway.Infrastructure.HttpClients;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Payments
{
    public class CreatePaymentIntentCommand : IRequest<string>
    {
        public Guid OrderId { get; set; }
    }

    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, string>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IDomainServiceClient _domainServiceClient;

        public CreatePaymentIntentCommandHandler(IPaymentClient paymentClient, IDomainServiceClient domainServiceClient)
        {
            _paymentClient = paymentClient;
            _domainServiceClient = domainServiceClient;
        }

        public async Task<string> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            var orderSummary = await _domainServiceClient.GetOrderSummaryAsync(request.OrderId);
            if (orderSummary == null || orderSummary.State != Shared.Enums.OrderState.New)
            {
                return null;
            }

            var metadata = new Dictionary<string, string>();

            // TODO: Move to SHARED constants
            metadata["orderId"] = orderSummary.Id.ToString();
            metadata["movie"] = orderSummary.ShowTimeSummary.Movie.Name;

            foreach (var ticket in orderSummary.Tickets)
            {
                metadata[$"{ticket.Row}_{ticket.Number}"] = $"row: {ticket.Row}, number: {ticket.Number}";
            }

            return await _paymentClient.CreatePaymentIntentAsync(
                request.OrderId,
                orderSummary.Cost,
                "usd",
                metadata
                );
        }
    }
}