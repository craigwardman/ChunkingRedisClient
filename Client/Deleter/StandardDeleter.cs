using System;
using System.Threading.Tasks;
using JsonRedis.Keys;
using StackExchange.Redis;

namespace JsonRedis.Client.Deleter
{
    public class StandardDeleter<TKey, TItem> : IRedisDeleter<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;

        public StandardDeleter(IKeygen<TKey> keygen)
        {
            _keygen = keygen ?? throw new ArgumentNullException(nameof(keygen));
        }

        public Task<bool> DeleteAsync(IDatabaseAsync redisDatabase, TKey key)
        {
            return redisDatabase.KeyDeleteAsync(_keygen.GetKey(key));
        }
    }
}