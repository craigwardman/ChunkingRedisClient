using System;
using ChunkingRedisClient.Client;
using ChunkingRedisClient.Client.Deleter;
using ChunkingRedisClient.Client.Reader;
using ChunkingRedisClient.Client.Writer;
using ChunkingRedisClient.Keys;
using ChunkingRedisClient.Locking;
using Microsoft.Extensions.DependencyInjection;

namespace ChunkingRedisClient
{
    public static class MicrosoftDiExtensions
    {
        public static void AddRedisClient<TKey, TItem>(this IServiceCollection serviceCollection, int? chunkSize)
        {
            serviceCollection.AddSingleton<IKeygen<Guid>, GuidKeygen>(); // user to manually register any other keygen classes
            serviceCollection.AddSingleton<IRedisLockFactory, RedisLockFactory>();

            if (chunkSize.GetValueOrDefault(0) is int chunk && chunk > 0)
            {
                serviceCollection.AddScoped<IRedisReader<TKey, TItem>, ChunkedJsonRedisReader<TKey, TItem>>();
                serviceCollection.AddScoped<IRedisDeleter<TKey, TItem>, ChunkedDeleter<TKey, TItem>>();
                serviceCollection.AddScoped(provider => new ChunkedJsonRedisWriter<TKey, TItem>(
                    provider.GetRequiredService<IKeygen<TKey>>(),
                    provider.GetRequiredService<IRedisLockFactory>(),
                    chunk));
            }
            else
            {
                serviceCollection.AddScoped<IRedisReader<TKey, TItem>, JsonRedisReader<TKey, TItem>>();
                serviceCollection.AddScoped<IRedisDeleter<TKey, TItem>, StandardDeleter<TKey, TItem>>();
                serviceCollection.AddScoped<IRedisWriter<TKey, TItem>, JsonRedisWriter<TKey, TItem>>();
            }

            serviceCollection.AddScoped<IRedisClient<TKey, TItem>, RedisClient<TKey, TItem>>(); // user to define IRedisClientConfig impl.
        }
    }
}