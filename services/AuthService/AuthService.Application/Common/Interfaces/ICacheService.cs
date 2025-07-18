﻿namespace AuthService.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
        //Task UpdateCacheAsync(Guid department);
    }
}
