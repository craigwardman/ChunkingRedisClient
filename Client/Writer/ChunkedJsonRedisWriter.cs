using System;
using System.Threading.Tasks;
using ChunkingRedisClient.Keys;
using ChunkingRedisClient.Locking;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Writer
{
    public class ChunkedJsonRedisWriter<TKey, TItem> : IRedisWriter<TKey, TItem>
    {
        private readonly IKeygen<TKey> _keygen;
        private readonly IRedisLockFactory _lockFactory;
        private readonly int _chunkSize;

        public ChunkedJsonRedisWriter(IKeygen<TKey> keygen, IRedisLockFactory lockFactory, int chunkSize)
        {
            _keygen = keygen ?? throw new ArgumentNullException(nameof(keygen));
            _lockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
            _chunkSize = chunkSize > 0 ? chunkSize : 10240;
        }

        public async Task<bool> WriteAsync(IDatabaseAsync redisDatabase, TKey key, TItem item, TimeSpan? expiry)
        {
            var headerKey = _keygen.GetKey(key);
            var chunkIndex = 0;
            Task<bool> previousWriteTask = null;

            using (var redisLock = _lockFactory.GetLockInstance(headerKey))
            {
                await redisLock.AcquireAsync().ConfigureAwait(false);

                using (var bufferedWriter = new BufferedTextWriter(_chunkSize))
                {
                    bufferedWriter.BufferFull += async (s, e) =>
                    {
                        var bufferContent = e.Buffer.ToString();
                        var myChunkIndex = chunkIndex++;

                        e.Buffer.Clear();

                        if (previousWriteTask != null)
                        {
                            await previousWriteTask.ConfigureAwait(false);
                        }

                        previousWriteTask = WriteToRedisAsync(redisDatabase, key, expiry, bufferContent, myChunkIndex);
                    };

                    JsonSerializer.CreateDefault().Serialize(bufferedWriter, item);

                    if (bufferedWriter.Buffer.Length > 0)
                    {
                        if (previousWriteTask != null)
                        {
                            await previousWriteTask.ConfigureAwait(false);
                            previousWriteTask = null;
                        }

                        await WriteToRedisAsync(redisDatabase, key, expiry, bufferedWriter.Buffer.ToString(), chunkIndex++).ConfigureAwait(false);
                    }
                }

                return await redisDatabase.StringSetAsync(headerKey, chunkIndex, expiry).ConfigureAwait(false);
            }
        }

        private Task<bool> WriteToRedisAsync(IDatabaseAsync redisDatabase, TKey key, TimeSpan? expiry, string bufferContent, int chunkIndex)
        {
            return redisDatabase.StringSetAsync(
                    _keygen.GetKey(key, chunkIndex.ToString()),
                    bufferContent,
                    expiry);
        }
    }
}