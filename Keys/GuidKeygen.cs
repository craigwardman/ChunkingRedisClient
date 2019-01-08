using System;
using StackExchange.Redis;

namespace ChunkingRedisClient.Keys
{
    public class GuidKeygen : IKeygen<Guid>
    {
        public RedisKey GetKey(Guid input, string suffix = null)
        {
            return $"{input}:{suffix}";
        }
    }
}