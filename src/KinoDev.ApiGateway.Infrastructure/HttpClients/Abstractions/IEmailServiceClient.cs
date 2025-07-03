namespace KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

public interface IEmailServiceClient
{
    Task<bool> SendEmailAsync(string email, string subject, string body);

    Task Up();
}
