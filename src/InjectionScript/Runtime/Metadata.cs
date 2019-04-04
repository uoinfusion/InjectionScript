using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Runtime
{
    public class Metadata
    {
        private readonly Dictionary<string, SubrutineDefinition> subrutines
            = new Dictionary<string, SubrutineDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> subrutineNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, NativeSubrutineDefinition> intrinsicVariables
            = new Dictionary<string, NativeSubrutineDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GlobalVariableDefinition> globalVariables
            = new Dictionary<string, GlobalVariableDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly NativeSubrutineMetadata nativeSubrutines = new NativeSubrutineMetadata();

        public IEnumerable<SubrutineDefinition> Subrutines => subrutines.Values;
        public IEnumerable<NativeSubrutineDefinition> NativeSubrutines => nativeSubrutines.Subrutines;
        public IEnumerable<GlobalVariableDefinition> GlobalVariables => globalVariables.Values;

        private static HashSet<string> shortcutVariables = new HashSet<string>()
        {
            "lastcorpse",
            "self",
            "backpack",
            "finditem",
            "laststatus",
            "lasttarget"
        };

        public static IEnumerable<string> ShortcutVariables => shortcutVariables;

        public void Add(GlobalVariableDefinition globalVariable)
        {
            globalVariables.Add(globalVariable.Name, globalVariable);
        }

        public void Add(SubrutineDefinition subrutineDef)
        {
            subrutineNames.Add(subrutineDef.Name);
            subrutines[GetSubrutineKey(subrutineDef)] = subrutineDef;
        }

        public void Add(NativeSubrutineDefinition[] subrutineDefs)
            => nativeSubrutines.Add(subrutineDefs);
        public void Add(NativeSubrutineDefinition subrutineDef)
            => nativeSubrutines.Add(subrutineDef);

        public void AddIntrinsicVariables(NativeSubrutineDefinition[] subrutineDefs)
        {
            foreach (var subrutineDef in subrutineDefs)
                AddIntrinsicVariable(subrutineDef);
        }
        public void AddIntrinsicVariable(NativeSubrutineDefinition subrutineDef)
            => intrinsicVariables.Add(subrutineDef.Name, subrutineDef);

        public bool TryGetSubrutine(string name, int argumentCount, out SubrutineDefinition definition)
            => subrutines.TryGetValue(GetSubrutineKey(name, argumentCount), out definition);
        public SubrutineDefinition GetSubrutine(string name, int argumentCount)
            => subrutines[GetSubrutineKey(name, argumentCount)];
        public bool SubrutineExists(string name) => subrutineNames.Contains(name);

        private string GetSubrutineKey(SubrutineDefinition definition)
        {
            var paramCount = definition.Syntax.parameters()?.parameterName()?.Length ?? 0;

            return GetSubrutineKey(definition.Name, paramCount);
        }

        internal void Reset()
        {
            subrutines.Clear();
            globalVariables.Clear();
        }

        private string GetSubrutineKey(string name, int paramCount)
            => $"{name}`{paramCount}";

        public bool TryGetNativeSubrutine(string name, IEnumerable<InjectionValue> argumentValues, out NativeSubrutineDefinition subrutineDefinition)
            => nativeSubrutines.TryGet(name, argumentValues, out subrutineDefinition);

        internal bool ShortcutVariableExists(string name) => shortcutVariables.Contains(name);
        internal bool GlobalVariableExists(string name) => globalVariables.ContainsKey(name);
        internal bool NativeSubrutineExists(string name, int argumentCount)
            => nativeSubrutines.Exists(name, argumentCount);

        public bool TryGetIntrinsicVariable(string name, out NativeSubrutineDefinition variable)
            => intrinsicVariables.TryGetValue(name, out variable);
    }
}
