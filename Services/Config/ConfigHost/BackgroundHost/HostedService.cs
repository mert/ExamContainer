using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Config.Infrastructure;
using Config.Infrastructure.Models;
using Config.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigHost
{
    public class HostedService: IHostedService
    {
        readonly ILogger _logger;
        readonly ConcurrentDictionary<Guid, SchedulerTask> _tasks;
        readonly IServiceProvider _serviceProvider;
        readonly ConnectionMultiplexer _redis;
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public HostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<HostedService>();
            _tasks = new ConcurrentDictionary<Guid, SchedulerTask>();
            _serviceProvider = serviceProvider;
            _redis = redis;
            EventListener();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;
            
            _cts.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _tasks.Where(t => t.Value.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Value.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Value.Task(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            if (!args.Observed)
                                throw;
                        }
                    },
                    cancellationToken);
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private void EventListener()
        {
            _redis.GetSubscriber().Subscribe("hostChannel", (channel, message) => {
                var @event = Formatter.FromByteArray<Event>(message);

                if (@event.Type == EventType.Disposing)
                {
                    var select = _tasks[@event.Id];
                    _tasks.TryRemove(@event.Id, out select);
                    return;
                }

                var task = new SchedulerTask
                {
                    Task = arg =>
                    {
                        return Recycle();
                    },
                    Interval = @event.RefreshTimerIntervalInMs,
                    NextRunTime = DateTime.Now.AddMilliseconds(@event.RefreshTimerIntervalInMs)
                };

                _tasks.TryAdd(@event.Id, task);

            });
        }

        private async Task Recycle() {
            using (var scope = _serviceProvider.CreateScope())
            {
                var configService = scope.ServiceProvider.GetRequiredService<ConfigService>();
                foreach (var item in await configService.GetAll())
                    if (item.IsActive)
                        await _redis.GetDatabase().StringSetAsync($"{item.AppName}:{item.Name}", Formatter.ToByteArray(item.Value));
                    else
                        await _redis.GetDatabase().KeyDeleteAsync($"{item.AppName}:{item.Name}");
            }
        }

    }
}
