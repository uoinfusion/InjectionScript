using System;
using System.Threading;

namespace InjectionScript.Lsp
{
    public class SlidingTimer
    {
        private readonly Timer timer;
        private readonly object timerLock = new object();
        private readonly TimeSpan timeout;
        private bool isRunning = false;
        private bool needsReset = false;

        public event EventHandler TimeoutElapsed;

        public SlidingTimer(TimeSpan timeout)
        {
            this.timer = new Timer(OnTimeout);
            this.timeout = timeout;
        }

        private void OnTimeout(object state)
        {
            lock (timerLock)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                isRunning = true;
            }

            TimeoutElapsed?.Invoke(this, EventArgs.Empty);

            lock (timerLock)
            {
                if (needsReset)
                    timer.Change((int)timeout.TotalMilliseconds, Timeout.Infinite);
                isRunning = false;
                needsReset = false;
            }
        }

        public void Start()
        {
            lock (timerLock)
            {
                if (!isRunning)
                    timer.Change((int)timeout.TotalMilliseconds, Timeout.Infinite);
                else
                    needsReset = true;

            }
        }
    }
}
