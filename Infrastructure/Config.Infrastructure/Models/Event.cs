using System;
using StackExchange.Redis;

namespace Config.Infrastructure.Models
{
    [Serializable]
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AppName { get; set; }
        public EventType Type { get; set; } = EventType.Instance;
        public double RefreshTimerIntervalInMs { get; set; }

        public Event(string appName, double refleshTimer)
        {
            AppName = appName;
            RefreshTimerIntervalInMs = refleshTimer;
        }

        public static implicit operator RedisValue(Event @event) {
            return Formatter.ToByteArray(@event);
        }
    }

    public enum EventType {
        Instance,
        Disposing
    }
}
