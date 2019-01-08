using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChunkingRedisClient.Keys;
using ChunkingRedisClient.Locking;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Reader
{
    public class ChunkedJsonRedisReader<TKey, TItem> : IRedisReader<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;
        private readonly IRedisLockFactory _lockFactory;

        public ChunkedJsonRedisReader(IKeygen<TKey> keygen, IRedisLockFactory lockFactory)
        {
            _keygen = keygen ?? throw new ArgumentNullException(nameof(keygen));
            _lockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
        }

        public async Task<TItem> ReadAsync(IDatabaseAsync redisDatabase, TKey key, TimeSpan? resetExpiry)
        {
            var headerKey = _keygen.GetKey(key);
            var chunkGets = new List<Task<RedisValue>>();

            using (var redisLock = _lockFactory.GetLockInstance(headerKey))
            {
                await redisLock.AcquireAsync().ConfigureAwait(false);


                var header = await GetAndUpdateExpiry(redisDatabase, headerKey, resetExpiry).ConfigureAwait(false);
                if (header.IsNullOrEmpty)
                {
                    return default(TItem);
                }


                var totalChunks = (int)header;

                for (var chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
                {
                    var chunkKey = _keygen.GetKey(key, chunkIndex.ToString());

                    chunkGets.Add(GetAndUpdateExpiry(redisDatabase, chunkKey, resetExpiry));
                }

                await Task.WhenAll(chunkGets).ConfigureAwait(false);
            }

            var jsonData = string.Join("", chunkGets.Select(cg => cg.Result));

            return string.IsNullOrEmpty(jsonData) ? default(TItem) : JsonConvert.DeserializeObject<TItem>(jsonData);
        }

        private static async Task<RedisValue> GetAndUpdateExpiry(IDatabaseAsync connection, RedisKey key, TimeSpan? resetExpiry)
        {
            if (resetExpiry.HasValue)
            {
                await connection.KeyExpireAsync(key, resetExpiry.Value, CommandFlags.FireAndForget).ConfigureAwait(false);
            }

            return await connection.StringGetAsync(key).ConfigureAwait(false);
        }
    }
}