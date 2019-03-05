using System;
using System.Threading.Tasks;

namespace ChunkingRedisClient.Client
{
    public interface IRedisClient<in TKey, TItem>
    {
        Task<bool> DeleteAsync(TKey key);
        Task<TItem> GetAsync(TKey key, TimeSpan? resetExpiryTimeSpan = null);
        Task<bool> SetAsync(TKey key, TItem item, TimeSpan? expiryTimeSpan = null);
    }
}