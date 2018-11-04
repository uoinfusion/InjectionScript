using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class Runtime
    {
        public Metadata Metadata { get; } = new Metadata();
        public Interpreter Interpreter { get; }
        public Globals Globals { get; }

        public Runtime()
        {
            Interpreter = new Interpreter(Metadata);
            Globals = new Globals();
            RegisterNatives();
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO", "SetGlobal", (Action<string, string>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO", "GetGlobal", (Func<string, string>)Globals.GetGlobal));
        }

        public void Load(injectionParser.FileContext file)
        {
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(file);
        }

        public InjectionValue CallSubrutine(string name, params string[] arguments)
        {
            if (Metadata.TryGet(name, out var subrutine))
            {
                return Interpreter.Visit(subrutine.Subrutine);
            }
            else
                throw new NotImplementedException();
        }
    }
}
