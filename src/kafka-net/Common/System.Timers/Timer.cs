#if NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Timers
{

    public class Timer
        : IDisposable
    {

        #region Events
        public event EventHandler<ElapsedEventArgs> Elapsed;
        #endregion

        #region Fields
        private bool _autoReset;
        private double _interval;
        private bool _isRunning;

        private Task _task;
        #endregion

        #region Properties
        public bool AutoReset
        {
            get
            {
                return this._autoReset;
            }
            set
            {
                if (this._autoReset != value)
                {
                    this._autoReset = value;
                }
            }
        }

        public double Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            protected set
            {
                _isRunning = value;
            }
        }

        public bool Enabled
        {
            get
            {

                return this.IsRunning;
            }
            set
            {
                if (value)
                {
                    if (!this.IsRunning)
                    {
                        this.Start();
                    }
                }
                else
                {
                    this.Stop();
                }
            }
        }
        #endregion

        #region Constructors
        public Timer()
            : this(-1)
        {
        }

        public Timer(int interval)
        {
            if (interval <= 0)
            {
                this.AutoReset = false;
            }
            else
            {
                this.AutoReset = true;
            }
            this.Interval = interval;
        }

        ~Timer()
        {
            this.Dispose(false);
        }
        #endregion

        #region Methods
        public void Start()
        {
            if (this.IsRunning)
            {
                this._task = Task.Run(async () =>
                {
                    int executed = 0;
                    double seconds = 0;
                    IsRunning = true;
                    while (IsRunning)
                    {
                        if (seconds != 0 && seconds % this.Interval == 0)
                        {
                            OnTimerElapsed();
                            executed++;
                            if (!this.AutoReset && executed == 1)
                            {
                                this._task = null;
                                this._isRunning = false;
                                return;
                            }
                        }
                        await Task.Delay(1);
                        seconds++;
                    }
                });
            }
        }

        public void Stop()
        {
            IsRunning = false;
            while (this._task != null && this._task.Status == TaskStatus.Running)
            {
                Task.Delay(100).GetAwaiter().GetResult();
            }
            this._task = null;
        }

        protected virtual void OnTimerElapsed()
        {
            this.Elapsed?.Invoke(this, new ElapsedEventArgs());
        }
        #endregion

        #region IDisposable

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                this.Stop();
            }

            // Free any unmanaged objects here.
            disposed = true;
        }
        #endregion

    }

}
#endif