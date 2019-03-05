using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Reader
{
    public interface IRedisReader<in TKey, TItem>
    {
        Task<TItem> ReadAsync(IDatabaseAsync redisDatabase, TKey key, TimeSpan? resetExpiry);
    }
}