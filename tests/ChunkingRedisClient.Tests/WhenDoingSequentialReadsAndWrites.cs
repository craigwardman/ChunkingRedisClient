using ChunkingRedisClient.Client;
using ChunkingRedisClient.Client.Deleter;
using ChunkingRedisClient.Client.Reader;
using ChunkingRedisClient.Client.Writer;
using ChunkingRedisClient.Keys;
using ChunkingRedisClient.Locking;
using ChunkingRedisClient.Tests.TestObjects;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChunkingRedisClient.Tests
{
    public class WhenDoingSequentialReadsAndWrites
    {
        [Fact]
        public async Task CanWriteAndReadOneItem()
        {
            var keyGen = new GuidKeygen();
            var redisClient =
                new RedisClient<Guid, Product>(
                    new RedisClientConfig
                    {
                        ConnectionString = "my-redis:6379"
                    },
                    new ChunkedJsonRedisWriter<Guid, Product>(
                        keyGen, new RedisLockFactory(), 0),
                    new ChunkedJsonRedisReader<Guid, Product>(
                        keyGen, new RedisLockFactory()),
                    new ChunkedDeleter<Guid, Product>(
                        keyGen, new RedisLockFactory()));

            var product = TestDataGenerator.Generate(1).First();

            var guid = Guid.NewGuid();
            await redisClient.SetAsync(guid, product);
            var item = await redisClient.GetAsync(guid);
            item.Should().NotBeNull();
        }
    }
}