using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InjectionScript.Runtime
{
    public class NativeSubrutineDefinition
    {
        private readonly Delegate subrutine;
        public string Name { get; }
        internal int ArgumentCount { get; }

        public NativeSubrutineDefinition(string name, Delegate subrutine)
        {
            Name = name;

            this.subrutine = subrutine;

            if (!InjectionValue.IsSupported(subrutine.Method.ReturnType))
                throw new ArgumentException($"The retrun type {subrutine.Method.ReturnType} of {subrutine} is not compatible with native subrutine.");

            int argumentCount = 0;
            foreach (var param in subrutine.Method.GetParameters())
            {
                if (!InjectionValue.IsSupported(param.ParameterType))
                    throw new ArgumentException($"The type {param.ParameterType} of {param.Name} ({subrutine}) is not compatible with native subrutine.");

                argumentCount++;
            }

            ArgumentCount = argumentCount;
        }

        internal InjectionValue Call(InjectionValue[] argumentValues)
        {
            var args = argumentValues.Select(x => x.ToValue()).ToArray();

            try
            {
                var returnValue = subrutine.DynamicInvoke(args);
                if (returnValue is InjectionValue injVal)
                    return injVal;

                return new InjectionValue(returnValue, subrutine.Method.ReturnType);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        internal string GetSignature()
        {
            var parametersSignature = this.subrutine.Method
                .GetParameters()
                .Select(x => InjectionValue.GetKind(x.ParameterType));

            return GetSignature(Name, parametersSignature);
        }

        internal static string GetSignature(string name, IEnumerable<InjectionValue> parameters)
            => GetSignature(name, parameters.Select(x => x.Kind));

        internal static string GetSignature(string name, IEnumerable<InjectionValueKind> parameterTypes)
        {
            var parametersSignature = parameterTypes.Select(x => x.ToString())
                .Aggregate(string.Empty, (l, r) => l + "," + r);

            return $"{name}`{parametersSignature}";
        }
    }
}