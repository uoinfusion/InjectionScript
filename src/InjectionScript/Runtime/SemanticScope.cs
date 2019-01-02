using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class SemanticScope
    {
        private class Scope
        {
            public readonly Dictionary<string, InjectionValue> vars = new Dictionary<string, InjectionValue>();

            public Scope()
            {
                vars["lastcorpse"] = new InjectionValue("lastcorpse");
                vars["self"] = new InjectionValue("self");
                vars["backpack"] = new InjectionValue("backpack");
                vars["finditem"] = new InjectionValue("finditem");
                vars["laststatus"] = new InjectionValue("laststatus");
                vars["lasttarget"] = new InjectionValue("lasttarget");
            }
        }

        private readonly Stack<Scope> scopes = new Stack<Scope>();

        public void Start() => scopes.Push(new Scope());
        public void End() => scopes.Pop();

        public void SetDim(string name, int index, InjectionValue value)
        {
            var vars = scopes.Peek().vars;
            if (vars.TryGetValue(name, out var dim))
            {
                if (dim.Kind == InjectionValueKind.Array)
                    dim.Array[index] = value;
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }

        public void SetVar(string name, InjectionValue value)
        {
            var vars = scopes.Peek().vars;
            if (vars.ContainsKey(name))
                vars[name] = value;
            else
                throw new NotImplementedException();
        }

        public bool TryGetVar(string name, out InjectionValue value)
            => scopes.Peek().vars.TryGetValue(name, out value);

        public InjectionValue GetDim(string name, int index)
        {
            var vars = scopes.Peek().vars;
            if (vars.TryGetValue(name, out var dim))
            {
                if (dim.Kind == InjectionValueKind.Array)
                {
                    if (dim.Array[index] == InjectionValue.Unit)
                        throw new StatementFailedException($"Accessing not initialized array index {index} of dim '{name}'.");

                    return dim.Array[index];
                }
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();

        }

        internal void DefineVar(string name)
        {
            var vars = scopes.Peek().vars;
            vars[name] = InjectionValue.Unit;
        }

        internal void DefineDim(string name, int limit)
        {
            var vars = scopes.Peek().vars;

            var dim = new InjectionValue[limit + 1];
            for (int i = 0; i < limit + 1; i++)
                dim[i] = InjectionValue.Unit;

            vars[name] = new InjectionValue(dim);
        }

        internal void DefineVar(string name, InjectionValue value)
        {
            var vars = scopes.Peek().vars;
            vars[name] = value;
        }
    }
}
