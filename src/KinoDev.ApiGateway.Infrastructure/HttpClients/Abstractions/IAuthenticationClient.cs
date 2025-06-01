using KinoDev.ApiGateway.Infrastructure.Models;

namespace KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

public interface IAuthenticationClient
{
    Task<TokenModel> GetClientTokenAsync();
}