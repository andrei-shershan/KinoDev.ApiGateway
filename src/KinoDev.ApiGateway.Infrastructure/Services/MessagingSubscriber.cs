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
            System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            return  _messageBrokerService.SubscribeAsync("order-completed", async (message) =>
            {
                // Handle the message here. For example, you can log it or process it.
                Console.WriteLine($"Received message: {message}");
                System.Console.WriteLine("**********************");
            });
        }
    }
}