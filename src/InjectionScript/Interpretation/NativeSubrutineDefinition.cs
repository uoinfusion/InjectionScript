using System;
using System.Linq;

namespace InjectionScript.Interpretation
{
    public class NativeSubrutineDefinition
    {
        private readonly Delegate subrutine;
        public string NameSpace { get; }
        public string Name { get; }

        public NativeSubrutineDefinition(string ns, string name, Delegate subrutine)
            : this(name, subrutine)
        {
            NameSpace = ns;
        }

        public NativeSubrutineDefinition(string name, Delegate subrutine)
        {
            Name = name;

            this.subrutine = subrutine;

            if (!InjectionValue.IsSupported(subrutine.Method.ReturnType))
                throw new ArgumentException($"The retrun type {subrutine.Method.ReturnType} of {subrutine} is not compatible with native subrutine.");

            foreach (var param in subrutine.Method.GetParameters())
            {
                if (!InjectionValue.IsSupported(param.ParameterType))
                    throw new ArgumentException($"The type {param.ParameterType} of {param.Name} ({subrutine}) is not compatible with native subrutine.");
            }
        }

        internal InjectionValue Call(InjectionValue[] argumentValues)
        {
            var args = argumentValues.Select(x => x.ToValue()).ToArray();
            var returnValue = subrutine.DynamicInvoke(args);

            return new InjectionValue(returnValue, subrutine.Method.ReturnType);
        }
    }
}