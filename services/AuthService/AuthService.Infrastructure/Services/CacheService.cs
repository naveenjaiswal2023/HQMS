using HQMS.QueueService.Application.Common.Interfaces;
using HQMS.QueueService.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HQMS.QueueService.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IQueueItemRepository _queueRepository;

        public CacheService(IDistributedCache cache, IQueueItemRepository queueRepository)
        {
            _cache = cache;
            _queueRepository = queueRepository;
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var cached = await _cache.GetStringAsync(key);
            if (cached == null)
                return null;

            return JsonSerializer.Deserialize<T>(cached);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiry.HasValue)
                options.SetAbsoluteExpiration(expiry.Value);
            else
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task UpdateQueueCacheAsync(Guid department)
        {
            var queueData = await _queueRepository.GetWaitingPatientsByDepartmentAsync(department);
            var cacheKey = $"queue:{department}";
            await SetAsync(cacheKey, queueData, TimeSpan.FromMinutes(15));
        }

    }
}
