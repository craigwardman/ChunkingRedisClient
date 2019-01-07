using System;
using System.Threading.Tasks;
using JsonRedis.Keys;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JsonRedis.Client.Writer
{
    public class JsonRedisWriter<TKey, TItem> : IRedisWriter<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;

        public JsonRedisWriter(IKeygen<TKey> keygen)
        {
            _keygen = keygen;
        }

        public Task<bool> WriteAsync(IDatabaseAsync redisDatabase, TKey key, TItem item, TimeSpan? expiry)
        {
            var redisValue = JsonConvert.SerializeObject(item);
            return redisDatabase.StringSetAsync(_keygen.GetKey(key), redisValue, expiry);
        }
    }
}