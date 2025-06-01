using System.Text;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.Infrastructure.Models.Payments;
using KinoDev.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public class PaymentClient : IPaymentClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentClient> _logger;

        public PaymentClient(HttpClient httpClient, ILogger<PaymentClient> logger)   
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> CancelPendingOrderPayments(Guid orderId)
        {
            var requestUri = PaymentApiEndpoints.Payments.CancelPendingOrderPayments(orderId);
            var response = await _httpClient.PostAsync(requestUri, null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompletePayment(string paymentIntentId)
        {
            var requestUri = PaymentApiEndpoints.Payments.CompletePayment(paymentIntentId);
            var response = await _httpClient.PostAsync(requestUri, null);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, Dictionary<string, string> metadata)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { orderId = orderId.ToString(), amount, currency, metadata }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(PaymentApiEndpoints.Payments.CreatePaymentIntent, requestContent);

            System.Console.WriteLine($"CreatePaymentIntentAsync response: {response.StatusCode}");
            System.Console.WriteLine($"CreatePaymentIntentAsync response content: {await response.Content.ReadAsStringAsync()}");
            return await response.GetResponseAsync<string>(_logger);
        }

        public async Task<GenericPaymentIntent> GetPaymentIntentAsync(string id)
        {
            var requestUri = PaymentApiEndpoints.Payments.GetPaymentIntent(id);
            var response = await _httpClient.GetAsync(requestUri);

            System.Console.WriteLine($"GetPaymentIntentAsync response: {response.StatusCode}");
            System.Console.WriteLine($"GetPaymentIntentAsync response content: {await response.Content.ReadAsStringAsync()}");

            return await response.GetResponseAsync<GenericPaymentIntent>();
        }

    }
}
