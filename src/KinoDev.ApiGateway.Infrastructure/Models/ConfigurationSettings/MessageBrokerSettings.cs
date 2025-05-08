namespace KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings
{
    public class MessageBrokerSettings
    {
        public Topics Topics { get; set; }
    }

    public class Topics
    {
        public string OrderCompleted { get; set; }
    }
}