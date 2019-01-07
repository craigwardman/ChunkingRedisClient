using System;
using System.Linq.Expressions;
using System.Reflection;
using StackExchange.Redis;

namespace JsonRedis.Locking
{
    public class RedisLockFactory : IRedisLockFactory
    {
        private ConstructorInfo _lockTypeCtor;

        public RedisLockFactory()
        {
            Use<InMemoryRedisLock>();
        }

        public void Use<T>() where T : IRedisLock
        {
            foreach (var ctor in typeof(T).GetConstructors())
            {
                var ctorParams = ctor.GetParameters();
                if (ctorParams.Length == 1 && ctorParams[0].ParameterType == typeof(RedisKey))
                {
                    _lockTypeCtor = ctor;
                    return;
                }
            }

            throw new InvalidOperationException($"Cannot use type {typeof(T)} in RedisLockFactory. The type must inherit IRedisLock and have a public constructor taking a single argument of type RedisKey.");
        }

        public IRedisLock GetLockInstance(RedisKey key)
        {
            LambdaExpression lambda =
                Expression.Lambda<Func<IRedisLock>>(Expression.New(_lockTypeCtor, Expression.Constant(key)));

            return ((Func<IRedisLock>)lambda.Compile())();
        }
    }
}