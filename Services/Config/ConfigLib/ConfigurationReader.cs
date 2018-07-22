using System;
using System.IO;
using StackExchange.Redis;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConfigLib
{
    public class ConfigurationReader : IDisposable
    {
        readonly string _appName;
        readonly ConnectionMultiplexer _redis;
        readonly IDatabase _database;
        readonly BinaryFormatter _binaryFormatter;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs = 30_000)
        {
            _binaryFormatter = new BinaryFormatter();
            _appName = applicationName;
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public T GetValue<T>(string key)
        {
            return FromByteArray<T>(_database.StringGet($"{_appName}:{key}"));
        }

        private T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using (var ms = new MemoryStream(data))
            {
                return (T)_binaryFormatter.Deserialize(ms);
            }
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
