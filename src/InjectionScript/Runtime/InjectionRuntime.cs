using InjectionScript.Analysis;
using InjectionScript.Parsing;
using InjectionScript.Parsing.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace InjectionScript.Runtime
{
    public class InjectionRuntime
    {
        private readonly ThreadLocal<Interpreter> interpreter;

        public Metadata Metadata { get; } = new Metadata();
        public Interpreter Interpreter => interpreter.Value;
        public Globals Globals { get; } = new Globals();
        public Objects Objects { get; } = new Objects();
        public string CurrentFileName { get; private set; }
        public injectionParser.FileContext CurrentFileSyntax { get; private set; }
        public InjectionApi Api { get; }

        public InjectionRuntime() : this(null, new NullDebuggerServer())
        {
        }

        public InjectionRuntime(IApiBridge bridge, IDebuggerServer debuggerServer)
        {
            interpreter = new ThreadLocal<Interpreter>(() => new Interpreter(Metadata, CurrentFileName, debuggerServer.Create()));
            Api = new InjectionApi(bridge, Metadata, Globals, debuggerServer);
            RegisterNatives();
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO.exec", (Action<string>)Exec));
        }

        public MessageCollection Load(string fileName) 
            => Load(File.ReadAllText(fileName), fileName);

        public MessageCollection Load(string content, string fileName)
        {
            var parser = new Parser();
            CurrentFileSyntax = parser.ParseFile(content, out var errors);
            CurrentFileName = fileName;

            if (errors.Any())
                return errors;

            Metadata.ResetSubrutines();
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(CurrentFileSyntax);

            var sanityAnalyzer = new SanityAnalyzer();
            return sanityAnalyzer.Analyze(CurrentFileSyntax, Metadata);
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
                return Interpreter.CallSubrutine(subrutine.Syntax,
                    arguments.Select(x => new InjectionValue(x)).ToArray());
            }
            else
                throw new NotImplementedException();
        }

        public int GetObject(string id)
        {
            if (Objects.TryGet(id, out var value))
                return value;

            if (NumberConversions.TryStr2Int(id, out value))
                return value;

            return 0;
        }

        public void SetObject(string name, int value)
        {
            Objects.Set(name, value);
        }

        public void Exec(string parameters)
        {
            var parts = parameters.Split(' ');
            if (parts.Length == 1)
            {
                CallNativeSubrutine(parts[0]);
            }
            else if (parts.Length == 2)
            {
                CallNativeSubrutine(parts[0], new InjectionValue(parts[1].Trim('\'')));
            }
            else
                throw new NotImplementedException();
        }

        private void CallNativeSubrutine(string name, params InjectionValue[] args)
        {
            if (Metadata.TryGetNativeSubrutine("UO." + name, args, out var nativeSubrutine))
            {
                nativeSubrutine.Call(args);
            }
            else if (Metadata.TryGetSubrutine(name, args.Length, out var subrutine))
            {
                Interpreter.CallSubrutine(subrutine.Syntax, args);
            }
            else
                throw new NotImplementedException();
        }
    }
}
