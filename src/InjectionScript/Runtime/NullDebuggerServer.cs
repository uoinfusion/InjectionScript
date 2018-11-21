using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    internal sealed class NullDebuggerServer : IDebuggerServer
    {
        public void AddBreakpoint(string fileName, int line) { }
        public void Continue() { }
        public IDebugger Create() => null;
        public EvaluationResult EvaluateExpression(string expressionText) => new EvaluationResult(InjectionValue.Unit);
    }
}
