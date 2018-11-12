using InjectionScript.Parsing;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class Runtime
    {
        public Metadata Metadata { get; } = new Metadata();
        public Interpreter Interpreter { get; }
        public Globals Globals { get; } = new Globals();
        public Objects Objects { get; } = new Objects();
        public string CurrentFileName { get; private set; }
        public injectionParser.FileContext CurrentFileSyntax { get; private set; }

        public Runtime()
        {
            Interpreter = new Interpreter(Metadata);
            RegisterNatives();
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, string>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.GetGlobal", (Func<string, string>)Globals.GetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<int, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<double, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("val", (Func<string, InjectionValue>)InternalSubrutines.Val));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<string, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<int, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<double, int>)InternalSubrutines.Len));
        }

        public void Load(string fileName)
        {
            var parser = new Parser();
            var errorListener = new MemorySyntaxErrorListener();
            parser.AddErrorListener(errorListener);
            CurrentFileSyntax = parser.ParseFile(File.ReadAllText(fileName));
            CurrentFileName = fileName;

            if (errorListener.Errors.Any())
            {
                throw new SyntaxErrorException(fileName, errorListener.Errors);
            }

            Metadata.ResetSubrutines();
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(CurrentFileSyntax);
        }

        public void Load(injectionParser.FileContext file)
        {
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(file);
        }

        public InjectionValue CallSubrutine(string name, params string[] arguments)
        {
            if (Metadata.TryGetSubrutine(name, arguments.Length, out var subrutine))
            {
                return Interpreter.CallSubrutine(subrutine.Subrutine, 
                    arguments.Select(x => new InjectionValue(x)).ToArray());
            }
            else
                throw new NotImplementedException();
        }

        public int GetObject(string id)
        {
            if (Objects.TryGet(id, out int value))
                return value;

            if (NumberConversions.TryStr2Int(id, out value))
                return value;

            throw new StatementFailedException($"Invalid object {id}.");
        }

        public void SetObject(string name, int value) => Objects.Set(name, value);
    }
}
