using Config.WebMVC.Infrastructure.Repositories;
using Config.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.WebMVC.Infrastructure.Services
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
    }
}
