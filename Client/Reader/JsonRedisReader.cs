using System;
using System.Threading.Tasks;
using JsonRedis.Keys;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JsonRedis.Client.Reader
{
    public class JsonRedisReader<TKey, TItem> : IRedisReader<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;

        public JsonRedisReader(IKeygen<TKey> keygen)
        {
            _keygen = keygen;
        }

        public async Task<TItem> ReadAsync(IDatabaseAsync redisDatabase, TKey key, TimeSpan? resetExpiry)
        {
            var redisKey = _keygen.GetKey(key);
            var item = await redisDatabase.StringGetAsync(redisKey).ConfigureAwait(false);

            if (resetExpiry.HasValue)
            {
                await redisDatabase.KeyExpireAsync(redisKey, resetExpiry.Value, CommandFlags.FireAndForget).ConfigureAwait(false);
            }

            return string.IsNullOrEmpty(item) ? default(TItem) : JsonConvert.DeserializeObject<TItem>(item);
        }
    }
}