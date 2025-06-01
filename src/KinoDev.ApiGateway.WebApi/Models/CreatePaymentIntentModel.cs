namespace KinoDev.ApiGateway.WebApi.Models;

public class CreatePaymentIntentModel
{
    public int Amount { get; set; }

    public string Currency { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}