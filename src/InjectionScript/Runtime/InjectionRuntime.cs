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
        ThreadLocal<Interpreter> interpreter;
        private readonly IApiBridge bridge;
        private readonly IDebuggerServer debuggerServer;
        private readonly ITimeSource timeSource;
        private readonly Func<CancellationToken?> retrieveCancellationToken;
        private readonly Paths paths;

        public Metadata Metadata { get; } = new Metadata();
        public Interpreter Interpreter => interpreter.Value;
        public Globals Globals { get; } = new Globals();
        public Objects Objects { get; } = new Objects();
        public InjectionOptions Options { get; } = new InjectionOptions();
        public InjectionApi Api { get; }
        public ScriptFile CurrentScript { get; private set; }

        public InjectionRuntime(Func<CancellationToken?> retrieveCancellationToken = null)
            : this(null, new NullDebuggerServer(), new RealTimeSource(), retrieveCancellationToken)
        {
        }

        public InjectionRuntime(IApiBridge bridge, IDebuggerServer debuggerServer, ITimeSource timeSource, Func<CancellationToken?> retrieveCancellationToken)
        {
            paths = new Paths(() => Path.GetDirectoryName(CurrentScript.FileName));

            Api = new InjectionApi(bridge, Metadata, Globals, timeSource, paths, Objects);
            RegisterNatives();
            this.bridge = bridge;
            this.debuggerServer = debuggerServer;
            this.timeSource = timeSource;
            this.retrieveCancellationToken = retrieveCancellationToken;

            CurrentScript = new ScriptFile("<empty>", string.Empty, null);
            interpreter = new ThreadLocal<Interpreter>(()
                => new Interpreter(Metadata, CurrentScript.FileName, debuggerServer.Create(), retrieveCancellationToken));
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO.Exec", (Action<string>)Exec));
        }

        public MessageCollection Load(string fileName) 
            => Load(File.ReadAllText(fileName), fileName);

        public MessageCollection Load(string content, string fileName)
        {
            var parser = new Parser();
            var syntax = parser.ParseFile(content, out var errors);
            CurrentScript = new ScriptFile(fileName, content, syntax);
            interpreter = new ThreadLocal<Interpreter>(() 
                => new Interpreter(Metadata, CurrentScript.FileName, debuggerServer.Create(), retrieveCancellationToken));

            Metadata.Reset();
            var collector = new DefinitionCollector(Metadata);
            collector.Visit(CurrentScript.Syntax);

            if (errors.Any())
                return errors;

            var sanityAnalyzer = new SanityAnalyzer();
            return sanityAnalyzer.Analyze(CurrentScript.Syntax, Metadata);
        }

        public void Load(injectionParser.FileContext file)
        {
            CurrentScript = new ScriptFile("<script>", file.GetText(), file);
            interpreter = new ThreadLocal<Interpreter>(()
                => new Interpreter(Metadata, CurrentScript.FileName, debuggerServer.Create(), retrieveCancellationToken));

            Metadata.Reset();
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
                if (parts[0].Equals("exec", StringComparison.OrdinalIgnoreCase))
                {
                    bridge.Exec(parts[1]);
                }
                else
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
