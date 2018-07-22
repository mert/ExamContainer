using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ConfigHost
{
    public class HostedService: IHostedService
    {
        readonly ILogger _logger;
        readonly ConcurrentBag<SchedulerTask> _tasks;
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public HostedService(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<HostedService>();
            _tasks = new ConcurrentBag<SchedulerTask>();

            redis.GetSubscriber().Subscribe("refreshTimerIntervalInMs", (channel, message) => {
                Console.WriteLine((string)message);

                _tasks.Add(new SchedulerTask{
                    Task = arg => {
                        Console.WriteLine($"aaa {DateTime.Now}");
                        return Task.CompletedTask;
                    },
                    Interval = 10,
                    NextRunTime = DateTime.Now.AddSeconds(10)
                });
            });
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

            var tasksThatShouldRun = _tasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task(cancellationToken);
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
    }
}
