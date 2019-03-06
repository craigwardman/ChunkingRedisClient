using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChunkingRedisClient.Locking
{
    internal class InMemoryRedisLock : IRedisLock
    {
        private static readonly ConcurrentDictionary<RedisKey, SemaphoreSlim> KeyLocks = new ConcurrentDictionary<RedisKey, SemaphoreSlim>();
        private readonly SemaphoreSlim _mySemaphoreSlim;
        private bool _isLockAcquired;

        public InMemoryRedisLock(RedisKey key)
        {
            _mySemaphoreSlim = KeyLocks.GetOrAdd(key, redisKey => new SemaphoreSlim(1, 1));
        }

        public async Task AcquireAsync()
        {
            if (!_isLockAcquired)
            {
                await _mySemaphoreSlim.WaitAsync().ConfigureAwait(false);
                _isLockAcquired = true;
            }
        }

        public void Release()
        {
            if (_isLockAcquired)
            {
                _mySemaphoreSlim.Release();
                _isLockAcquired = false;
            }
        }

        public void Dispose()
        {
            Release();
        }
    }
}