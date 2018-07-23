using System;
using System.IO;
using StackExchange.Redis;
using System.Runtime.Serialization.Formatters.Binary;
using Config.Infrastructure;
using Config.Infrastructure.Models;

namespace ConfigLib
{
    public class ConfigurationReader : IDisposable
    {
        readonly string _appName;
        readonly ConnectionMultiplexer _redis;
        readonly IDatabase _database;
        readonly ISubscriber _bus;
        readonly Event _event;

        public ConfigurationReader(string applicationName, string connectionString, double refreshTimerIntervalInMs = 30_000)
        {
            _appName = applicationName;
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
            _bus = _redis.GetSubscriber();
            _event = new Event(applicationName, refreshTimerIntervalInMs);

            _bus.Publish("hostChannel", _event);
        }

        public T GetValue<T>(string key)
        {
            var value = _database.StringGet($"{_appName}:{key}");
            return Formatter.FromByteArray<T>(value);
        }

        public void Dispose()
        {
            _event.Type = EventType.Disposing;
            _bus.Publish("hostChannel", _event);
            _redis.Dispose();
        }
    }
}
