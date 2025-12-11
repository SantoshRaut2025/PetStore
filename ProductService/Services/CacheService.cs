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
            // Try distributed cache first
            var cachedData = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(cachedData);
                }
                catch (JsonException jex)
                {
                    _logger.LogWarning(jex, "Failed to deserialize distributed cache value for key: {Key}", key);
                    // Fall through to memory cache attempt
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting from Redis (distributed cache) for key: {Key}. Falling back to in-memory cache.", key);
            // Fall back to memory cache below
        }

        // Fallback to in-memory cache
        try
        {
            if (_memoryCache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Returned value from in-memory cache for key: {Key}", key);
                return value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting from in-memory cache for key: {Key}", key);
        }

        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var distributedExpiration = expiration ?? TimeSpan.FromMinutes(10);
        var memoryExpiration = expiration ?? TimeSpan.FromMinutes(5);

        // Prepare distributed cache options
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = distributedExpiration
        };

        // Serialize once
        string serializedData;
        try
        {
            serializedData = JsonSerializer.Serialize(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing value for key: {Key}. Aborting cache set.", key);
            return;
        }

        // Try to set distributed cache; if it fails, fallback to in-memory cache.
        try
        {
            await _distributedCache.SetStringAsync(key, serializedData, options);
            _logger.LogInformation("Data cached in distributed cache (Redis) with key: {Key}", key);

            // Also populate in-memory cache for fast local access
            try
            {
                var memoryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = memoryExpiration
                };
                _memoryCache.Set(key, value, memoryOptions);
            }
            catch (Exception memEx)
            {
                _logger.LogWarning(memEx, "Failed to set in-memory cache after distributed cache set for key: {Key}", key);
            }

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting distributed cache for key: {Key}. Falling back to in-memory cache.", key);
        }

        // Fallback: set in-memory cache
        try
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = memoryExpiration
            };
            _memoryCache.Set(key, value, cacheOptions);
            _logger.LogInformation("Data cached in in-memory cache with key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting in-memory cache for key: {Key}", key);
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