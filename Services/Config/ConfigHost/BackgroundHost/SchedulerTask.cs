using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigHost
{
    public class SchedulerTask
    {
        public Func<CancellationToken, Task> Task { get; set; }

        public DateTime LastRunTime { get; set; }
        public DateTime NextRunTime { get; set; }
        public double Interval { get; set; }

        public void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = LastRunTime.AddMilliseconds(Interval);
        }

        public bool ShouldRun(DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}
