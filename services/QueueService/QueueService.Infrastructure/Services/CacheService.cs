using Microsoft.Extensions.Caching.Memory;
using QueueService.Application.Common.Interfaces;
using System.Text.Json;

namespace QueueService.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public async Task UpdateQueueCacheAsync(Guid department)
        {
            var cacheKey = $"queue:{department}";
            // var queueData = await _queueRepository.GetWaitingPatientsByDepartmentAsync(department);
            // await SetAsync(cacheKey, queueData, TimeSpan.FromMinutes(15));
        }
    }
}