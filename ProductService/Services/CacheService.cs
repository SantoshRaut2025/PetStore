using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace ProductService.Services;
public class CacheService(IMemoryCache memoryCache,
IDistributedCache distributedCache, 
ILogger<CacheService> logger) 
: ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly ILogger<CacheService> _logger = logger;

    // Implementation of asynchronous methods
    public async Task<T?> GetAsync<T>(string key)
    {
       try
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting from Redis cache for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(value);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            await _distributedCache.SetStringAsync(key, serializedData, options);
            _logger.LogInformation("Data cached in Redis with key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting Redis cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
         try
        {
            await _distributedCache.RemoveAsync(key);
            _logger.LogInformation("Cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing Redis cache for key: {Key}", key);
        }
    }

    // In-Memory Cache Methods (backward compatibility)
    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };
        _memoryCache.Set(key, value, cacheOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}