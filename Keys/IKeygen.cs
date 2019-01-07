using StackExchange.Redis;

namespace JsonRedis.Keys
{
    public interface IKeygen<in TInput>
    {
        RedisKey GetKey(TInput input, string suffix = null);
    }
}