using KinoDev.ApiGateway.Infrastructure.HttpClients;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Payments
{
    public class CreatePaymentIntentCommand : IRequest<string>
    {
        public int Amount { get; set; }

        public string Currency { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }

    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, string>
    {
        private readonly IPaymentClient _paymentClient;

        public CreatePaymentIntentCommandHandler(IPaymentClient paymentClient)
        {
            _paymentClient = paymentClient;
        }

        public async Task<string> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            if (request.Metadata == null)
            {
                request.Metadata = new Dictionary<string, string>();
            }

            return await _paymentClient.CreatePaymentIntentAsync(request.Amount, request.Currency, request.Metadata);
        }
    }
}