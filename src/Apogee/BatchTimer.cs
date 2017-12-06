using System;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using Microsoft.Extensions.Options;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Apogee
{
    class BatchTimer<T>
    {
        readonly IBatchQueue<T> _queue;
        readonly IServiceProvider _services;
        readonly Timer _timer;
        readonly int _maximumQueueCountBeforeAutoFlush;
        readonly double _interval;

        public BatchTimer(IBatchQueue<T> queue, IBatchNotifier<T> notifier, IServiceProvider services, IOptions<ApogeeOptions> options)
        {
            _queue = queue;
            _services = services;
            _interval = options.Value.FlushInterval.TotalMilliseconds;
            _timer = new Timer(_interval);
            _timer.Elapsed += Elapsed;

            if (options.Value.MaximumQueueCountBeforeAutoFlush.HasValue)
            {
                _maximumQueueCountBeforeAutoFlush = options.Value.MaximumQueueCountBeforeAutoFlush.Value;
                notifier.Notified += FlushIfOverCount;
            }
            else
            {
                notifier.Notified += ResetTimer;
            }
            notifier.Flushed += Notifier_Flushed;
        }

        void Process()
        {
            using (var scope = _services.CreateScope())
            {
                if (_queue.TryDequeue(out var batch))
                    scope.ServiceProvider
                        .GetRequiredService<IBatchProcessor<T>>()
                        .Process(batch);
            }
        }

        private void Notifier_Flushed(object sender, EventArgs e)
        {
            _timer.Stop();

            Process();
        }

        private void ResetTimer(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Start();
        }

        private void FlushIfOverCount(object sender, EventArgs e)
        {
            if (_timer.Enabled) return;

            if (_queue.Count >= _maximumQueueCountBeforeAutoFlush)
            {
                // Restart the timer with a low interval so that 
                // the execution happens on a separate thread.
                _timer.Interval = 1;
                _timer.Start();
            }
            else
                _timer.Start();
        }
        SpinLock _lock = new SpinLock();
        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _timer.Interval = _interval;

            bool taken = false;
            _lock.TryEnter(ref taken);
            if (taken)
            {
                try
                {
                    Process();
                }
                finally
                {
                    _lock.Exit();
                }
            }
        }
    }
}
