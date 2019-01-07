using System;
using System.Threading.Tasks;

namespace JsonRedis.Locking
{
    public interface IRedisLock : IDisposable
    {
        Task AcquireAsync();
        void Release();
    }
}