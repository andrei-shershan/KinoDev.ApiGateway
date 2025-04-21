using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.Enums;
using MediatR;

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

        public CancelActiveOrderHandler(IPaymentClient paymentClient, IDomainServiceClient domainServiceClient)
        {
            _paymentClient = paymentClient;
            _domainServiceClient = domainServiceClient;
        }


        public async Task<bool> Handle(CancelActiveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _domainServiceClient.GetOrderAsync(request.OrderId);
            if (order == null || order.State != OrderState.New)
            {
                return false;
            }

            var paymentResult = await _paymentClient.CancelPendingOrderPayments(order.Id);
            if (!paymentResult)
            {
                return false;
            }

            return await _domainServiceClient.DeleteActiveOrder(request.OrderId);
        }
    }
}