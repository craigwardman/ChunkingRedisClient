using System.Threading.Tasks;
using StackExchange.Redis;

namespace JsonRedis.Client.Deleter
{
    public interface IRedisDeleter<in TKey, TItem>
    {
        Task<bool> DeleteAsync(IDatabaseAsync redisDatabase, TKey key);
    }
}