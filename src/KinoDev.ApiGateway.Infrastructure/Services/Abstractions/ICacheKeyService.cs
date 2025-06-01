namespace KinoDev.ApiGateway.Infrastructure.Services.Abstractions;

public interface ICacheKeyService
{
    string GetCacheKey(string prefix, params string[] keys);
}