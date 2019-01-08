using StackExchange.Redis;

namespace ChunkingRedisClient.Keys
{
    public class GenericKeygen<TInput> : IKeygen<TInput>
    {
        public RedisKey GetKey(TInput input, string suffix = null)
        {
            return $"{input}:{input.GetHashCode()}:{suffix}";
        }
    }
}