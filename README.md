# JsonRedis
A library which wraps the StackExchange.Redis client, specifically using JSON serialisation, and adds functionality such as chunked reading/writing and sliding expiration.

The purpose of this library is to create a re-usable library of code (NB. which I need to put into a NuGet package) for wrapping the StackExchange.RedisClient and solving the issues I usually need to solve.

Those being:

* IoC wrappers/abstractions
   - Just take your dependency on "IRedisClient<TKey, TItem>"
   - By default you should configure your DI container to inject the provided RedisClient<TKey, TItem>
   - Since IoC is used throughout you also need to configure:
     ~ IRedisWriter<TKey, Item> -> JsonRedisWriter or ChunkedJsonRedisWriter
     ~ IRedisReader<TKey, Item> -> JsonRedisReader or ChunkedJsonRedisReader
     ~ IRedisWriter<TKey, Item> -> JsonRedisDeleter or ChunkedJsonRedisDeleter
     (note: for one combination of TKey, TItem - ensure the decision to chunk or not is consistent)
     ~ For chunking, locking is required:
             IRedisLockFactory -> RedisLockFactory
             To override the default of InMemoryRedisLock, call RedisLockFactory.Use<IRedisLock>() <-- your class here
     ~ IKeygen<TKey> to GenericKeygen<TKey>, or implement a specific one like GuidKeygen
     
* Strongly typed access to the cache
  - Use any C# object as your TKey and TItem, given that:
      ~ Your TKey is unique by GetHashCode(), or implement your own Keygen
      ~ Your TItem is serialisable by Newtonsoft.Json
      
* Implementing the StackExchange Connection Multiplexer
  - This is handled by the RedisDatabaseFactory
  - Not using the usual "Lazy<ConnectionMulitplexer>" approach, as I want to support one multiplexer per connection string (if your app is dealing with more than 1 cache)
  - The multiplexers are stored in a concurrent dictionary where the connection string is the key
  - The multiplexer begins connecting asynchronously on first use
    
* Sliding expiration of cache keys
  - Pass in the optional timespan to read methods if you want to use sliding expiration
  - This updates the expiry when you read the item, so that keys which are still in use for read purposes live longer
  
* Chunked JSON data
  - This solves a performance issue whereby Redis does not perform well with large payloads.
  - The default chunk size is 10KB which can be configured in the ChunkedJsonRedisWriter
  - The JSON data is streamed from Newtonsoft into a buffer. Every time the buffer is full it is written to Redis under the main cache key with a suffix of "chunkIndex"
  - The main cache key is then written to contain the count of chunks, which is used by the reader and deleter.
  
* Generating keys for objects
  - I don't like using bytes for keys as they are not human readable, so I like to generate unique strings
  - I have provided a default GenericKeygen which uses a combination of "ToString()" and "GetHashCode()" to generate a human readable yet unique key.
  - Since we know Guids are unique, I have demonstrated the ability to create custom keygens which omits GetHashCode in this case.


The code can be extended to support other serialisation types (TODO), distributed locks (TODO), different ways of generating keys or whatever you need it to do.
