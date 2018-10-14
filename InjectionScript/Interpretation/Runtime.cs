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

        public Runtime()
        {
            Interpreter = new Interpreter(Metadata);
        }

        public void Load(injectionParser.FileContext file)
        {
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(file);
        }

        public void CallSubrutine(string name, params string[] arguments)
        {
            if (Metadata.TryGet(name, out var subrutine))
            {
                Interpreter.Visit(subrutine.Subrutine);
            }
            else
                throw new NotImplementedException();
        }
    }
}
