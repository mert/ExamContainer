using System;
using Config.Infrastructure.Models;
using MongoDB.Driver;

namespace Config.Infrastructure
{
    public class ConfigContext
    {
        private readonly IMongoDatabase _database = null;

        public ConfigContext()
        {
            var client = new MongoClient("mongodb://mongodb");
            if (client != null)
                _database = client.GetDatabase("exam");
        }

        public IMongoCollection<ConfigItem> ConfigItem => _database.GetCollection<ConfigItem>("configItem");
    }
}
