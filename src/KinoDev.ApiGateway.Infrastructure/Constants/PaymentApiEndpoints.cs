namespace KinoDev.ApiGateway.Infrastructure.Constants
{
    public class PaymentApiEndpoints
    {
        public class Payments
        {
            public const string CreatePaymentIntent = "api/payments";
            
            public static string GetPaymentIntent(string paymentIntentId) => $"api/payments/{paymentIntentId}";

            public static string CancelPendingOrderPayments(Guid orderId) => $"api/payments/order/{orderId}/cancel";

            public static string CompletePayment(string paymentIntentId) => $"api/payments/{paymentIntentId}/complete";
        }
    }
}