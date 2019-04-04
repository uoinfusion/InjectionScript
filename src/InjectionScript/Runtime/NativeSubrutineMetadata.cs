using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    internal sealed class NativeSubrutineMetadata
    {
        private readonly Dictionary<string, NativeSubrutineDefinition> subrutines
            = new Dictionary<string, NativeSubrutineDefinition>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<NativeSubrutineDefinition> Subrutines => subrutines.Values;

        public void Add(NativeSubrutineDefinition[] subrutineDefs)
        {
            foreach (var subrutineDef in subrutineDefs)
                subrutines.Add(subrutineDef.GetSignature(), subrutineDef);
        }

        public void Add(NativeSubrutineDefinition subrutineDef)
            => subrutines.Add(subrutineDef.GetSignature(), subrutineDef);

        public bool TryGet(string name, IEnumerable<InjectionValue> argumentValues, out NativeSubrutineDefinition subrutineDefinition)
        {
            var key = NativeSubrutineDefinition.GetSignature(name, argumentValues);
            if (subrutines.TryGetValue(key, out var value))
            {
                subrutineDefinition = value;
                return true;
            }

            key = NativeSubrutineDefinition.GetAnySignature(name, argumentValues);
            if (subrutines.TryGetValue(key, out value))
            {
                subrutineDefinition = value;
                return true;
            }

            subrutineDefinition = null;
            return false;
        }

        internal bool Exists(string name, int argumentCount)
            => subrutines.Any(x => x.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                && x.Value.ArgumentCount == argumentCount);
    }
}
