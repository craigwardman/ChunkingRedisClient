using System;
using StackExchange.Redis;

namespace JsonRedis.Keys
{
    public class GuidKeygen : IKeygen<Guid>
    {
        public RedisKey GetKey(Guid input, string suffix = null)
        {
            return $"{input}:{suffix}";
        }
    }
}