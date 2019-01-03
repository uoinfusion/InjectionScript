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
        internal int ArgumentCount => parameterKinds.Length;
        private readonly InjectionValueKind[] parameterKinds;

        public NativeSubrutineDefinition(string name, Func<InjectionValue, InjectionValue> subrutine)
            : this(name, subrutine, new[] { InjectionValueKind.Any })
        {
        }

        public NativeSubrutineDefinition(string name, Func<InjectionValue, InjectionValue, InjectionValueKind> subrutine)
            : this(name, subrutine, new[] { InjectionValueKind.Any, InjectionValueKind.Any })
        {
        }

        public NativeSubrutineDefinition(string name, Action<InjectionValue, InjectionValue, InjectionValue, InjectionValue> subrutine)
            : this(name, subrutine, new[] { InjectionValueKind.Any, InjectionValueKind.Any, InjectionValueKind.Any, InjectionValueKind.Any })
        {
        }

        private NativeSubrutineDefinition(string name, Delegate subrutine, InjectionValueKind[] parameterKinds)
        {
            Name = name;
            this.subrutine = subrutine;
            this.parameterKinds = parameterKinds;
        }

        public NativeSubrutineDefinition(string name, Delegate subrutine)
        {
            Name = name;

            this.subrutine = subrutine;

            if (!InjectionValue.IsSupported(subrutine.Method.ReturnType))
                throw new ArgumentException($"The retrun type {subrutine.Method.ReturnType} of {subrutine} is not compatible with native subrutine.");

            var parameterTypes = this.subrutine.Method
                .GetParameters();

            foreach (var param in parameterTypes)
            {
                if (!InjectionValue.IsSupported(param.ParameterType))
                    throw new ArgumentException($"The type {param.ParameterType} of {param.Name} ({subrutine}) is not compatible with native subrutine.");
            }

            parameterKinds = parameterTypes.Select(x => InjectionValue.GetKind(x.ParameterType)).ToArray();
        }

        internal InjectionValue Call(InjectionValue[] argumentValues)
        {
            if (argumentValues.Length != parameterKinds.Length)
                throw new NativeSubrutineException($"Mismatch of argument ({argumentValues.Length}) and parameters ({parameterKinds.Length}) count and parameters for {Name}.");

            object[] args = new object[argumentValues.Length];
            for (int i = 0; i < argumentValues.Length; i++)
                args[i] = argumentValues[i].ConvertTo(parameterKinds[i]);

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
                    throw new NativeSubrutineException("Native subrutine call failed.", ex.InnerException);
                else
                    throw;
            }
        }

        internal string GetSignature()
        {
            return GetSignature(Name, parameterKinds);
        }

        internal static string GetSignature(string name, IEnumerable<InjectionValue> parameters)
            => GetSignature(name, parameters.Select(x => x.Kind));

        internal static string GetSignature(string name, IEnumerable<InjectionValueKind> parameterTypes)
        {
            var parametersSignature = parameterTypes.Select(x => x.ToString())
                .Aggregate(string.Empty, (l, r) => l + "," + r);

            return $"{name}`{parametersSignature}";
        }

        internal static string GetAnySignature(string name, IEnumerable<InjectionValue> argumentValues)
        {
            var parametersSignature = argumentValues.Select(x => "Any")
                .Aggregate(string.Empty, (l, r) => l + "," + r);

            return $"{name}`{parametersSignature}";
        }

        public override string ToString() => $"Subrutine {Name}";
    }
}