using Config.WebMVC.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.WebMVC.Infrastructure
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
