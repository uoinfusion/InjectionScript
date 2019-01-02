using System;

namespace InjectionScript.Runtime
{
    public interface ITimeSource
    {
        TimeSpan SinceStart { get; }
        DateTime Now { get; }
    }
}
