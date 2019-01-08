using StackExchange.Redis;

namespace ChunkingRedisClient.Keys
{
    public interface IKeygen<in TInput>
    {
        RedisKey GetKey(TInput input, string suffix = null);
    }
}