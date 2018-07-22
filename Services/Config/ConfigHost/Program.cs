using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using StackExchange.Redis;
using Config.Infrastructure.Services;
using Config.Infrastructure;
using Config.Infrastructure.Repositories;

namespace ConfigHost
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();

                    services.AddSingleton(sp =>
                    {
                        return ConnectionMultiplexer.Connect("redis:6379");
                    });

                    services.AddTransient<ConfigService>();
                    services.AddTransient<ConfigRepository>();
                    services.AddTransient<ConfigContext>();
                    services.AddHostedService<HostedService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
