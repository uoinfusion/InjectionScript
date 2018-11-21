using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public interface IDebuggerServer
    {
        IDebugger Create();
        void AddBreakpoint(string fileName, int line);
        void Continue();
    }
}
