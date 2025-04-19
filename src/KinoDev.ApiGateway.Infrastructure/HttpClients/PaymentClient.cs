using System.Text;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    // TODO: Move to Shared project
    // TOOD: Use models from shared project
    public enum PaymentProvider
    {
        Stripe
    }

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

    public interface IPaymentClient
    {
        Task<string> CreatePaymentIntentAsync(int amount, string currency, Dictionary<string, string> metadata);
        Task<GenericPaymentIntent> GetPaymentIntentAsync(string id);
        Task<bool> CompletePayment(string paymentIntentId);
    }

    public class PaymentClient : IPaymentClient
    {
        private readonly HttpClient _httpClient;

        public PaymentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CompletePayment(string paymentIntentId)
        {
            var requestUri = PaymentApiEndpoints.Payments.CompletePayment(paymentIntentId);
            var response = await _httpClient.PostAsync(requestUri, null);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<string> CreatePaymentIntentAsync(int amount, string currency, Dictionary<string, string> metadata)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { amount, currency, metadata }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(PaymentApiEndpoints.Payments.CreatePaymentIntent, requestContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return await response.GetResponseAsync<string>();
        }

        public async Task<GenericPaymentIntent> GetPaymentIntentAsync(string id)
        {
            var requestUri = PaymentApiEndpoints.Payments.GetPaymentIntent(id);
            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.GetResponseAsync<GenericPaymentIntent>();
            }

            return null;
        }

    }
}
