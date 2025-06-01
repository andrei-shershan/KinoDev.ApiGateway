namespace KinoDev.ApiGateway.Infrastructure.Models.Payments;

public class GenericPaymentIntent
{
    public string PaymentIntentId { get; set; }
    public string ClientSecret { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public Dictionary<string, string> Metadata { get; set; }

    public string State { get; set; }

    public PaymentProvider PaymentProvider { get; set; }
}