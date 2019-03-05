using System;
using System.Threading.Tasks;

namespace ChunkingRedisClient.Locking
{
    public interface IRedisLock : IDisposable
    {
        Task AcquireAsync();
        void Release();
    }
}