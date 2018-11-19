using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class InjectionApiUO
    {
        private readonly IApiBridge bridge;
        private readonly InjectionApi injectionApi;
        private readonly Globals globals;

        internal InjectionApiUO(IApiBridge bridge, InjectionApi injectionApi, Metadata metadata, Globals globals)
        {
            this.bridge = bridge;
            this.injectionApi = injectionApi;
            this.globals = globals;
            Register(metadata);
        }

        internal void Register(Metadata metadata)
        {
            metadata.Add(new NativeSubrutineDefinition("UO.set", (Action<string, string>)Set));
            metadata.Add(new NativeSubrutineDefinition("UO.set", (Action<string, int>)Set));

            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, string>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, int>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, double>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.GetGlobal", (Func<string, string>)globals.GetGlobal));

            metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<int>)GetX));
            metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<string, int>)GetX));
            metadata.Add(new NativeSubrutineDefinition("UO.getx", (Func<int, int>)GetX));

            metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<int>)GetY));
            metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<string, int>)GetY));
            metadata.Add(new NativeSubrutineDefinition("UO.gety", (Func<int, int>)GetY));

            metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<int>)GetZ));
            metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<string, int>)GetZ));
            metadata.Add(new NativeSubrutineDefinition("UO.getz", (Func<int, int>)GetZ));

            metadata.Add(new NativeSubrutineDefinition("UO.getdistance", (Func<string, int>)GetDistance));
            metadata.Add(new NativeSubrutineDefinition("UO.getdistance", (Func<int, int>)GetDistance));

            metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<int>)GetHP));
            metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<int, int>)GetHP));
            metadata.Add(new NativeSubrutineDefinition("UO.gethp", (Func<string, int>)GetHP));

            metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<int>)GetMaxHP));
            metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<int, int>)GetMaxHP));
            metadata.Add(new NativeSubrutineDefinition("UO.getmaxhp", (Func<string, int>)GetMaxHP));

            metadata.Add(new NativeSubrutineDefinition("UO.getnotoriety", (Func<int, int>)GetNotoriety));
            metadata.Add(new NativeSubrutineDefinition("UO.getnotoriety", (Func<string, int>)GetNotoriety));

            metadata.Add(new NativeSubrutineDefinition("UO.getname", (Func<int, string>)GetName));
            metadata.Add(new NativeSubrutineDefinition("UO.getname", (Func<string, string>)GetName));

            metadata.Add(new NativeSubrutineDefinition("UO.getgraphic", (Func<string, int>)GetGraphics));
            metadata.Add(new NativeSubrutineDefinition("UO.getgraphic", (Func<int, int>)GetGraphics));

            metadata.Add(new NativeSubrutineDefinition("UO.getdir", (Func<string, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.getdir", (Func<int, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.getdir", (Func<int>)GetDir));

            metadata.Add(new NativeSubrutineDefinition("UO.isnpc", (Func<int, int>)IsNpc));
            metadata.Add(new NativeSubrutineDefinition("UO.isnpc", (Func<string, int>)IsNpc));

            metadata.Add(new NativeSubrutineDefinition("UO.getserial", (Func<string, string>)GetSerial));
            metadata.Add(new NativeSubrutineDefinition("UO.getquantity", (Func<string, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.getquantity", (Func<int, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.dead", (Func<int>)Dead));
            metadata.Add(new NativeSubrutineDefinition("UO.hidden", (Func<int>)Hidden));
            metadata.Add(new NativeSubrutineDefinition("UO.hidden", (Func<string, int>)Hidden));

            metadata.Add(new NativeSubrutineDefinition("UO.addobject", (Action<string, int>)AddObject));
            metadata.Add(new NativeSubrutineDefinition("UO.addobject", (Action<string>)AddObject));

            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.str", (Func<int>)Str));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.int", (Func<int>)Int));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.dex", (Func<int>)Dex));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.stamina", (Func<int>)Stamina));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.mana", (Func<int>)Mana));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.weight", (Func<int>)Weight));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.gold", (Func<int>)Gold));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.life", (Func<int>)Life));

            metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int, int, int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<string, string, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.findtype", (Action<int, int, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.findcount", (Func<int>)FindCount));
            metadata.Add(new NativeSubrutineDefinition("UO.ignore", (Action<int>)Ignore));
            metadata.Add(new NativeSubrutineDefinition("UO.ignore", (Action<string>)Ignore));
            metadata.Add(new NativeSubrutineDefinition("UO.ignorereset", (Action)IgnoreReset));
            metadata.Add(new NativeSubrutineDefinition("UO.count", (Func<string, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.count", (Func<int, int>)Count));

            metadata.Add(new NativeSubrutineDefinition("UO.click", (Action<string>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.click", (Action<int>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.useobject", (Action<string>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.useobject", (Action<int>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.attack", (Action<string>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.attack", (Action<int>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.getstatus", (Action<string>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.getstatus", (Action<int>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.usetype", (Action<int>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.usetype", (Action<string>)UseType));

            metadata.Add(new NativeSubrutineDefinition("UO.waittargetobject", (Action<string>)WaitTargetObject));
            metadata.Add(new NativeSubrutineDefinition("UO.waittargetobject", (Action<string, string>)WaitTargetObject));
            metadata.Add(new NativeSubrutineDefinition("UO.waittargetself", (Action)WaitTargetSelf));
            metadata.Add(new NativeSubrutineDefinition("UO.waittargetself", (Action<string>)WaitTargetSelf));
            metadata.Add(new NativeSubrutineDefinition("UO.waittargetlast", (Action)WaitTargetLast));
            metadata.Add(new NativeSubrutineDefinition("UO.waittargetlast", (Action<string>)WaitTargetLast));
            metadata.Add(new NativeSubrutineDefinition("UO.targeting", (Func<int>)IsTargeting));

            metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<int, int>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<int, string>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.grab", (Action<string, string>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.setreceivingcontainer", (Action<int>)SetReceivingContainer));
            metadata.Add(new NativeSubrutineDefinition("UO.setreceivingcontainer", (Action<string>)SetReceivingContainer));

            metadata.Add(new NativeSubrutineDefinition("UO.lclick", (Action<int, int>)LClick));
            metadata.Add(new NativeSubrutineDefinition("UO.keypress", (Action<int>)KeyPress));
            metadata.Add(new NativeSubrutineDefinition("UO.press", (Action<int>)Press));
            metadata.Add(new NativeSubrutineDefinition("UO.say", (Action<string>)Say));

            metadata.Add(new NativeSubrutineDefinition("UO.playwav", (Action<string>)PlayWav));

            metadata.Add(new NativeSubrutineDefinition("UO.textopen", (Action)TextOpen));
            metadata.Add(new NativeSubrutineDefinition("UO.textprint", (Action<string>)TextPrint));

            metadata.Add(new NativeSubrutineDefinition("UO.msg", (Action<string>)Msg));
            metadata.Add(new NativeSubrutineDefinition("UO.serverprint", (Action<string>)ServerPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.print", (Action<string>)Print));
            metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<int, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<string, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<int, int, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.charprint", (Action<string, int, string>)CharPrint));

            metadata.Add(new NativeSubrutineDefinition("UO.injournal", (Func<string, int>)InJournal));
            metadata.Add(new NativeSubrutineDefinition("UO.deletejournal", (Action)DeleteJournal));
            metadata.Add(new NativeSubrutineDefinition("UO.journal", (Func<int, string>)GetJournalText));
            metadata.Add(new NativeSubrutineDefinition("UO.journalserial", (Func<int, string>)JournalSerial));
            metadata.Add(new NativeSubrutineDefinition("UO.setjournalline", (Action<int>)SetJournalLine));
            metadata.Add(new NativeSubrutineDefinition("UO.setjournalline", (Action<int, string>)SetJournalLine));

            metadata.Add(new NativeSubrutineDefinition("UO.arm", (Action<string>)Arm));
            metadata.Add(new NativeSubrutineDefinition("UO.setarm", (Action<string>)SetArm));

            metadata.Add(new NativeSubrutineDefinition("UO.warmode", (Action<int>)WarMode));
            metadata.Add(new NativeSubrutineDefinition("UO.warmode", (Func<int>)WarMode));

            metadata.Add(new NativeSubrutineDefinition("UO.useskill", (Action<string>)UseSkill));
            metadata.Add(new NativeSubrutineDefinition("UO.cast", (Action<string>)Cast));
            metadata.Add(new NativeSubrutineDefinition("UO.cast", (Action<string, string>)Cast));

            metadata.Add(new NativeSubrutineDefinition("UO.morph", (Action<string>)Morph));
            metadata.Add(new NativeSubrutineDefinition("UO.morph", (Action<int>)Morph));

            metadata.Add(new NativeSubrutineDefinition("UO.terminate", (Action<string>)Terminate));
        }

        public void Set(string name, int value)
        {
            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
                bridge.SetFindDistance(value);
        }

        public void Set(string name, string valueStr)
        {
            bool successfulConversion = NumberConversions.TryStr2Int(valueStr, out var value);

            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
            {
                bridge.SetFindDistance(successfulConversion ? value : 0);
            }
        }

        private int GetObject(string name) => injectionApi.GetObject(name);

        public int GetX() => GetX("self");
        public int GetX(string id) => GetX(GetObject(id));
        public int GetX(int id) => bridge.GetX(id);

        public int GetY() => GetY("self");
        public int GetY(string id) => GetY(GetObject(id));
        public int GetY(int id) => bridge.GetY(id);

        public int GetZ() => GetZ("self");
        public int GetZ(string id) => GetZ(GetObject(id));
        public int GetZ(int id) => bridge.GetY(id);

        public int GetDistance(string id) => GetDistance(GetObject(id));
        public int GetDistance(int id) => bridge.GetDistance(id);

        public int GetHP() => GetHP("self");
        public int GetHP(string id) => GetHP(GetObject(id));
        public int GetHP(int id) => bridge.GetHP(id);

        public int GetMaxHP() => GetHP("self");
        public int GetMaxHP(string id) => GetMaxHP(GetObject(id));
        public int GetMaxHP(int id) => bridge.GetMaxHP(id);

        public int GetNotoriety(string id) => GetNotoriety(GetObject(id));
        public int GetNotoriety(int id) => bridge.GetNotoriety(id);

        public string GetName(string id) => GetName(GetObject(id));
        public string GetName(int id) => bridge.GetName(id);

        public int GetGraphics(string id) => GetGraphics(GetObject(id));
        public int GetGraphics(int id) => bridge.GetGraphics(id);

        public int GetDir() => GetDir("self");
        public int GetDir(string id) => GetDir(GetObject(id));
        public int GetDir(int id) => bridge.GetDir(id);

        public int IsNpc(string id) => IsNpc(GetObject(id));
        public int IsNpc(int id) => bridge.IsNpc(id);

        public int GetQuantity(string id) => GetQuantity(GetObject(id));
        public int GetQuantity(int id) => bridge.GetQuantity(id);

        public string GetSerial(string id) => NumberConversions.Int2Hex(GetObject(id));
        public int Dead() => bridge.Dead();
        public int Hidden() => Hidden("self");
        public int Hidden(string idText) => Hidden(GetObject(idText));
        public int Hidden(int id) => bridge.Hidden(id);

        public void AddObject(string name, int id) => injectionApi.SetObject(name, id);
        public void AddObject(string name) => bridge.AddObject(name);

        public int Str() => bridge.Strength;
        public int Int() => bridge.Intelligence;
        public int Dex() => bridge.Dexterity;
        public int Stamina() => bridge.Stamina;
        public int Mana() => bridge.Mana;
        public int Weight() => bridge.Weight;
        public int Gold() => bridge.Gold;
        public int Life() => GetHP("self");

        public void Click(string id) => Click(GetObject(id));
        public void Click(int id) => bridge.Click(id);
        public void UseObject(string id) => UseObject(GetObject(id));
        public void UseObject(int id) => bridge.UseObject(id);
        public void Attack(string id) => Attack(GetObject(id));
        public void Attack(int id) => bridge.Attack(id);
        public void GetStatus(string id) => GetStatus(GetObject(id));
        public void GetStatus(int id) => bridge.GetStatus(id);

        public void UseType(string type) => UseType(NumberConversions.Str2Int(type));
        public void UseType(int type) => bridge.UseType(type);

        public void WaitTargetObject(string id) => WaitTargetObject(GetObject(id));
        public void WaitTargetObject(int id) => bridge.WaitTargetObject(id);
        public void WaitTargetObject(string id1, string id2) => WaitTargetObject(GetObject(id1), GetObject(id2));
        public void WaitTargetObject(int id1, int id2) => bridge.WaitTargetObject(id1, id2);
        public void WaitTargetSelf(string id /* ignored */) => WaitTargetObject("self");
        public void WaitTargetSelf() => WaitTargetObject("self");
        public void WaitTargetLast(string id /* ignored */) => WaitTargetObject("lasttarget");
        public void WaitTargetLast() => WaitTargetObject("lasttarget");
        public int IsTargeting() => bridge.IsTargeting();

        public void SetReceivingContainer(string id) => SetReceivingContainer(GetObject(id));
        public void SetReceivingContainer(int id) => bridge.SetReceivingContainer(id);
        public void Grab(int amount, string id) => Grab(amount, GetObject(id));
        public void Grab(string amount, string id) => Grab(NumberConversions.Str2Int(amount), GetObject(id));
        public void Grab(int amount, int id) => bridge.Grab(amount, id);

        public void FindType(string typeStr) => FindType(NumberConversions.Str2Int(typeStr));
        public void FindType(int type) => FindType(type, -1, -1);
        public void FindType(string type, string color, string container)
            => FindType(NumberConversions.Str2Int(type), NumberConversions.Str2Int(color), ConvertContainer(container));
        public void FindType(int type, int color, string container)
            => FindType(type, color, ConvertContainer(container));
        public void FindType(int type, int color, int containerId) => bridge.FindType(type, color, containerId);
        public int FindCount() => bridge.FindCount();
        public int Count(string type) => Count(NumberConversions.Str2Int(type));
        public int Count(int type) => bridge.Count(type);

        public void Ignore(string id) => Ignore(GetObject(id));
        public void Ignore(int id) => bridge.Ignore(id);
        public void IgnoreReset() => bridge.IgnoreReset();

        public void LClick(int x, int y) => bridge.LClick(x, y);
        public void Press(int key) => KeyPress(key);
        public void KeyPress(int key) => bridge.KeyPress(key);
        public void Say(string message) => bridge.Say(message);

        public void PlayWav(string file) => bridge.PlayWav(file);

        public void TextOpen() => bridge.TextOpen();
        public void TextPrint(string text) => bridge.TextPrint(text);

        public void Msg(string message) => bridge.ServerPrint(message);
        public void ServerPrint(string message) => bridge.ServerPrint(message);

        public void Print(string msg) => bridge.Print(msg);
        public void CharPrint(int color, string msg) => CharPrint(bridge.Self, color, msg);
        public void CharPrint(string color, string msg) => CharPrint(bridge.Self, NumberConversions.Str2Int(color), msg);
        public void CharPrint(string id, int color, string msg) => CharPrint(GetObject(id), color, msg);
        public void CharPrint(int id, int color, string msg) => bridge.ClientPrint(id, color, msg);

        public int InJournal(string pattern) => bridge.InJournal(pattern);
        public void DeleteJournal() => bridge.DeleteJournal();
        public string GetJournalText(int index) => bridge.GetJournalText(index);
        public string JournalSerial(int index) => bridge.JournalSerial(index);
        public void SetJournalLine(int index) => bridge.SetJournalLine(index);
        public void SetJournalLine(int index, string text) => bridge.SetJournalLine(index);

        public void Arm(string name) => bridge.Arm(name);
        public void SetArm(string name) => bridge.SetArm(name);

        public void WarMode(int mode) => bridge.WarMode(mode);
        public int WarMode() => bridge.WarMode();

        public void UseSkill(string skillName) => bridge.UseSkill(skillName);
        public void Cast(string spellName) => bridge.Cast(spellName);
        public void Cast(string spellName, string id) => Cast(spellName, GetObject(id));
        public void Cast(string spellName, int id) => bridge.Cast(spellName, id);

        public void Morph(string type) => Morph(NumberConversions.Str2Int(type));
        public void Morph(int type) => bridge.Morph(type);
        public void Terminate(string subrutineName) => bridge.Terminate(subrutineName);

        private int ConvertContainer(string id)
        {
            if (id.Equals("my", StringComparison.OrdinalIgnoreCase))
                return -1;
            else if (id.Equals("ground", StringComparison.OrdinalIgnoreCase))
                return 1;

            return GetObject(id);
        }
    }
}
