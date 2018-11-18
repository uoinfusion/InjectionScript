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

        public InjectionRuntime() : this(null)
        {

        }

        public InjectionRuntime(IApiBridge bridge)
        {
            interpreter = new ThreadLocal<Interpreter>(() => new Interpreter(Metadata));
            Api = new InjectionApi(bridge);
            RegisterNatives();
        }

        private void RegisterNatives()
        {
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, string>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, int>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, double>)Globals.SetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("UO.GetGlobal", (Func<string, string>)Globals.GetGlobal));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<int, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<double, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("str", (Func<string, string>)InternalSubrutines.Str));
            Metadata.Add(new NativeSubrutineDefinition("val", (Func<string, InjectionValue>)InternalSubrutines.Val));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<string, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<int, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("len", (Func<double, int>)InternalSubrutines.Len));
            Metadata.Add(new NativeSubrutineDefinition("UO.exec", (Action<string>)Exec));

            Metadata.Add(new NativeSubrutineDefinition("wait", (Action<int>)Api.Wait));

            Metadata.Add(new NativeSubrutineDefinition("UO.set", (Action<string, string>)Api.Set));
            Metadata.Add(new NativeSubrutineDefinition("UO.set", (Action<string, int>)Api.Set));

            Metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<int>)Api.GetX));
            Metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<string, int>)Api.GetX));
            Metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<int, int>)Api.GetX));

            Metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<int>)Api.GetY));
            Metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<string, int>)Api.GetY));
            Metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<int, int>)Api.GetY));

            Metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<int>)Api.GetZ));
            Metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<string, int>)Api.GetZ));
            Metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<int, int>)Api.GetZ));

            Metadata.Add(new NativeSubrutineDefinition("UO.getdistance", (Func<string, int>)Api.GetDistance));
            Metadata.Add(new NativeSubrutineDefinition("UO.getdistance", (Func<int, int>)Api.GetDistance));

            Metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<int>)Api.GetHP));
            Metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<int, int>)Api.GetHP));
            Metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<string, int>)Api.GetHP));

            Metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<int>)Api.GetMaxHP));
            Metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<int, int>)Api.GetMaxHP));
            Metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<string, int>)Api.GetMaxHP));

            Metadata.Add(new NativeSubrutineDefinition("UO.getnotoriety", (Func<int, int>)Api.GetNotoriety));
            Metadata.Add(new NativeSubrutineDefinition("UO.getnotoriety", (Func<string, int>)Api.GetNotoriety));

            Metadata.Add(new NativeSubrutineDefinition("UO.getname", (Func<int, string>)Api.GetName));
            Metadata.Add(new NativeSubrutineDefinition("UO.getname", (Func<string, string>)Api.GetName));

            Metadata.Add(new NativeSubrutineDefinition("UO.isnpc", (Func<int, int>)Api.IsNpc));
            Metadata.Add(new NativeSubrutineDefinition("UO.isnpc", (Func<string, int>)Api.IsNpc));

            Metadata.Add(new NativeSubrutineDefinition("UO.getserial", (Func<string, string>)Api.GetSerial));
            Metadata.Add(new NativeSubrutineDefinition("UO.getquantity", (Func<string, int>)Api.GetQuantity));
            Metadata.Add(new NativeSubrutineDefinition("UO.getquantity", (Func<int, int>)Api.GetQuantity));
            Metadata.Add(new NativeSubrutineDefinition("UO.dead", (Func<int>)Api.Dead));
            Metadata.Add(new NativeSubrutineDefinition("UO.hidden", (Func<int>)Api.Hidden));
            Metadata.Add(new NativeSubrutineDefinition("UO.hidden", (Func<string, int>)Api.Hidden));

            Metadata.Add(new NativeSubrutineDefinition("UO.addobject", (Action<string, int>)Api.AddObject));
            Metadata.Add(new NativeSubrutineDefinition("UO.addobject", (Action<string>)Api.AddObject));

            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.str", (Func<int>)Api.Str));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.int", (Func<int>)Api.Int));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.dex", (Func<int>)Api.Dex));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.stamina", (Func<int>)Api.Stamina));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.mana", (Func<int>)Api.Mana));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.weight", (Func<int>)Api.Weight));
            Metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.gold", (Func<int>)Api.Gold));

            Metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<string>)Api.FindType));
            Metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int>)Api.FindType));
            Metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int, int, int>)Api.FindType));
            Metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<string, string, string>)Api.FindType));
            Metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int, int, string>)Api.FindType));
            Metadata.Add(new NativeSubrutineDefinition("UO.findcount", (Func<int>)Api.FindCount));
            Metadata.Add(new NativeSubrutineDefinition("UO.ignore", (Action<int>)Api.Ignore));
            Metadata.Add(new NativeSubrutineDefinition("UO.ignore", (Action<string>)Api.Ignore));
            Metadata.Add(new NativeSubrutineDefinition("UO.ignorereset", (Action)Api.IgnoreReset));
            Metadata.Add(new NativeSubrutineDefinition("UO.count", (Func<string, int>)Api.Count));
            Metadata.Add(new NativeSubrutineDefinition("UO.count", (Func<int, int>)Api.Count));

            Metadata.Add(new NativeSubrutineDefinition("UO.click", (Action<string>)Api.Click));
            Metadata.Add(new NativeSubrutineDefinition("UO.click", (Action<int>)Api.Click));
            Metadata.Add(new NativeSubrutineDefinition("UO.useobject", (Action<string>)Api.UseObject));
            Metadata.Add(new NativeSubrutineDefinition("UO.useobject", (Action<int>)Api.UseObject));
            Metadata.Add(new NativeSubrutineDefinition("UO.attack", (Action<string>)Api.Attack));
            Metadata.Add(new NativeSubrutineDefinition("UO.attack", (Action<int>)Api.Attack));
            Metadata.Add(new NativeSubrutineDefinition("UO.getstatus", (Action<string>)Api.GetStatus));
            Metadata.Add(new NativeSubrutineDefinition("UO.getstatus", (Action<int>)Api.GetStatus));
            Metadata.Add(new NativeSubrutineDefinition("UO.usetype", (Action<int>)Api.UseType));
            Metadata.Add(new NativeSubrutineDefinition("UO.usetype", (Action<string>)Api.UseType));

            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetobject", (Action<string>)Api.WaitTargetObject));
            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetobject", (Action<string, string>)Api.WaitTargetObject));
            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetself", (Action)Api.WaitTargetSelf));
            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetself", (Action<string>)Api.WaitTargetSelf));
            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetlast", (Action)Api.WaitTargetLast));
            Metadata.Add(new NativeSubrutineDefinition("UO.waittargetlast", (Action<string>)Api.WaitTargetLast));
            Metadata.Add(new NativeSubrutineDefinition("UO.targeting", (Func<int>)Api.IsTargeting));

            Metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<int, int>)Api.Grab));
            Metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<int, string>)Api.Grab));
            Metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<string, string>)Api.Grab));
            Metadata.Add(new NativeSubrutineDefinition("UO.setreceivingcontainer", (Action<int>)Api.SetReceivingContainer));
            Metadata.Add(new NativeSubrutineDefinition("UO.setreceivingcontainer", (Action<string>)Api.SetReceivingContainer));

            Metadata.Add(new NativeSubrutineDefinition("UO.keypress", (Action<int>)Api.KeyPress));
            Metadata.Add(new NativeSubrutineDefinition("UO.say", (Action<string>)Api.Say));
            Metadata.Add(new NativeSubrutineDefinition("UO.msg", (Action<string>)Api.Msg));
            Metadata.Add(new NativeSubrutineDefinition("UO.serverprint", (Action<string>)Api.ServerPrint));
            Metadata.Add(new NativeSubrutineDefinition("UO.print", (Action<string>)Api.Print));
            Metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<int, string>)Api.CharPrint));
            Metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<string, string>)Api.CharPrint));
            Metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<int, int, string>)Api.CharPrint));
            Metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<string, int, string>)Api.CharPrint));

            Metadata.Add(new NativeSubrutineDefinition("UO.injournal", (Func<string, int>)Api.InJournal));
            Metadata.Add(new NativeSubrutineDefinition("UO.deletejournal", (Action)Api.DeleteJournal));
            Metadata.Add(new NativeSubrutineDefinition("UO.journal", (Func<int, string>)Api.GetJournalText));
            Metadata.Add(new NativeSubrutineDefinition("UO.journalserial", (Func<int, string>)Api.JournalSerial));
            Metadata.Add(new NativeSubrutineDefinition("UO.setjournalline", (Action<int>)Api.SetJournalLine));
            Metadata.Add(new NativeSubrutineDefinition("UO.setjournalline", (Action<int, string>)Api.SetJournalLine));

            Metadata.Add(new NativeSubrutineDefinition("UO.arm", (Action<string>)Api.Arm));
            Metadata.Add(new NativeSubrutineDefinition("UO.setarm", (Action<string>)Api.SetArm));

            Metadata.Add(new NativeSubrutineDefinition("UO.warmode", (Action<int>)Api.WarMode));
            Metadata.Add(new NativeSubrutineDefinition("UO.warmode", (Func<int>)Api.WarMode));

            Metadata.Add(new NativeSubrutineDefinition("UO.useskill", (Action<string>)Api.UseSkill));
            Metadata.Add(new NativeSubrutineDefinition("UO.cast", (Action<string>)Api.Cast));
            Metadata.Add(new NativeSubrutineDefinition("UO.cast", (Action<string, string>)Api.Cast));

            Metadata.Add(new NativeSubrutineDefinition("UO.morph", (Action<string>)Api.Morph));
            Metadata.Add(new NativeSubrutineDefinition("UO.morph", (Action<int>)Api.Morph));

            Metadata.Add(new NativeSubrutineDefinition("UO.terminate", (Action<string>)Api.Terminate));

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
