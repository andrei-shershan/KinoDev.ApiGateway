using System.Text;
using KinoDev.ApiGateway.Infrastructure.Constants;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients
{
    public interface IPaymentClient
    {
        Task<string> CreatePaymentIntentAsync(int amount, string currency, Dictionary<string, string> metadata);
    }

    public class PaymentClient : IPaymentClient
    {
        private readonly HttpClient _httpClient;

        public PaymentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreatePaymentIntentAsync(int amount, string currency, Dictionary<string, string> metadata)
        {
            System.Console.WriteLine("!!!!!!!!!! " + JsonConvert.SerializeObject(new { amount, currency, metadata }));
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { amount, currency, metadata }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(PaymentApiEndpoints.Payments.CreatePaymentIntent, requestContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return await response.GetResponseAsync<string>();
        }
    }
}
