using System;

namespace Apogee
{
    class BatchNotifier<T> : IBatchNotifier<T>
    {
        public event EventHandler Flushed;
        public event EventHandler Notified;
        public void Flush() =>
            Flushed?.Invoke(this, EventArgs.Empty);

        public void Notify() =>
            Notified?.Invoke(this, EventArgs.Empty);
    }
}
