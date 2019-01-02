using System;

namespace InjectionScript.Runtime
{
    public class RealTimeSource : ITimeSource
    {
        private readonly DateTime startTime = DateTime.Now;

        public TimeSpan SinceStart => DateTime.Now - startTime;

        public DateTime Now => DateTime.Now;
    }
}
