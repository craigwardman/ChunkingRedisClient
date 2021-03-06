﻿using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChunkingRedisClient.Client.Writer
{
    public interface IRedisWriter<in TKey, in TItem>
    {
        Task<bool> WriteAsync(IDatabaseAsync redisDatabase, TKey key, TItem item, TimeSpan? expiry);
    }
}