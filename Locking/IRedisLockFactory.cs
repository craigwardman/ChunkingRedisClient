using StackExchange.Redis;

namespace ChunkingRedisClient.Locking
{
    public interface IRedisLockFactory
    {
        IRedisLock GetLockInstance(RedisKey key);
        void Use<T>() where T : IRedisLock;
    }
}