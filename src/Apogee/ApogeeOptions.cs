using System;

namespace Apogee
{
    class ApogeeOptions
    {
        public TimeSpan FlushInterval { get; set; }
        public int? MaximumQueueCountBeforeAutoFlush { get; set; }
    }
}
