using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Deleter
{
    public interface IRedisDeleter<in TKey, TItem>
    {
        Task<bool> DeleteAsync(IDatabaseAsync redisDatabase, TKey key);
    }
}