using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis
{
    public class RedisCache : IRedisCache
    {
        #region Fields
        private readonly object _lock = new object();//used to put instance level lock; only one thread will execute code block per instance
        private readonly object _redisConnectLockObject = new object();
        private IDatabase _database;//lock don't work on this because it is being reassigned each time new connection requested; though _redis.GetDatabase() is thread safe and should be used to let mutiplexor manage connection for best performance. Considering these let's avoid putting lock on it
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly string _cacheIdentifier;
        #endregion

        #region Properties

        // ReSharper disable once BuiltInTypeReferenceStyle
        public Int64 Count
        {
            get
            {
                _database = _redisConnection.GetDatabase();
                lock (_lock)
                {
                    var count = _database.Multiplexer.GetEndPoints()
                        .Sum(endpoint => _database.Multiplexer.GetServer(endpoint).Keys(pattern: "*").LongCount());
                    return count;
                }
            }
        }

        #endregion

        #region Cotr
        public RedisCache(string config) : this(ConfigurationOptions.Parse(config)) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public RedisCache(ConfigurationOptions options)
        {
            _redisConnection = GetRedisConnection(options);
            _cacheIdentifier = "_EntitySetKey_";
        }

        public RedisCache(string config, string cacheIdentifier)
        {
            _redisConnection = GetRedisConnection(config);
            _cacheIdentifier = cacheIdentifier;
        }

        public RedisCache(ConfigurationOptions options, string cacheIdentifier)
        {
            _redisConnection = GetRedisConnection(options);
            _cacheIdentifier = cacheIdentifier;
        }

        #endregion

        public void InvalidateSets(IEnumerable<string> entitySets)
        {
            _database = _redisConnection.GetDatabase();

            var itemsToInvalidate = new HashSet<string>();

            lock (_lock)
            {
                try
                {
                    // ReSharper disable once PossibleMultipleEnumeration - the guard clause should not enumerate, its just checking the reference is not null
                    foreach (var entitySet in entitySets)
                    {
                        var entitySetKey = AddCacheQualifier(entitySet);
                        var keys = _database.SetMembers(entitySetKey).Select(v => v.ToString());
                        itemsToInvalidate.UnionWith(keys);
                        _database.KeyDelete(entitySetKey, CommandFlags.FireAndForget);
                    }
                }
                catch (Exception e)
                {
                    //OnCachingFailed(e);
                    return;
                }

                //foreach (var key in itemsToInvalidate)
                //{
                //    InvalidateItem(key);
                //}
            }
        }

        

        private RedisKey AddCacheQualifier(string entitySet) => string.Concat(_cacheIdentifier, ".", entitySet);

        private static string HashKey(string key)
        {
            //Looking up large Keys in Redis can be expensive (comparing Large Strings), so if keys are large, hash them, otherwise if keys are short just use as-is
            if (key.Length <= 128) return key;
            using (var sha = new SHA1CryptoServiceProvider())
            {
                key = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(key)));
                return key;
            }
        }

        private ConnectionMultiplexer GetRedisConnection(ConfigurationOptions options)
        {
            if (this._redisConnection != null && this._redisConnection.IsConnected)
            {
                return this._redisConnection;
            }

            lock (_redisConnectLockObject)
            {
                //释放连接
                _redisConnection?.Dispose();
                return ConnectionMultiplexer.Connect(options);
            }
        }

        private ConnectionMultiplexer GetRedisConnection(string config)
        {
            if (_redisConnection != null && _redisConnection.IsConnected)
            {
                return this._redisConnection;
            }

            lock (_redisConnectLockObject)
            {
                //释放连接
                _redisConnection?.Dispose();
                return ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(config));
            }
        }
    }
}
