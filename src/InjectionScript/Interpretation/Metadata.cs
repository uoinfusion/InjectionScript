using System.Collections.Generic;

namespace InjectionScript.Interpretation
{
    public class Metadata
    {
        private readonly Dictionary<string, SubrutineDefinition> subrutines
            = new Dictionary<string, SubrutineDefinition>();
        private readonly Dictionary<string, NativeSubrutineDefinition> nativeSubrutines
            = new Dictionary<string, NativeSubrutineDefinition>();

        public void Add(SubrutineDefinition subrutineDef) => subrutines.Add(subrutineDef.Name, subrutineDef);
        public void Add(NativeSubrutineDefinition subrutineDef)
            => nativeSubrutines.Add(GetNativeSubrutineKey(subrutineDef), subrutineDef);
        public bool TryGet(string name, out SubrutineDefinition subrutine)
            => subrutines.TryGetValue(name, out subrutine);

        public SubrutineDefinition GetCustomSubrutine(string name) => subrutines[name];

        public NativeSubrutineDefinition GetNativeSubrutine(string ns, string name)
        {
            string key = string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
            if (nativeSubrutines.TryGetValue(key, out NativeSubrutineDefinition value))
                return value;

            return null;
        }

        private string GetNativeSubrutineKey(NativeSubrutineDefinition subrutineDef)
            => string.IsNullOrEmpty(subrutineDef.NameSpace)
                ? subrutineDef.Name
                : $"{subrutineDef.NameSpace}.{subrutineDef.Name}";
    }
}
