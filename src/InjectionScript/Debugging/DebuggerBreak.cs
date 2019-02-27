using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    public abstract class DebuggerBreak
    {
        public SourceCodeLocation Location { get; }

        public DebuggerBreak(SourceCodeLocation location)
        {
            Location = location;
        }
    }

    public sealed class BreakpointDebuggerBreak : DebuggerBreak
    {
        public Breakpoint Breakpoint { get; }

        public BreakpointDebuggerBreak(Breakpoint breakpoint) 
            : base(new SourceCodeLocation(breakpoint.FileName, breakpoint.Line))
        {
            Breakpoint = breakpoint;
        }
    }

    public sealed class StepDebuggerBreak : DebuggerBreak
    {
        public StepDebuggerBreak(SourceCodeLocation location) : base(location)
        {
        }
    }
}
