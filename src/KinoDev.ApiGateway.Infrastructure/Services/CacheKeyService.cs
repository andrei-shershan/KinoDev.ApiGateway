namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public interface ICacheKeyService
    {
        string GetCacheKey(string prefix, params string[] keys);
    }

    public class CacheKeyService : ICacheKeyService
    {
        public string GetCacheKey(string prefix, params string[] keys)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));
            }

            if (keys == null || keys.Length == 0)
            {
                throw new ArgumentException("At least one key must be provided.", nameof(keys));
            }

            return $"{prefix}_{string.Join("_", keys)}";
        }
    }
}