namespace KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings
{
    public class MessageBrokerSettings
    {
        public Queues Queues { get; set; }
    }

    public class Queues
    {
        public string OrderCompleted { get; set; }
    }
}