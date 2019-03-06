using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChunkingRedisClient.Keys;
using ChunkingRedisClient.Locking;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Deleter
{
    public class ChunkedDeleter<TKey, TItem> : IRedisDeleter<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;
        private readonly IRedisLockFactory _lockFactory;

        public ChunkedDeleter(IKeygen<TKey> keygen, IRedisLockFactory lockFactory)
        {
            _keygen = keygen ?? throw new ArgumentNullException(nameof(keygen));
            _lockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
        }

        public async Task<bool> DeleteAsync(IDatabaseAsync redisDatabase, TKey key)
        {
            var headerKey = _keygen.GetKey(key);
            var chunkDeletes = new List<Task<bool>>();

            using (var redisLock = _lockFactory.GetLockInstance(headerKey))
            {
                await redisLock.AcquireAsync().ConfigureAwait(false);

                var header = await redisDatabase.StringGetAsync(headerKey).ConfigureAwait(false);
                if (header.IsNullOrEmpty)
                {
                    return false;
                }

                var chunkCount = (int)header;
                for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
                {
                    var chunkKey = _keygen.GetKey(key, chunkIndex.ToString());

                    chunkDeletes.Add(redisDatabase.KeyDeleteAsync(chunkKey));
                }

                await Task.WhenAll(chunkDeletes).ConfigureAwait(false);
                return await redisDatabase.KeyDeleteAsync(headerKey).ConfigureAwait(false);
            }
        }
    }
}