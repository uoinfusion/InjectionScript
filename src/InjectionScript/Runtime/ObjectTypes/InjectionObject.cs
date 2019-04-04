using System;
using System.Collections.Generic;

namespace InjectionScript.Runtime.ObjectTypes
{
    public abstract class InjectionObject
    {
        public string Name { get; }

        protected InjectionObject(string name)
        {
            Name = name;
        }

        private readonly NativeSubrutineMetadata definitions = new NativeSubrutineMetadata();

        protected void Register(NativeSubrutineDefinition definition)
            => definitions.Add(definition);

        public bool TryGet(string name, IEnumerable<InjectionValue> argumentValues, out NativeSubrutineDefinition subrutineDefinition)
            => definitions.TryGet(name, argumentValues, out subrutineDefinition);
    }
}
