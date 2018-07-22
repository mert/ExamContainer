using Config.Infrastructure;
using Config.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.WebMVC.Infrastructure
{
    public class ConfigContextSeed
    {
        private readonly ConfigContext _ctx;

        public ConfigContextSeed(ConfigContext configContext)
        {
            _ctx = configContext;
        }

        public async Task SeedAsync()
        {
            if (!_ctx.ConfigItem.Find(l => true).Any())
            {
                await _ctx.ConfigItem.InsertManyAsync(new List<ConfigItem> {
                    new ConfigItem
                    {
                        AppName = "SERVICE-A",
                        Name = "SiteName",
                        Type = "String",
                        Value = "Boyner.com.tr",
                        IsActive = true
                    },
                    new ConfigItem
                    {
                        AppName = "SERVICE-B",
                        Name = "IsBasketEnabled",
                        Type = "Boolean",
                        Value = true,
                        IsActive = true
                    },
                    new ConfigItem
                    {
                        AppName = "SERVICE-A",
                        Name = "MaxItemCount",
                        Type = "Int",
                        Value = 50,
                        IsActive = false
                    }
                });
            }
        }
    }
}
