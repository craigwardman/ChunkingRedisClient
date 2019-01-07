using StackExchange.Redis;

namespace JsonRedis.Locking
{
    public interface IRedisLockFactory
    {
        IRedisLock GetLockInstance(RedisKey key);
        void Use<T>() where T : IRedisLock;
    }
}