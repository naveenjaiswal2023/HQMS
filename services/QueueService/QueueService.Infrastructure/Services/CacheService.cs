using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using QueueService.Application.Common.Interfaces;
using System.Text.Json;

namespace QueueService.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly bool _useDistributedCache;

        public CacheService(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;

            // Read from appsettings.json: CacheSettings:UseDistributedCache = true/false
            _useDistributedCache = configuration.GetValue<bool>("CacheSettings:UseDistributedCache");
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_useDistributedCache)
            {
                var jsonData = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(jsonData))
                    return default;

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            else
            {
                _memoryCache.TryGetValue(key, out T? value);
                return value;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (_useDistributedCache)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
                };

                var jsonData = JsonSerializer.Serialize(value);
                await _distributedCache.SetStringAsync(key, jsonData, options);
            }
            else
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
                };

                _memoryCache.Set(key, value, options);
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (_useDistributedCache)
            {
                await _distributedCache.RemoveAsync(key);
            }
            else
            {
                _memoryCache.Remove(key);
            }
        }
    }
}
