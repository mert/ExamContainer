using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config.Infrastructure.Models;
using Config.Infrastructure.Repositories;

namespace Config.Infrastructure.Services
{
    public class ConfigService
    {
        private readonly ConfigRepository _repository;

        public ConfigService(ConfigRepository configRepository)
        {
            _repository = configRepository;
        }

        public async Task<List<ConfigItem>> GetAll()
        {
            return await _repository.GetItemsAsync();
        }

        public async Task SaveConfig(ConfigItem item)
        {
            await _repository.SaveConfig(item);
        }

        public async Task<ConfigItem> GetItemAsync(string id)
        {
            return await _repository.GetItemAsync(id);
        }
    }
}
