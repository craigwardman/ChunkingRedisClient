using System.Collections.Concurrent;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChunkingRedisClient.Database
{
    internal static class RedisDatabaseFactory
    {
        private static readonly ConcurrentDictionary<string, Task<ConnectionMultiplexer>> ConnectionMultiplexers = new ConcurrentDictionary<string, Task<ConnectionMultiplexer>>();

        public static async Task<IDatabase> GetRedisDatabaseAsync(string connectionString)
        {
            var connectionMultiplexer = ConnectionMultiplexers.GetOrAdd(connectionString, GetConnectionMuliplexerAsync);

            return (await connectionMultiplexer.ConfigureAwait(false)).GetDatabase();
        }

        private static Task<ConnectionMultiplexer> GetConnectionMuliplexerAsync(string connectionString)
        {
            return ConnectionMultiplexer.ConnectAsync(connectionString);
        }
    }
}