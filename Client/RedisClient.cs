using System;
using System.Threading.Tasks;
using JsonRedis.Client.Deleter;
using JsonRedis.Client.Reader;
using JsonRedis.Client.Writer;
using JsonRedis.Database;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace JsonRedis.Client
{
    public class RedisClient<TKey, TItem> : IRedisClient<TKey, TItem>
    {
        private readonly string _connectionString;
        private readonly IRedisWriter<TKey, TItem> _redisWriter;
        private readonly IRedisReader<TKey, TItem> _redisReader;
        private readonly IRedisDeleter<TKey, TItem> _redisDeleter;
        private readonly string _keyPrefix;
        private IDatabase _prefixedDatabase;

        public RedisClient(string connectionString, IRedisWriter<TKey, TItem> redisWriter, IRedisReader<TKey, TItem> redisReader, IRedisDeleter<TKey, TItem> redisDeleter)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _redisWriter = redisWriter ?? throw new ArgumentNullException(nameof(redisWriter));
            _redisReader = redisReader ?? throw new ArgumentNullException(nameof(redisReader));
            _redisDeleter = redisDeleter ?? throw new ArgumentNullException(nameof(redisDeleter));

            _keyPrefix = $"{typeof(TItem).Namespace}.{typeof(TItem).Name}::";
        }

        public Task<bool> SetAsync(TKey key, TItem item, TimeSpan? expiryTimeSpan = null)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (item == null) throw new ArgumentNullException(nameof(item));

            return SetAsyncInternal(key, item, expiryTimeSpan);
        }

        public Task<TItem> GetAsync(TKey key, TimeSpan? resetExpiryTimeSpan = null)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return GetAsyncInternal(key, resetExpiryTimeSpan);
        }

        public Task<bool> DeleteAsync(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return DeleteAsyncInternal(key);
        }

        private async Task<bool> DeleteAsyncInternal(TKey key)
        {
            var connection = await GetKeyspacedDatabaseAsync().ConfigureAwait(false);

            return await _redisDeleter.DeleteAsync(connection, key).ConfigureAwait(false);
        }

        private async Task<TItem> GetAsyncInternal(TKey key, TimeSpan? resetExpiryTimeSpan)
        {
            var connection = await GetKeyspacedDatabaseAsync().ConfigureAwait(false);

            return await _redisReader.ReadAsync(connection, key, resetExpiryTimeSpan).ConfigureAwait(false);
        }

        private async Task<bool> SetAsyncInternal(TKey key, TItem item, TimeSpan? expiryTimeSpan)
        {
            var connection = await GetKeyspacedDatabaseAsync().ConfigureAwait(false);

            return await _redisWriter.WriteAsync(connection, key, item, expiryTimeSpan).ConfigureAwait(false);
        }

        private async Task<IDatabase> GetKeyspacedDatabaseAsync()
        {
            if (_prefixedDatabase == null)
            {
                _prefixedDatabase = (await RedisDatabaseFactory
                        .GetRedisDatabaseAsync(_connectionString)
                        .ConfigureAwait(false))
                    .WithKeyPrefix(_keyPrefix);
            }

            return _prefixedDatabase;
        }
    }
}