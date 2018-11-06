using System;
using System.Collections.Generic;

namespace InjectionScript.Interpretation
{
    public class Metadata
    {
        private readonly Dictionary<string, SubrutineDefinition> subrutines
            = new Dictionary<string, SubrutineDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> subrutineNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, NativeSubrutineDefinition> nativeSubrutines
            = new Dictionary<string, NativeSubrutineDefinition>(StringComparer.OrdinalIgnoreCase);

        public void Add(SubrutineDefinition subrutineDef)
        {
            subrutineNames.Add(subrutineDef.Name);
            subrutines.Add(GetSubrutineKey(subrutineDef), subrutineDef);
        }

        public void Add(NativeSubrutineDefinition[] subrutineDefs)
        {
            foreach (var subrutineDef in subrutineDefs)
                nativeSubrutines.Add(subrutineDef.GetSignature(), subrutineDef);
        }
        public void Add(NativeSubrutineDefinition subrutineDef)
            => nativeSubrutines.Add(subrutineDef.GetSignature(), subrutineDef);

        public bool TryGetSubrutine(string name, int argumentCount, out SubrutineDefinition definition)
            => subrutines.TryGetValue(GetSubrutineKey(name, argumentCount), out definition);
        public SubrutineDefinition GetSubrutine(string name, int argumentCount)
            => subrutines[GetSubrutineKey(name, argumentCount)];
        public bool SubrutineExists(string name) => subrutineNames.Contains(name);

        private string GetSubrutineKey(SubrutineDefinition definition)
        {
            var paramCount = definition.Subrutine.parameters()?.parameterName()?.Length ?? 0;

            return GetSubrutineKey(definition.Name, paramCount);
        }

        internal void ResetSubrutines() => subrutines.Clear();
        private string GetSubrutineKey(string name, int paramCount)
            => $"{name}`{paramCount}";

        public bool TryGetNativeSubrutine(string ns, string name, IEnumerable<InjectionValue> argumentValues, out NativeSubrutineDefinition subrutineDefinition)
        {
            var key = NativeSubrutineDefinition.GetSignature(ns, name, argumentValues);
            if (nativeSubrutines.TryGetValue(key, out var value))
            {
                subrutineDefinition = value;
                return true;
            }

            subrutineDefinition = null;
            return false;
        }
    }
}
