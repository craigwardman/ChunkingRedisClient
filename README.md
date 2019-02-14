# Chunking Redis Client
A C#/.NET Core library which wraps the StackExchange.Redis client, specifically using JSON serialisation, and adds functionality such as chunked reading/writing and sliding expiration.

To install without the source, use the NuGet package:
https://www.nuget.org/packages/ChunkingRedisClient/

The purpose of this library is to create a re-usable library of code for wrapping the StackExchange.RedisClient and solving the issues I usually need to solve.

Those being:

* IoC wrappers/abstractions<br/>
   - Just take your dependency on `IRedisClient<TKey, TItem>`<br/>
   - By default you should configure your DI container to inject the provided RedisClient<TKey, TItem><br/>
   - Since IoC is used throughout you also need to configure:<br/>
     ~ `IRedisWriter<TKey, Item>` -> JsonRedisWriter or ChunkedJsonRedisWriter<br/>
     ~ `IRedisReader<TKey, Item>` -> JsonRedisReader or ChunkedJsonRedisReader<br/>
     ~ `IRedisWriter<TKey, Item>` -> JsonRedisDeleter or ChunkedJsonRedisDeleter<br/>
     (note: for one combination of TKey, TItem - ensure the decision to chunk or not is consistent)<br/>
     ~ `IKeygen<TKey>` to an object specific implementation, like GuidKeygen<br/>
     ~ For chunking, locking is required:<br/>
             IRedisLockFactory -> RedisLockFactory<br/>
             To override the default of InMemoryRedisLock, call `RedisLockFactory.Use<IRedisLock>() <-- your class here`<br/>
     
* Strongly typed access to the cache<br/>
  - Use any C# object as your TKey and TItem, given that:<br/>
      ~ You can implement your own Keygen for TKey<br/>
      ~ Your TItem is serialisable by Newtonsoft.Json<br/>
      
* Implementing the StackExchange Connection Multiplexer<br/>
  - This is handled by the RedisDatabaseFactory<br/>
  - Not using the usual `Lazy<ConnectionMulitplexer>` approach, as I want to support one multiplexer per connection string (if your app is dealing with more than 1 cache)<br/>
  - The multiplexers are stored in a concurrent dictionary where the connection string is the key<br/>
  - The multiplexer begins connecting asynchronously on first use<br/>
    
* Sliding expiration of cache keys<br/>
  - Pass in the optional timespan to read methods if you want to use sliding expiration<br/>
  - This updates the expiry when you read the item, so that keys which are still in use for read purposes live longer<br/>
  
* Chunked JSON data<br/>
  - This solves a performance issue whereby Redis does not perform well with large payloads.<br/>
  - Sometimes you may also have had errors from the server when the queue is full.<br/>
  - The default chunk size is 10KB which can be configured in the ChunkedJsonRedisWriter<br/>
  - The JSON data is streamed from Newtonsoft into a buffer. Every time the buffer is full it is written to Redis under the main cache key with a suffix of "chunkIndex"<br/>
  - The main cache key is then written to contain the count of chunks, which is used by the reader and deleter.<br/>
  
* Generating keys for objects<br/>
  - I don't like using bytes for keys as they are not human readable, so I like to generate unique strings<br/>
  - There is no none-intrusive way of providing a type agnostic generic keygen, therefore you must write your own. If you write something for a CLR type, considering contributing it to the project!
  - Since we know Guids are unique, I have demonstrated the ability to create custom keygens.<br/>


The code can be extended to support other serialisation types (TODO), distributed locks (TODO), different ways of generating keys or whatever you need it to do.
