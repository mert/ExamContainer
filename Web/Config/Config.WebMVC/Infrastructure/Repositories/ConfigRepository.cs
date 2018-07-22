using Config.WebMVC.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.WebMVC.Infrastructure.Repositories
{
    public class ConfigRepository
    {
        readonly ConfigContext _context;
        public ConfigRepository(ConfigContext configContext)
        {
            _context = configContext;
        }

        public async Task<List<ConfigItem>> GetItemsAsync()
        {
            return await _context.ConfigItem.Find(new BsonDocument()).ToListAsync();
        }

        public async Task SaveConfig(ConfigItem item)
        {
            await _context.ConfigItem.ReplaceOneAsync(doc => doc.Id == item.Id, item, new UpdateOptions { IsUpsert = true });
        }

        public async Task<ConfigItem> GetItemAsync(string id)
        {
            return await _context.ConfigItem.Find(l => l.Id == id).FirstOrDefaultAsync();
        }
    }
}
