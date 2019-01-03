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
            private class Var
            {
                public InjectionValue Value { get; set; }
                public bool IsGlobal { get; set; }
            }

            private readonly Dictionary<string, Var> vars = new Dictionary<string, Var>();
            private readonly Dictionary<string, Var> globalVars = new Dictionary<string, Var>();

            public Scope(Scope parentScope) : this()
            {
                if (parentScope != null)
                {
                    foreach (var pair in parentScope.globalVars)
                    {
                        vars.Add(pair.Key, new Var { IsGlobal = true, Value = pair.Value.Value });
                    }
                }
            }

            private static readonly KeyValuePair<string, InjectionValue>[] shortcutVariables 
                = Metadata.ShortcutVariables
                    .Select(x => new KeyValuePair<string, InjectionValue>(x, new InjectionValue(x)))
                    .ToArray();

            public Scope()
            {
                foreach (var pair in shortcutVariables)
                    DefineVar(pair.Key, pair.Value);
            }

            internal void SetVar(string name, InjectionValue value)
            {
                if (vars.ContainsKey(name))
                    vars[name].Value = value;
                else
                    throw new StatementFailedException($"Variable {name} is not declared.");
            }

            internal bool TryGetValue(string name, out InjectionValue value)
            {
                if (vars.TryGetValue(name, out var var))
                {
                    value = var.Value;
                    return true;
                }

                value = InjectionValue.Unit;
                return false;
            }

            internal void DefineVar(string name) => DefineVar(name, InjectionValue.Unit);
            internal void DefineVar(string name, InjectionValue value) => vars[name] = new Var { Value = value, IsGlobal = false };

            internal void DefineGlobalVar(string name) => DefineGlobalVar(name, InjectionValue.Unit);
            internal void DefineGlobalVar(string name, InjectionValue value)
            {
                vars[name] = new Var { Value = value, IsGlobal = true };
                globalVars[name] = vars[name];
            }
        }

        private readonly Stack<Scope> scopes = new Stack<Scope>();

        public void Start()
        {
            var scope = new Scope(scopes.FirstOrDefault());
            scopes.Push(scope);
        }

        public void End()
        {
            scopes.Pop();
        }

        public void SetDim(string name, int index, InjectionValue value)
        {
            if (CurrentScope.TryGetValue(name, out var dim))
            {
                if (dim.Kind == InjectionValueKind.Array)
                    dim.Array[index] = value;
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }

        public void SetVar(string name, InjectionValue value) => CurrentScope.SetVar(name, value);

        private bool TopLevelScope => scopes.Count == 1;
        private Scope CurrentScope => scopes.Peek();

        internal void DefineGlobalVariables(IEnumerable<GlobalVariableDefinition> globalVariables,
            Func<GlobalVariableDefinition, InjectionValue> expressionEvaluator)
        {
            if (!TopLevelScope)
                return;

            foreach (var globalVar in globalVariables)
            {
                if (globalVar.HasInitialValue)
                {
                    var initialValue = expressionEvaluator(globalVar);
                    DefineGlobalVar(globalVar.Name, initialValue);
                }
                else
                    DefineGlobalVar(globalVar.Name);
            }
        }

        public bool TryGetVar(string name, out InjectionValue value) 
            => CurrentScope.TryGetValue(name, out value);

        public InjectionValue GetDim(string name, int index)
        {
            if (CurrentScope.TryGetValue(name, out var dim))
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

        internal void DefineGlobalVar(string name) => CurrentScope.DefineGlobalVar(name);
        internal void DefineGlobalVar(string name, InjectionValue value) => CurrentScope.DefineGlobalVar(name, value);

        internal void DefineVar(string name) => CurrentScope.DefineVar(name);
        internal void DefineVar(string name, InjectionValue value) => CurrentScope.DefineVar(name, value);

        internal void DefineDim(string name, int limit)
        {
            var dim = new InjectionValue[limit + 1];
            for (int i = 0; i < limit + 1; i++)
                dim[i] = InjectionValue.Unit;

            CurrentScope.DefineVar(name, new InjectionValue(dim));
        }
    }
}
