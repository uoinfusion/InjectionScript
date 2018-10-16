using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class SemanticScope
    {
        private readonly Stack<Dictionary<string, InjectionValue>> scopes = new Stack<Dictionary<string, InjectionValue>>();

        public void SetLocalVariable(string name, InjectionValue value)
        {
            var locals = scopes.Peek();
            if (locals.ContainsKey(name))
                locals[name] = value;
            else
                throw new NotImplementedException();
        }
        public InjectionValue GetLocalVariable(string name) => scopes.Peek()[name];

        public void Start() => scopes.Push(new Dictionary<string, InjectionValue>());
        public void End() => scopes.Pop();

        internal void DefineVariable(string name)
        {
            var locals = scopes.Peek();
            locals[name] = InjectionValue.Unit;
        }
    }
}
