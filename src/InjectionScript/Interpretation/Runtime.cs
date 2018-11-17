using InjectionScript.Analysis;
using InjectionScript.Parsing;
using InjectionScript.Parsing.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace InjectionScript.Interpretation
{
    public class Runtime
    {
        private readonly ThreadLocal<Interpreter> interpreter;

        public Metadata Metadata { get; } = new Metadata();
        public Interpreter Interpreter => interpreter.Value;
        public Globals Globals { get; } = new Globals();
        public Objects Objects { get; } = new Objects();
        public string CurrentFileName { get; private set; }
        public injectionParser.FileContext CurrentFileSyntax { get; private set; }

        public Runtime()
        {
            interpreter = new ThreadLocal<Interpreter>(() => new Interpreter(Metadata));
            RegisterNatives();
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, string>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, int>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.GetGlobal", (Func<string, string>)Globals.GetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<int, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<double, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<string, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("val", (Func<string, InjectionValue>)InternalSubrutines.Val));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<string, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<int, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<double, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("UO.exec", (Action<string>)Exec));

            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("true", (Func<int>)(() => 1)));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("false", (Func<int>)(() => 0)));
        }

        public MessageCollection Load(string fileName) 
            => Load(File.ReadAllText(fileName), fileName);

        public MessageCollection Load(string content, string fileName)
        {
            var parser = new Parser();
            var errorListener = new MemorySyntaxErrorListener();
            parser.AddErrorListener(errorListener);
            CurrentFileSyntax = parser.ParseFile(content);
            CurrentFileName = fileName;

            if (errorListener.Errors.Any())
                return new MessageCollection(errorListener.Errors);

            Metadata.ResetSubrutines();
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(CurrentFileSyntax);

            var sanityAnalyzer = new SanityAnalyzer();
            return sanityAnalyzer.Analyze(CurrentFileSyntax);
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
