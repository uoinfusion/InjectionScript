using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class SemanticScope
    {
        private class Scope
        {
            public readonly Dictionary<string, InjectionValue> vars = new Dictionary<string, InjectionValue>();
            public readonly Dictionary<string, InjectionValue?[]> dims = new Dictionary<string, InjectionValue?[]>();

            public Scope()
            {
                vars["lastcorpse"] = new InjectionValue("lastcorpse");
                vars["self"] = new InjectionValue("self");
                vars["finditem"] = new InjectionValue("finditem");
                vars["laststatus"] = new InjectionValue("laststatus");
                vars["lasttarget"] = new InjectionValue("lasttarget");
            }
        }

        private readonly Stack<Scope> scopes = new Stack<Scope>();

        public void Start() => scopes.Push(new Scope());
        public void End() => scopes.Pop();

        public void SetVar(string name, InjectionValue value)
        {
            var vars = scopes.Peek().vars;
            if (vars.ContainsKey(name))
                vars[name] = value;
            else if (scopes.Peek().dims.ContainsKey(name))
            {
                scopes.Peek().dims.Remove(name);
                vars[name] = value;
            }
            else
                throw new NotImplementedException();
        }

        public bool TryGetVar(string name, out InjectionValue value)
            => scopes.Peek().vars.TryGetValue(name, out value);

        internal void DefineVar(string name)
        {
            var vars = scopes.Peek().vars;
            vars[name] = InjectionValue.Unit;
        }

        internal void DefineVar(string name, InjectionValue value)
        {
            var vars = scopes.Peek().vars;
            vars[name] = value;
        }

        public void SetDim(string name, int index, InjectionValue value)
        {
            var dims = scopes.Peek().dims;
            if (dims.ContainsKey(name))
                dims[name][index] = value;
            else
                throw new NotImplementedException();
        }

        public InjectionValue GetDim(string name, int index)
        {
            var scope = scopes.Peek();
            if (!scope.dims.TryGetValue(name, out var dim))
                throw new StatementFailedException($"Variable '{name}' is not defined.");

            if (index < 0 && index > dim.Length)
                throw new StatementFailedException($"Index {index} is out of range for variable '{name}' (maximum size of this array is {dim.Length})");

            if (!dim[index].HasValue)
                throw new StatementFailedException($"Reading value from variable '{name}' at  index {index} that is not initialized. Assign some value to '{name}[{index}]' first.");

            return dim[index].Value;
        }

        internal void DefineDim(string name, int limit)
        {
            var dims = scopes.Peek().dims;
            dims[name] = new InjectionValue?[limit + 1];
        }
    }
}
