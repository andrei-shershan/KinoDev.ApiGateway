using KinoDev.ApiGateway.Infrastructure.Models.Payments;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

public interface IPaymentClient
{
    Task<string> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, Dictionary<string, string> metadata);

    Task<GenericPaymentIntent> GetPaymentIntentAsync(string id);

    Task<bool> CompletePayment(string paymentIntentId);

    Task<bool> CancelPendingOrderPayments(Guid orderId);
}