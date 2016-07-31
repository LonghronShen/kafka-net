#if NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Timers
{

    /// <summary>
    /// Provides data for the System.Timers.Timer.Elapsed event.
    /// </summary>
    public class ElapsedEventArgs
        : EventArgs
    {

        /// <summary>
        /// Gets the time the System.Timers.Timer.Elapsed event was raised.
        /// </summary>
        public DateTime SignalTime { get; }

        public ElapsedEventArgs()
        {
            this.SignalTime = DateTime.Now;
        }

    }

}
#endif