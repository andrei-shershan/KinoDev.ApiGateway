namespace KinoDev.ApiGateway.Infrastructure.Services.Abstractions;

public interface ICacheProvider
{
    T? Get<T>(string key);
    void Set<T>(string key, T item, TimeSpan expiration);
}