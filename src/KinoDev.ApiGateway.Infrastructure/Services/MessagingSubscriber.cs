using KinoDev.Shared.Services;
using Microsoft.Extensions.Hosting;

namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public class MessagingSubscriber : BackgroundService
    {
        private readonly IMessageBrokerService _messageBrokerService;

        public MessagingSubscriber(IMessageBrokerService messageBrokerService)
        {
            _messageBrokerService = messageBrokerService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // return  _messageBrokerService.SubscribeAsync("order-completed", "queue-api-gateway", async (message) =>
            // {
            //     // Handle the message here. For example, you can log it or process it.
            // });

            return Task.CompletedTask;
        }
    }
}