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
        private readonly Random random;
        private readonly ITimeSource timeSource;

        internal InjectionApiUO(IApiBridge bridge, InjectionApi injectionApi, Metadata metadata, Globals globals, ITimeSource timeSource)
        {
            this.bridge = bridge;
            this.injectionApi = injectionApi;
            this.globals = globals;
            Register(metadata);
            random = new Random();
            this.timeSource = timeSource;
        }

        internal void Register(Metadata metadata)
        {
            metadata.Add(new NativeSubrutineDefinition("UO.Set", (Action<string, string>)Set));
            metadata.Add(new NativeSubrutineDefinition("UO.Set", (Action<string, int>)Set));

            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, string>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, int>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.SetGlobal", (Action<string, double>)globals.SetGlobal));
            metadata.Add(new NativeSubrutineDefinition("UO.GetGlobal", (Func<string, string>)globals.GetGlobal));

            metadata.Add(new NativeSubrutineDefinition("UO.GetX", (Func<int>)GetX));
            metadata.Add(new NativeSubrutineDefinition("UO.GetX", (Func<string, int>)GetX));
            metadata.Add(new NativeSubrutineDefinition("UO.GetX", (Func<int, int>)GetX));

            metadata.Add(new NativeSubrutineDefinition("UO.GetY", (Func<int>)GetY));
            metadata.Add(new NativeSubrutineDefinition("UO.GetY", (Func<string, int>)GetY));
            metadata.Add(new NativeSubrutineDefinition("UO.GetY", (Func<int, int>)GetY));

            metadata.Add(new NativeSubrutineDefinition("UO.GetZ", (Func<int>)GetZ));
            metadata.Add(new NativeSubrutineDefinition("UO.GetZ", (Func<string, int>)GetZ));
            metadata.Add(new NativeSubrutineDefinition("UO.GetZ", (Func<int, int>)GetZ));

            metadata.Add(new NativeSubrutineDefinition("UO.GetDistance", (Func<string, int>)GetDistance));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDistance", (Func<int, int>)GetDistance));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDistance", (Func<int, int, int, int, int>)GetDistance));

            metadata.Add(new NativeSubrutineDefinition("UO.GetHP", (Func<int>)GetHP));
            metadata.Add(new NativeSubrutineDefinition("UO.GetHP", (Func<int, int>)GetHP));
            metadata.Add(new NativeSubrutineDefinition("UO.GetHP", (Func<string, int>)GetHP));

            metadata.Add(new NativeSubrutineDefinition("UO.GetMaxHP", (Func<int>)GetMaxHP));
            metadata.Add(new NativeSubrutineDefinition("UO.GetMaxHP", (Func<int, int>)GetMaxHP));
            metadata.Add(new NativeSubrutineDefinition("UO.GetMaxHP", (Func<string, int>)GetMaxHP));

            metadata.Add(new NativeSubrutineDefinition("UO.GetNotoriety", (Func<int, int>)GetNotoriety));
            metadata.Add(new NativeSubrutineDefinition("UO.GetNotoriety", (Func<string, int>)GetNotoriety));

            metadata.Add(new NativeSubrutineDefinition("UO.GetInfo", (Func<string, string>)GetInfo));
            metadata.Add(new NativeSubrutineDefinition("UO.GetName", (Func<int, string>)GetName));
            metadata.Add(new NativeSubrutineDefinition("UO.GetName", (Func<string, string>)GetName));

            metadata.Add(new NativeSubrutineDefinition("UO.GetGraphic", (Func<string, int>)GetGraphics));
            metadata.Add(new NativeSubrutineDefinition("UO.GetGraphic", (Func<int, int>)GetGraphics));

            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<string, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<int, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<int>)GetDir));

            metadata.Add(new NativeSubrutineDefinition("UO.IsNpc", (Func<int, int>)IsNpc));
            metadata.Add(new NativeSubrutineDefinition("UO.IsNpc", (Func<string, int>)IsNpc));

            metadata.Add(new NativeSubrutineDefinition("UO.Exists", (Func<int, int>)Exists));
            metadata.Add(new NativeSubrutineDefinition("UO.Exists", (Func<string, int>)Exists));

            metadata.Add(new NativeSubrutineDefinition("UO.Getserial", (Func<string, string>)GetSerial));
            metadata.Add(new NativeSubrutineDefinition("UO.GetQuantity", (Func<string, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.GetQuantity", (Func<int, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.IsOnline", (Func<int>)IsOnline));
            metadata.Add(new NativeSubrutineDefinition("UO.Dead", (Func<int>)Dead));
            metadata.Add(new NativeSubrutineDefinition("UO.Hidden", (Func<int>)Hidden));
            metadata.Add(new NativeSubrutineDefinition("UO.Hidden", (Func<string, int>)Hidden));

            metadata.Add(new NativeSubrutineDefinition("UO.AddObject", (Action<string, int>)AddObject));
            metadata.Add(new NativeSubrutineDefinition("UO.AddObject", (Action<string>)AddObject));

            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Str", (Func<int>)Str));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Int", (Func<int>)Int));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Dex", (Func<int>)Dex));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Stamina", (Func<int>)Stamina));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Mana", (Func<int>)Mana));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Weight", (Func<int>)Weight));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Gold", (Func<int>)Gold));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Life", (Func<int>)Life));

            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<int, int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<int, int, int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<string, int, int>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<string, string, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Action<int, int, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<int>)FindCount));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<string, int>)((ignoredParam1) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<string, string, int>)((ignoredParam1, ignoredParam2) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<string, string, string, int>)((ignoredParam1, ignoredParam2, ignoredParam3) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<int, int>)((ignoredParam1) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<int, int, int>)((ignoredParam1, ignoredParam2) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.FindCount", (Func<int, int, int, int>)((ignoredParam1, ignoredParam2, ignoredParam3) => FindCount())));
            metadata.Add(new NativeSubrutineDefinition("UO.Ignore", (Action<int>)Ignore));
            metadata.Add(new NativeSubrutineDefinition("UO.Ignore", (Action<string>)Ignore));
            metadata.Add(new NativeSubrutineDefinition("UO.IgnoreReset", (Action)IgnoreReset));
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<string, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<int, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<InjectionValue, InjectionValue, InjectionValue, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Click", (Action<string>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.Click", (Action<int>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.UseObject", (Action<string>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.UseObject", (Action<int>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.Attack", (Action<string>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.Attack", (Action<int>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.GetStatus", (Action<string>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.GetStatus", (Action<int>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<int>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<string>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<int, int>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<string, string>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<int, string>)UseType));
            metadata.Add(new NativeSubrutineDefinition("UO.UseType", (Action<string, int>)UseType));

            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetObject", (Action<string>)WaitTargetObject));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetObject", (Action<string, string>)WaitTargetObject));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetSelf", (Action)WaitTargetSelf));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetSelf", (Action<string>)WaitTargetSelf));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetLast", (Action)WaitTargetLast));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetLast", (Action<string>)WaitTargetLast));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetTile", (Action<int, int, int, int>)WaitTargetTile));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetTile", (Action<string, string, string, string>)WaitTargetTile));
            metadata.Add(new NativeSubrutineDefinition("UO.Targeting", (Func<int>)IsTargeting));

            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<int, int>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<int, string>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<string, string>)Grab));

            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<string, string>)MoveItem));
            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<string, int>)MoveItem));
            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<int, int>)MoveItem));
            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<string, string, string>)MoveItem));
            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<int, int, int>)MoveItem));

            metadata.Add(new NativeSubrutineDefinition("UO.SetReceivingContainer", (Action<int>)SetReceivingContainer));
            metadata.Add(new NativeSubrutineDefinition("UO.SetReceivingContainer", (Action<string>)SetReceivingContainer));
            metadata.Add(new NativeSubrutineDefinition("UO.UnsetReceivingContainer", (Action)UnsetReceivingContainer));
            
            metadata.Add(new NativeSubrutineDefinition("UO.LClick", (Action<int, int>)LClick));
            metadata.Add(new NativeSubrutineDefinition("UO.KeyPress", (Action<int>)KeyPress));
            metadata.Add(new NativeSubrutineDefinition("UO.Press", (Action<int>)Press));
            metadata.Add(new NativeSubrutineDefinition("UO.Say", (Action<string>)Say));

            metadata.Add(new NativeSubrutineDefinition("UO.PlayWav", (Action<string>)PlayWav));

            metadata.Add(new NativeSubrutineDefinition("UO.TextOpen", (Action)TextOpen));
            metadata.Add(new NativeSubrutineDefinition("UO.TextPrint", (Action<string>)TextPrint));

            metadata.Add(new NativeSubrutineDefinition("UO.Msg", (Action<string>)Msg));
            metadata.Add(new NativeSubrutineDefinition("UO.ServerPrint", (Action<string>)ServerPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.Print", (Action<string>)Print));
            metadata.Add(new NativeSubrutineDefinition("UO.CharPrint", (Action<int, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.CharPrint", (Action<string, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.CharPrint", (Action<int, int, string>)CharPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.CharPrint", (Action<InjectionValue, InjectionValue, InjectionValue>)CharPrint));

            metadata.Add(new NativeSubrutineDefinition("UO.InJournal", (Func<string, int>)InJournal));
            metadata.Add(new NativeSubrutineDefinition("UO.InJournalBetweenTimes", (Func<string, int, int, int>)InJournalBetweenTimes));
            metadata.Add(new NativeSubrutineDefinition("UO.InJournalBetweenTimes", (Func<string, int, int, int, int>)InJournalBetweenTimes));
            metadata.Add(new NativeSubrutineDefinition("UO.DeleteJournal", (Action)DeleteJournal));
            metadata.Add(new NativeSubrutineDefinition("UO.DeleteJournal", (Action<string>)DeleteJournal));
            metadata.Add(new NativeSubrutineDefinition("UO.Journal", (Func<int, string>)GetJournalText));
            metadata.Add(new NativeSubrutineDefinition("UO.JournalSerial", (Func<int, string>)JournalSerial));
            metadata.Add(new NativeSubrutineDefinition("UO.JournalColor", (Func<int, string>)JournalColor));
            metadata.Add(new NativeSubrutineDefinition("UO.SetJournalLine", (Action<int>)SetJournalLine));
            metadata.Add(new NativeSubrutineDefinition("UO.SetJournalLine", (Action<int, string>)SetJournalLine));

            metadata.Add(new NativeSubrutineDefinition("UO.Arm", (Action<string>)Arm));
            metadata.Add(new NativeSubrutineDefinition("UO.SetArm", (Action<string>)SetArm));
            metadata.Add(new NativeSubrutineDefinition("UO.Disarm", (Action)Disarm));
            metadata.Add(new NativeSubrutineDefinition("UO.Unequip", (Action<string>)Unequip));
            metadata.Add(new NativeSubrutineDefinition("UO.Equip", (Action<string, int>)Equip));
            metadata.Add(new NativeSubrutineDefinition("UO.Equip", (Action<string, string>)Equip));
            metadata.Add(new NativeSubrutineDefinition("UO.ObjAtLayer", (Func<string, InjectionValue>)ObjAtLayer));

            metadata.Add(new NativeSubrutineDefinition("UO.WarMode", (Action<int>)WarMode));
            metadata.Add(new NativeSubrutineDefinition("UO.WarMode", (Func<int>)WarMode));

            metadata.Add(new NativeSubrutineDefinition("UO.UseSkill", (Action<string>)UseSkill));
            metadata.Add(new NativeSubrutineDefinition("UO.Cast", (Action<string>)Cast));
            metadata.Add(new NativeSubrutineDefinition("UO.Cast", (Action<string, string>)Cast));

            metadata.Add(new NativeSubrutineDefinition("UO.Morph", (Action<string>)Morph));
            metadata.Add(new NativeSubrutineDefinition("UO.Morph", (Action<int>)Morph));

            metadata.Add(new NativeSubrutineDefinition("UO.Timer", (Func<int>)Timer));
            metadata.Add(new NativeSubrutineDefinition("UO.Time", (Func<int>)Time));
            metadata.Add(new NativeSubrutineDefinition("UO.Date", (Func<int>)Date));

            metadata.Add(new NativeSubrutineDefinition("UO.Terminate", (Action<string>)Terminate));
            metadata.Add(new NativeSubrutineDefinition("UO.Random", (Func<int, int>)Random));

            metadata.Add(new NativeSubrutineDefinition("UO.PrivateGetTile", (Func<int, int, int, int, int, string>)PrivateGetTile));
            metadata.Add(new NativeSubrutineDefinition("UO.Snap", (Action<string>)Snap));
            metadata.Add(new NativeSubrutineDefinition("UO.Snap", (Action)Snap));
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

        private int GetObject(InjectionValue name) => GetObject((string)name);

        private int GetObject(string name)
        {
            var id = injectionApi.GetObject(name);
            if (id == 0)
                bridge.Error($"Unknown object {name}");

            return id;
        }

        public int GetX() => GetX("self");
        public int GetX(string id) => GetX(GetObject(id));
        public int GetX(int id) => bridge.GetX(id);

        public int GetY() => GetY("self");
        public int GetY(string id) => GetY(GetObject(id));
        public int GetY(int id) => bridge.GetY(id);

        public int GetZ() => GetZ("self");
        public int GetZ(string id) => GetZ(GetObject(id));
        public int GetZ(int id) => bridge.GetZ(id);

        public int GetDistance(string id) => GetDistance(GetObject(id));
        public int GetDistance(int id) => bridge.GetDistance(id);
        public int GetDistance(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            return Math.Max(dx, dy);
        }

        public int GetHP() => GetHP("self");
        public int GetHP(string id) => GetHP(GetObject(id));
        public int GetHP(int id) => bridge.GetHP(id);

        public int GetMaxHP() => GetHP("self");
        public int GetMaxHP(string id) => GetMaxHP(GetObject(id));
        public int GetMaxHP(int id) => bridge.GetMaxHP(id);

        public int GetNotoriety(string id) => GetNotoriety(GetObject(id));
        public int GetNotoriety(int id) => bridge.GetNotoriety(id);

        private string GetInfo(string arg)
        {
            if (arg.Equals("character", StringComparison.Ordinal))
                return GetName("self");

            return "UNKNOWN";
        }

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

        public int Exists(string id) => Exists(GetObject(id));
        public int Exists(int id) => bridge.Exists(id);
        public string GetSerial(string id) => NumberConversions.Int2Hex(GetObject(id));
        public int IsOnline() => bridge.IsOnline();
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

        public void UseType(string type) => UseType(NumberConversions.ToInt(type));
        public void UseType(string type, string color) => UseType(NumberConversions.ToInt(type), NumberConversions.ToInt(color));
        public void UseType(int type, string color) => UseType(type, NumberConversions.ToInt(color));
        public void UseType(string type, int color) => UseType(NumberConversions.ToInt(type), color);
        public void UseType(int type) => UseType(type, -1);
        public void UseType(int type, int color) => bridge.UseType(type, color);

        public void WaitTargetObject(string id) => WaitTargetObject(GetObject(id));
        public void WaitTargetObject(int id) => bridge.WaitTargetObject(id);
        public void WaitTargetObject(string id1, string id2) => WaitTargetObject(GetObject(id1), GetObject(id2));
        public void WaitTargetObject(int id1, int id2) => bridge.WaitTargetObject(id1, id2);
        public void WaitTargetSelf(string id /* ignored */) => WaitTargetObject("self");
        public void WaitTargetSelf() => WaitTargetObject("self");
        public void WaitTargetLast(string id /* ignored */) => WaitTargetObject("lasttarget");
        public void WaitTargetLast() => WaitTargetObject("lasttarget");
        public void WaitTargetTile(int type, int x, int y, int z) => bridge.WaitTargetTile(type, x, y, z);
        public void WaitTargetTile(string type, string x, string y, string z) 
            => WaitTargetTile(NumberConversions.ToInt(type), NumberConversions.ToInt(x), NumberConversions.ToInt(y), NumberConversions.ToInt(z));
        public int IsTargeting() => bridge.IsTargeting();

        public void SetReceivingContainer(string id) => SetReceivingContainer(GetObject(id));
        public void SetReceivingContainer(int id) => bridge.SetReceivingContainer(id);
        public void UnsetReceivingContainer() => bridge.UnsetReceivingContainer();
        public void Grab(int amount, string id) => Grab(amount, GetObject(id));
        public void Grab(string amount, string id) => Grab(NumberConversions.ToInt(amount), GetObject(id));
        public void Grab(int amount, int id) => bridge.Grab(amount, id);

        public void MoveItem(int id, int amount) => MoveItem(id, amount, 0);
        public void MoveItem(string id, int amount) => MoveItem(GetObject(id), amount, 0);
        public void MoveItem(string id, string amount) => MoveItem(GetObject(id), NumberConversions.ToInt(amount), 0);
        public void MoveItem(string id, string amount, string targetContainerId)
            => MoveItem(GetObject(id), NumberConversions.ToInt(amount), GetObject(targetContainerId));
        public void MoveItem(int id, int amount, int targetContainerId) => bridge.MoveItem(id, amount, targetContainerId);

        public void FindType(string typeStr) => FindType(NumberConversions.ToInt(typeStr));
        public void FindType(int type) => FindType(type, -1, -1);
        public void FindType(int type, int color) => FindType(type, color, -1);
        public void FindType(string type, string color, string container)
            => FindType(NumberConversions.ToInt(type), NumberConversions.ToInt(color), ConvertContainer(container));
        public void FindType(int type, int color, string container)
            => FindType(type, color, ConvertContainer(container));
        public void FindType(int type, int color, int containerId) => bridge.FindType(type, color, containerId, -1);
        public void FindType(string type, int color, int containerId)
            => bridge.FindType(NumberConversions.ToInt(type), color, containerId, -1);

        public void FindType(InjectionValue type, InjectionValue color, InjectionValue containerId, InjectionValue range)
            => bridge.FindType(NumberConversions.ToInt(type),
                NumberConversions.ToInt(color),
                ConvertContainer(containerId),
                NumberConversions.ToInt(range));
        public int FindCount() => bridge.FindCount();
        public int Count(string type) => bridge.Count(NumberConversions.ToInt(type), -1, -1);
        public int Count(int type) => bridge.Count(type, -1, -1);
        public int Count(int type, int color) => bridge.Count(type, color, 01);
        public int Count(InjectionValue type, InjectionValue color, InjectionValue container) => 
            bridge.Count(NumberConversions.ToInt(type), NumberConversions.ToInt(color), ConvertContainer(container));

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
        public void CharPrint(string color, string msg) => CharPrint(bridge.Self, NumberConversions.ToInt(color), msg);
        public void CharPrint(int id, int color, string msg) => bridge.CharPrint(id, color, msg);
        public void CharPrint(InjectionValue id, InjectionValue color, InjectionValue msg) 
            => CharPrint(GetObject(id), NumberConversions.ToInt(color), (string)msg);

        public int InJournal(string pattern) => bridge.InJournal(pattern);
        public int InJournalBetweenTimes(string pattern, int startTime, int endTime) => InJournalBetweenTimes(pattern, startTime, endTime, -1);
        public int InJournalBetweenTimes(string pattern, int startTime, int endTime, int limit) => bridge.InJournalBetweenTimes(pattern, startTime, endTime, limit);
        public void DeleteJournal() => bridge.DeleteJournal();
        public void DeleteJournal(string text) => bridge.DeleteJournal(text);
        public string GetJournalText(int index) => bridge.GetJournalText(index);
        public string JournalSerial(int index) => bridge.JournalSerial(index);
        public string JournalColor(int index) => bridge.JournalColor(index);
        public void SetJournalLine(int index) => bridge.SetJournalLine(index);
        public void SetJournalLine(int index, string text) => bridge.SetJournalLine(index);

        public void Arm(string name) => bridge.Arm(name);
        public void SetArm(string name) => bridge.SetArm(name);
        public void Disarm()
        {
            Unequip("Lhand");
            Unequip("Rhand");
        }

        public void Unequip(string layer) => bridge.Unequip(layer);
        public void Equip(string layer, int id) => bridge.Equip(layer, id);
        public void Equip(string layer, string id) => Equip(layer, GetObject(id));
        public InjectionValue ObjAtLayer(string layer)
        {
            var id = bridge.ObjAtLayer(layer);
            if (id == 0)
                return InjectionValue.Zero;

            return new InjectionValue("0x" + id.ToString("X8"));
        }

        public void WarMode(int mode) => bridge.WarMode(mode);
        public int WarMode() => bridge.WarMode();

        public void UseSkill(string skillName) => bridge.UseSkill(skillName);
        public void Cast(string spellName) => bridge.Cast(spellName);
        public void Cast(string spellName, string id) => Cast(spellName, GetObject(id));
        public void Cast(string spellName, int id) => bridge.Cast(spellName, id);

        public void Morph(string type) => Morph(NumberConversions.ToInt(type));
        public void Morph(int type) => bridge.Morph(type);
        public void Terminate(string subrutineName) => bridge.Terminate(subrutineName);

        public string PrivateGetTile(int x, int y, int unknown, int minTile, int maxTile)
            => bridge.PrivateGetTile(x, y, unknown, minTile, maxTile);
        public void Snap(string name) => bridge.Snap(name);
        public void Snap() => bridge.Snap(null);

        public int Timer() => (int)timeSource.SinceStart.TotalMilliseconds / 100;
        public int Date()
        {
            var now = timeSource.Now;
            return (now.Year % 100) * 10000 + now.Month * 100 + now.Day;
        }

        public int Time()
        {
            var now = timeSource.Now;
            return now.Hour * 10000 + now.Minute * 100 + now.Second;
        }

        public int Random(int max) => random.Next(max);

        private int ConvertContainer(InjectionValue containerId)
        {
            switch (containerId.Kind)
            {
                case InjectionValueKind.String:
                    return ConvertContainer(containerId.String);
                case InjectionValueKind.Integer:
                    return containerId.Integer;
                default:
                    throw new NotImplementedException($"Conversion for {containerId.Kind})");
            }
        }

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
