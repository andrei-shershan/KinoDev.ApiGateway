namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public interface IMessageBrokerService
    {
        Task PublishAsync(object data, string subscription, string key = "");

        Task SubscribeAsync(string subscription, Func<string, Task> callback, string key = "");
    }    
}