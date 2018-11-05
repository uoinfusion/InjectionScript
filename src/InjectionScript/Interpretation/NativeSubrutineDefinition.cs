using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Interpretation
{
    public class NativeSubrutineDefinition
    {
        private readonly Delegate subrutine;
        public string NameSpace { get; }
        public string Name { get; }

        public NativeSubrutineDefinition(string ns, string name, Delegate subrutine)
            : this(name, subrutine) => NameSpace = ns;

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

        internal string GetSignature()
        {
            var parametersSignature = this.subrutine.Method
                .GetParameters()
                .Select(x => InjectionValue.GetKind(x.ParameterType));

            return GetSignature(NameSpace, Name, parametersSignature);
        }

        internal static string GetSignature(string ns, string name, IEnumerable<InjectionValue> parameters)
            => GetSignature(ns, name, parameters.Select(x => x.Kind));

        internal static string GetSignature(string ns, string name, IEnumerable<InjectionValueKind> parameterTypes)
        {
            var parametersSignature = parameterTypes.Select(x => x.ToString())
                .Aggregate(string.Empty, (l, r) => l + "," + r);

            return string.IsNullOrEmpty(ns)
                ? $"{name}`{parametersSignature}"
                : $"{ns}.{name}{parametersSignature}";
        }
    }
}