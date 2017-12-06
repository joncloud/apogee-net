using System;

namespace Apogee
{
    interface IBatchNotifier<T>
    {
        event EventHandler Flushed;
        event EventHandler Notified;
        void Flush();
        void Notify();
    }
}
