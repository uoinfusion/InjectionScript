using System;

namespace InjectionScript.Runtime
{
    public class TestTimeSource : ITimeSource
    {
        public DateTime StartTime { get; set; }

        public TimeSpan SinceStart => Now - StartTime;

        public DateTime Now { get; set; }
    }
}
