using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Paths paths;
        private readonly Objects objects = new Objects();

        private static readonly Dictionary<string, int> layerNameToNumber = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"Rhand", 0x01 },
            {"Lhand", 0x02 },
            {"Shoes", 0x03 },
            {"Pants", 0x04 },
            {"Shirt", 0x05 },
            {"Hat", 0x06 },
            {"Gloves", 0x07 },
            {"Ring", 0x08 },
            {"Neck", 0x09 },
            {"Hair", 0x0B },
            {"Waist", 0x0C },
            {"Torso", 0x0D },
            {"Brace", 0x0E },
            {"Beard", 0x10 },
            {"Ear", 0x12 },
            {"Arms", 0x13 },
            {"Bpack", 0x15 },
            {"Bank", 0x1D },
            {"Rstk", 0x1A },
            {"NRstk", 0x1B },
            { "Sell", 0x1C },
        };

        private static readonly Dictionary<int, string> layerNumberToName = new Dictionary<int, string>();

        static InjectionApiUO()
        {
            foreach (var pair in layerNameToNumber)
                layerNumberToName.Add(pair.Value, pair.Key);
        }

        internal InjectionApiUO(IApiBridge bridge, InjectionApi injectionApi, Metadata metadata, Globals globals, ITimeSource timeSource, Paths paths)
        {
            this.bridge = bridge;
            this.injectionApi = injectionApi;
            this.globals = globals;
            Register(metadata);
            random = new Random();
            this.timeSource = timeSource;
            this.paths = paths;
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
            metadata.Add(new NativeSubrutineDefinition("UO.GetDistance", (Func<InjectionValue, InjectionValue, InjectionValue, int>)GetDistance));
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
            metadata.Add(new NativeSubrutineDefinition("UO.GetName", (Func<string>)GetName));
            metadata.Add(new NativeSubrutineDefinition("UO.GetName", (Func<int, string>)GetName));
            metadata.Add(new NativeSubrutineDefinition("UO.GetName", (Func<string, string>)GetName));

            metadata.Add(new NativeSubrutineDefinition("UO.GetGraphic", (Func<string, int>)GetGraphics));
            metadata.Add(new NativeSubrutineDefinition("UO.GetGraphic", (Func<int, int>)GetGraphics));

            metadata.Add(new NativeSubrutineDefinition("UO.GetColor", (Func<InjectionValue, string>)GetColor));
            metadata.Add(new NativeSubrutineDefinition("UO.GetLayer", (Func<InjectionValue, string>)GetLayer));

            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<string, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<int, int>)GetDir));
            metadata.Add(new NativeSubrutineDefinition("UO.GetDir", (Func<int>)GetDir));

            metadata.Add(new NativeSubrutineDefinition("UO.IsNpc", (Func<int, int>)IsNpc));
            metadata.Add(new NativeSubrutineDefinition("UO.IsNpc", (Func<string, int>)IsNpc));

            metadata.Add(new NativeSubrutineDefinition("UO.Exists", (Func<int, int>)Exists));
            metadata.Add(new NativeSubrutineDefinition("UO.Exists", (Func<string, int>)Exists));

            metadata.Add(new NativeSubrutineDefinition("UO.GetSerial", (Func<string, string>)GetSerial));
            metadata.Add(new NativeSubrutineDefinition("UO.GetQuantity", (Func<string, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.GetQuantity", (Func<int, int>)GetQuantity));
            metadata.Add(new NativeSubrutineDefinition("UO.IsOnline", (Func<int>)IsOnline));
            metadata.Add(new NativeSubrutineDefinition("UO.Dead", (Func<int>)Dead));
            metadata.Add(new NativeSubrutineDefinition("UO.Dead", (Func<InjectionValue, int>)Dead));
            metadata.Add(new NativeSubrutineDefinition("UO.Hidden", (Func<int>)Hidden));
            metadata.Add(new NativeSubrutineDefinition("UO.Hidden", (Func<string, int>)Hidden));

            metadata.Add(new NativeSubrutineDefinition("UO.AddObject", (Action<string, int>)AddObject));
            metadata.Add(new NativeSubrutineDefinition("UO.AddObject", (Action<string>)AddObject));
            metadata.Add(new NativeSubrutineDefinition("UO.DeleteObject", (Action<string>)DeleteObject));

            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Str", (Func<int>)Str));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Int", (Func<int>)Int));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Dex", (Func<int>)Dex));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Stamina", (Func<int>)Stamina));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Mana", (Func<int>)Mana));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Weight", (Func<int>)Weight));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Gold", (Func<int>)Gold));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("UO.Life", (Func<int>)Life));

            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Func<InjectionValue, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Func<InjectionValue, InjectionValue, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Func<InjectionValue, InjectionValue, InjectionValue, string>)FindType));
            metadata.Add(new NativeSubrutineDefinition("UO.FindType", (Func<InjectionValue, InjectionValue, InjectionValue, InjectionValue, string>)FindType));
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
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<InjectionValue, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<InjectionValue, InjectionValue, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Count", (Func<InjectionValue, InjectionValue, InjectionValue, int>)Count));
            metadata.Add(new NativeSubrutineDefinition("UO.Click", (Action<string>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.Click", (Action<int>)Click));
            metadata.Add(new NativeSubrutineDefinition("UO.UseObject", (Action<string>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.UseObject", (Action<int>)UseObject));
            metadata.Add(new NativeSubrutineDefinition("UO.Attack", (Action<string>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.Attack", (Action<int>)Attack));
            metadata.Add(new NativeSubrutineDefinition("UO.GetStatus", (Action<string>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.GetStatus", (Action<int>)GetStatus));
            metadata.Add(new NativeSubrutineDefinition("UO.ReceiveObjectName", (Action<InjectionValue>)ReceiveObjectName));
            metadata.Add(new NativeSubrutineDefinition("UO.ReceiveObjectName", (Action<InjectionValue, InjectionValue>)ReceiveObjectName));
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
            metadata.Add(new NativeSubrutineDefinition("UO.WaitTargetTile", (Action<InjectionValue, InjectionValue, InjectionValue, InjectionValue>)WaitTargetTile));
            metadata.Add(new NativeSubrutineDefinition("UO.Targeting", (Func<int>)IsTargeting));

            metadata.Add(new NativeSubrutineDefinition("UO.LastTile", (Func<string>)LastTile));
            metadata.Add(new NativeSubrutineDefinition("UO.LastTile", (Func<InjectionValue, int>)LastTile));

            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<int, int>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<int, string>)Grab));
            metadata.Add(new NativeSubrutineDefinition("UO.Grab", (Action<string, string>)Grab));

            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<InjectionValue, InjectionValue>)MoveItem));
            metadata.Add(new NativeSubrutineDefinition("UO.MoveItem", (Action<InjectionValue, InjectionValue, InjectionValue>)MoveItem));

            metadata.Add(new NativeSubrutineDefinition("UO.SetReceivingContainer", (Action<int>)SetReceivingContainer));
            metadata.Add(new NativeSubrutineDefinition("UO.SetReceivingContainer", (Action<string>)SetReceivingContainer));
            metadata.Add(new NativeSubrutineDefinition("UO.UnsetReceivingContainer", (Action)UnsetReceivingContainer));
            
            metadata.Add(new NativeSubrutineDefinition("UO.LClick", (Action<int, int>)LClick));
            metadata.Add(new NativeSubrutineDefinition("UO.KeyPress", (Action<int>)KeyPress));
            metadata.Add(new NativeSubrutineDefinition("UO.KeyPress", (Action<int, int>)KeyPress));
            metadata.Add(new NativeSubrutineDefinition("UO.KeyPress", (Action<int, int, int>)KeyPress));
            metadata.Add(new NativeSubrutineDefinition("UO.Press", (Action<int>)Press));
            metadata.Add(new NativeSubrutineDefinition("UO.Press", (Action<int, int>)Press));
            metadata.Add(new NativeSubrutineDefinition("UO.Press", (Action<int, int, int>)Press));
            metadata.Add(new NativeSubrutineDefinition("UO.Say", (Action<string>)Say));

            metadata.Add(new NativeSubrutineDefinition("UO.PlayWav", (Action<string>)PlayWav));
            metadata.Add(new NativeSubrutineDefinition("UO.Launch", (Action<string>)Launch));

            metadata.Add(new NativeSubrutineDefinition("UO.TextOpen", (Action)TextOpen));
            metadata.Add(new NativeSubrutineDefinition("UO.TextPrint", (Action<string>)TextPrint));
            metadata.Add(new NativeSubrutineDefinition("UO.TextClear", (Action)TextClear));

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
            metadata.Add(new NativeSubrutineDefinition("UO.UseSkill", (Action<InjectionValue, InjectionValue>)UseSkill));
            metadata.Add(new NativeSubrutineDefinition("UO.SkillVal", (Func<string, int>)SkillVal));

            metadata.Add(new NativeSubrutineDefinition("UO.Cast", (Action<string>)Cast));
            metadata.Add(new NativeSubrutineDefinition("UO.Cast", (Action<InjectionValue, InjectionValue>)Cast));

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

            metadata.Add(new NativeSubrutineDefinition("UO.PMove", (Action <int, int>)PMove));
            metadata.Add(new NativeSubrutineDefinition("UO.PMove", (Action <int, int, int>)PMove));

            metadata.Add(new NativeSubrutineDefinition("UO.WaitGump", (Action<int>)WaitGump));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitGump", (Action<string>)WaitGump));
            metadata.Add(new NativeSubrutineDefinition("UO.SendGumpSelect", (Action<int>)SendGumpSelect));
            metadata.Add(new NativeSubrutineDefinition("UO.SendGumpSelect", (Action<string>)SendGumpSelect));

            metadata.Add(new NativeSubrutineDefinition("UO.WaitMenu", (Action<string, string>)WaitMenu));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitMenu", (Action<string, string, string, string>)WaitMenu));
            metadata.Add(new NativeSubrutineDefinition("UO.WaitMenu", (Action<string, string, string, string, string, string>)WaitMenu));

            metadata.Add(new NativeSubrutineDefinition("UO.SaveConfig", (Action)SaveConfig));
        }

        public void WaitGump(string triggerId) => WaitGump(NumberConversions.ToInt(triggerId));
        public void WaitGump(int triggerId) => bridge.WaitGump(triggerId);

        public void SendGumpSelect(string triggerId) => SendGumpSelect(NumberConversions.ToInt(triggerId));
        public void SendGumpSelect(int triggerId) => bridge.SendGumpSelect(triggerId);

        public void WaitMenu(string prompt, string choice)
            => bridge.WaitMenu(prompt, choice);
        public void WaitMenu(string prompt1, string choice1, string prompt2, string choice2)
            => bridge.WaitMenu(prompt1, choice1, prompt2, choice2);
        public void WaitMenu(string prompt1, string choice1, string prompt2, string choice2, string prompt3, string choice3)
            => bridge.WaitMenu(prompt1, choice1, prompt2, choice2, prompt3, choice3);

        private int GetDirection(int currentX, int currentY, int targetX, int targetY)
        {
            if (targetX < currentX && targetY == currentY)
                return 6;
            else if (targetX > currentX && targetY == currentY)
                return 2;
            else if (targetX == currentX && targetY > currentY)
                return 4;
            else if (targetX == currentX && targetY < currentY)
                return 0;
            else if (targetX < currentX && targetY > currentY)
                return 5;
            else if (targetX > currentX && targetY < currentY)
                return 1;
            else if (targetX < currentX && targetY < currentY)
                return 7;
            else if (targetX > currentX && targetY > currentY)
                return 3;

            throw new NotImplementedException($"Unknown direction for {currentX}, {currentY} -> {targetX}, {targetY}");
        }

        private int GetKey(int direction)
        {
            if (direction == 0)
                return 33;
            else if (direction == 1)
                return 39;
            else if (direction == 2)
                return 34;
            else if (direction == 3)
                return 40;
            else if (direction == 4)
                return 35;
            else if (direction == 5)
                return 37;
            else if (direction == 6)
                return 36;
            else if (direction == 7)
                return 38;

            throw new NotImplementedException(direction.ToString());
        }

        public void PMove(int x, int y, int z) => PMove(x, y);
        public void PMove(int targetX, int targetY)
        {
            var currentX = GetX();
            var currentY = GetY();

            while (targetX != currentX || targetY != currentY)
            {
                var direction = GetDirection(currentX, currentY, targetX, targetY);
                var key = GetKey(direction);

                bridge.MakeStepByKey(key);

                currentX = GetX();
                currentY = GetY();
            }
        }

        public void Set(string name, int value)
        {
            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
                bridge.SetFindDistance(value);
        }

        public void Set(string name, string valueStr)
        {
            int valueInt = NumberConversions.TryStr2Int(valueStr, out var value) ? value : 0;

            if (name.Equals("finddistance", StringComparison.OrdinalIgnoreCase))
            {
                bridge.SetFindDistance(valueInt);
            }
            else if (name.Equals("grabdelay", StringComparison.OrdinalIgnoreCase))
            {
                bridge.SetGrabDelay(valueInt);
            }
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

        public int GetDistance(InjectionValue id1, InjectionValue x2, InjectionValue y2)
        {
            var obj1 = GetObject(id1);
            int x1 = bridge.GetX(obj1);
            int y1 = bridge.GetY(obj1);

            return GetDistance(x1, y1, NumberConversions.ToInt(x2), NumberConversions.ToInt(y2));
        }

        public int GetHP() => GetHP("self");
        public int GetHP(string id) => GetHP(GetObject(id));
        public int GetHP(int id) => bridge.GetHP(id);

        public int GetMaxHP() => GetMaxHP("self");
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

        public string GetName() => GetName("self");
        public string GetName(string id) => GetName(GetObject(id));
        public string GetName(int id) => bridge.GetName(id);

        public int GetGraphics(string id) => GetGraphics(GetObject(id));
        public int GetGraphics(int id) => bridge.GetGraphics(id);

        public string GetColor(InjectionValue id) => NumberConversions.ToHex((short)bridge.GetColor(GetObject(id)));
        public string GetLayer(InjectionValue id) => ConvertLayer(bridge.GetLayer(GetObject(id)));

        public int GetDir() => GetDir("self");
        public int GetDir(string id) => GetDir(GetObject(id));
        public int GetDir(int id) => bridge.GetDir(id);

        public int IsNpc(string id) => IsNpc(GetObject(id));
        public int IsNpc(int id) => bridge.IsNpc(id);

        public int GetQuantity(string id) => GetQuantity(GetObject(id));
        public int GetQuantity(int id) => bridge.GetQuantity(id);

        public int Exists(string id) => Exists(GetObject(id));
        public int Exists(int id) => bridge.Exists(id);
        public string GetSerial(string id) => NumberConversions.ToHex(GetObject(id));
        public int IsOnline() => bridge.IsOnline();
        public int Dead() => bridge.Dead();
        public int Dead(InjectionValue id)
        {
            if (id.Kind == InjectionValueKind.String && id.String.Equals("self", StringComparison.OrdinalIgnoreCase))
                return Dead();

            return 0;
        }
        public int Hidden() => Hidden("self");
        public int Hidden(string idText) => Hidden(GetObject(idText));
        public int Hidden(int id) => bridge.Hidden(id);

        public void AddObject(string name, int id) => objects.Set(name, id);
        public void AddObject(string name)
        {
            SystemMessage($"What is {name}");
            bridge.AddObject(name);
        }

        public void DeleteObject(string name)
        {
            if (objects.TryGet(name, out int value))
            {
                objects.Remove(name);
                SystemMessage($"Object {name} has been deleted");
            }
            else
                SystemMessage($"Object {name} not found");
        }

        private int GetObject(InjectionValue name) => GetObject((string)name);
        public int GetObject(string name)
        {
            if (name.Equals("finditem", StringComparison.OrdinalIgnoreCase))
                return bridge.FindItem;
            if (name.Equals("self", StringComparison.OrdinalIgnoreCase))
                return bridge.Self;
            if (name.Equals("lastcorpse", StringComparison.OrdinalIgnoreCase))
                return bridge.LastCorpse;
            if (name.Equals("lasttarget", StringComparison.OrdinalIgnoreCase))
                return bridge.LastTarget;
            if (name.Equals("laststatus", StringComparison.OrdinalIgnoreCase))
                return bridge.LastStatus;
            if (name.Equals("backpack", StringComparison.OrdinalIgnoreCase))
                return bridge.Backpack;

            if (objects.TryGet(name, out var value))
                return value;

            if (NumberConversions.TryStr2Int(name, out value))
                return value;

            bridge.Error($"Unknown object {name}");

            return 0;
        }

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
        public void ReceiveObjectName(InjectionValue id) => bridge.ReceiveObjectName(GetObject(id), 0);
        public void ReceiveObjectName(InjectionValue id, InjectionValue timeout) => bridge.ReceiveObjectName(GetObject(id), NumberConversions.ToInt(timeout));

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
        public void WaitTargetTile(InjectionValue type, InjectionValue x, InjectionValue y, InjectionValue z)
            => bridge.WaitTargetTile(NumberConversions.ToInt(type), NumberConversions.ToInt(x), NumberConversions.ToInt(y), NumberConversions.ToInt(z));
        public int IsTargeting() => bridge.IsTargeting();

        public string LastTile()
        {
            var tileInfo = bridge.LastTile() ?? new[] { 0, 0, 0, 0 };

            return $"{tileInfo[0]} {tileInfo[1]} {tileInfo[2]} {tileInfo[3]}";
        }

        public int LastTile(InjectionValue index)
        {
            var i = NumberConversions.ToInt(index);
            if (i < 0 || i > 3)
                return 0;

            return bridge?.LastTile()[i] ?? 0;

        }

        public void SetReceivingContainer(string id) => SetReceivingContainer(GetObject(id));
        public void SetReceivingContainer(int id) => bridge.SetReceivingContainer(id);
        public void UnsetReceivingContainer() => bridge.UnsetReceivingContainer();
        public void Grab(int amount, string id) => Grab(amount, GetObject(id));
        public void Grab(string amount, string id) => Grab(NumberConversions.ToInt(amount), GetObject(id));
        public void Grab(int amount, int id) => bridge.Grab(amount, id);

        public void MoveItem(InjectionValue id, InjectionValue amount)
            => bridge.MoveItem(GetObject(id), NumberConversions.ToInt(amount), 0);
        public void MoveItem(InjectionValue id, InjectionValue amount, InjectionValue targetContainer) 
            => bridge.MoveItem(GetObject(id), NumberConversions.ToInt(amount), ConvertContainer(targetContainer));

        public string FindType(InjectionValue type)
            => FindType(NumberConversions.ToInt(type));
        public string FindType(int type)
            => FindType(type, -1, -1);
        public string FindType(InjectionValue type, InjectionValue color)
            => FindType(NumberConversions.ToInt(type), NumberConversions.ToInt(color), -1, -1);
        public string FindType(InjectionValue type, InjectionValue color, InjectionValue container)
            => FindType(NumberConversions.ToInt(type), NumberConversions.ToInt(color), ConvertContainer(container));
        public string FindType(int type, int color, int containerId)
            => FindType(type, color, containerId, -1);

        public string FindType(InjectionValue type, InjectionValue color, InjectionValue containerId, InjectionValue range)
            => FindType(NumberConversions.ToInt(type),
                NumberConversions.ToInt(color),
                ConvertContainer(containerId),
                NumberConversions.ToInt(range));

        public string FindType(int type, int color, int containerId, int range)
            => NumberConversions.ToHex(bridge.FindType(type, color, containerId, range));

        public int FindCount() => bridge.FindCount();
        public int Count(InjectionValue type) 
            => Count(NumberConversions.ToInt(type), -1, -1);
        public int Count(InjectionValue type, InjectionValue color) 
            => Count(NumberConversions.ToInt(type), NumberConversions.ToInt(color), 0);
        public int Count(InjectionValue type, InjectionValue color, InjectionValue container)
            => Count(NumberConversions.ToInt(type), NumberConversions.ToInt(color), ConvertContainer(container));
        public int Count(int type, int color, int container)
            => bridge.Count(type, color, container);

        public void Ignore(string id) => Ignore(GetObject(id));
        public void Ignore(int id) => bridge.Ignore(id);
        public void IgnoreReset() => bridge.IgnoreReset();

        public void LClick(int x, int y) => bridge.LClick(x, y);
        public void Press(int key) => KeyPress(key);
        public void Press(int key, int count) => KeyPress(key, count);
        public void Press(int key, int count, int delay) => KeyPress(key, count, delay);
        public void KeyPress(int key) => KeyPress(key, 1);
        public void KeyPress(int key, int count) => KeyPress(key, count, 105);
        public void KeyPress(int key, int count, int delay)
        {
            while (count > 0)
            {
                bridge.KeyPress(key);
                bridge.Wait(delay);
                count--;
            }
        }

        public void Say(string message) => bridge.Say(message);

        public void PlayWav(string file) => bridge.PlayWav(file);

        public void Launch(string fileName)
        {
            var info = new ProcessStartInfo(fileName)
            {
                WorkingDirectory = paths.ScriptPath,
                UseShellExecute = true
            };

            try
            {
                Process.Start(info);
            }
            catch (Exception)
            {
                // just swallow it, Injection doesn't show any error message
            }
        }

        public void TextOpen() => bridge.TextOpen();
        public void TextPrint(string text) => bridge.TextPrint(text);
        public void TextClear() => bridge.TextClear();

        public void Msg(string message) => bridge.ServerPrint(message);
        public void ServerPrint(string message) => bridge.ServerPrint(message);

        public void SystemMessage(string msg) => bridge.Print(msg);
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

        public void Unequip(string layer) => bridge.Unequip(ConvertLayer(layer));
        public void Equip(string layer, int id) => bridge.Equip(ConvertLayer(layer), id);
        public void Equip(string layer, string id) => Equip(layer, GetObject(id));
        public InjectionValue ObjAtLayer(string layer)
        {
            var id = bridge.ObjAtLayer(ConvertLayer(layer));
            if (id == 0)
                return InjectionValue.Zero;

            return new InjectionValue("0x" + id.ToString("X8"));
        }

        public void WarMode(int mode) => bridge.WarMode(mode);
        public int WarMode() => bridge.WarMode();

        public void UseSkill(string skillName)
        {
            UseSkillCore(skillName);
        }

        private bool UseSkillCore(string skillName)
        {
            var skillId = ConvertSkillName(skillName);
            if (!skillId.HasValue)
            {
                SystemMessage("Unknown action skill name");
                return false;
            }

            bridge.UseSkill(skillId.Value);

            return true;
        }

        public void UseSkill(InjectionValue skillName, InjectionValue target)
        {
            if (UseSkillCore((string)skillName))
                WaitTargetObject(GetObject(target));
        }

        public int SkillVal(string skillName)
        {
            var skillId = ConvertSkillName(skillName);
            if (skillId.HasValue)
                return bridge.SkillVal(skillId.Value);
            else
                return 0;
        }

        public void Cast(string spellName)
        {
            CastCore(spellName);
        }

        private bool CastCore(string spellName)
        {
            var spellId = ConvertSpellName(spellName);
            if (!spellId.HasValue)
            {
                SystemMessage("Unknown spell name");
                return false;
            }
            bridge.Cast(spellId.Value);

            return true;
        }

        public void Cast(InjectionValue spellName, InjectionValue target)
        {
            if (CastCore((string)spellName))
                WaitTargetObject(GetObject(target));
        }

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

        public void SaveConfig() => Print("SaveConfig is not implemented yet.");

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

        private int ConvertLayer(string layer) => layerNameToNumber[layer];
        private string ConvertLayer(int layer) => layerNumberToName[layer];

        private static readonly Dictionary<string, int> spellNameToSpellId = new Dictionary<string, int>()
        {
            {"Clumsy", 1},
            {"Create Food", 2},
            {"Feeblemind", 3},
            {"Heal", 4},
            {"Magic Arrow", 5},
            {"Night Sight", 6},
            {"Reactive Armor", 7},
            {"Weaken", 8},
            {"Agility", 9},
            {"Cunning", 10},
            {"Cure", 11},
            {"Harm", 12},
            {"Magic Trap", 13},
            {"Magic Untrap", 14},
            {"Protection", 15},
            {"Strength", 16},
            {"Bless", 17},
            {"Fireball", 18},
            {"Magic Lock", 19},
            {"Poison", 20},
            {"Telekenisis", 21},
            {"Teleport", 22},
            {"Unlock", 23},
            {"Wall of Stone", 24},
            {"Arch Cure", 25},
            {"Arch Protection", 26},
            {"Curse", 27},
            {"Fire Field", 28},
            {"Greater Heal", 29},
            {"Lightning", 30},
            {"Mana Drain", 31},
            {"Recall", 32},
            {"Blade Spirits", 33},
            {"Dispel Field", 34},
            {"Incognito", 35},
            {"Reflection", 36},
            {"Mind Blast", 37},
            {"Paralyze", 38},
            {"Poison Field", 39},
            {"Summon Creature", 40},
            {"Dispel", 41},
            {"Energy Bolt", 42},
            {"Explosion", 43},
            {"Invisibility", 44},
            {"Mark", 45},
            {"Mass Curse", 46},
            {"Paralyze Field", 47},
            {"Reveal", 48},
            {"Chain Lightning", 49},
            {"Energy Field", 50},
            {"Flame Strike", 51},
            {"Gate", 52},
            {"Mana Vampire", 53},
            {"Mass Dispel", 54},
            {"Meteor Swarm", 55},
            {"Polymorph", 56},
            {"Earthquake", 57},
            {"Energy Vortex", 58},
            {"Resurrection", 59},
            {"Air Elemental", 60},
            {"Summon Daemon", 61},
            {"Earth Elemental", 62},
            {"Fire Elemental", 63},
            {"Water Elemental", 64}
        };

        private int? ConvertSpellName(string spellName)
        {
            if (spellNameToSpellId.TryGetValue(spellName, out var spellId))
                return spellId;

            return null;
        }

        private static readonly Dictionary<string, int> skillNameToSkillId = new Dictionary<string, int>()
        {
            { "Alchemy", 1 },
            { "Anatomy", 2 },
            { "Animal Lore", 3 },
            { "Item ID", 4 },
            { "Arms Lore", 5 },
            { "Parrying", 6 },
            { "Begging", 7 },
            { "Blacksmithing", 8 },
            { "Bowcraft", 9 },
            { "Peacemaking", 10 },
            { "Camping", 11 },
            { "Carpentry", 12 },
            { "Cartography", 13 },
            { "Cooking", 14 },
            { "Detect Hidden", 15 },
            { "Enticement", 16 },
            { "Evaluate Intelligence", 17 },
            { "Healing", 18 },
            { "Fishing", 19 },
            { "Forensic Evaluation", 20 },
            { "Herding", 21 },
            { "Hiding", 22 },
            { "Provocation", 23 },
            { "Inscription", 24 },
            { "Lockpicking", 25 },
            { "Magery", 26 },
            { "Magic Resistance", 27 },
            { "Tactics", 28 },
            { "Snooping", 29 },
            { "Musicianship", 30 },
            { "Poisoning", 31 },
            { "Archery", 32 },
            { "Spirit Speak", 33 },
            { "Stealing", 34 },
            { "Tailoring", 35 },
            { "Animal Taming", 36 },
            { "Taste Identification", 37 },
            { "Tinkering", 38 },
            { "Tracking", 39 },
            { "Veterinary", 40 },
            { "Swordsmanship", 41 },
            { "Mace Fighting", 42 },
            { "Fencing", 43 },
            { "Wrestling", 44 },
            { "Lumberjacking", 45 },
            { "Mining", 46 },
            { "Meditation", 47 },
            { "Stealth", 48 },
            { "Remove Trap", 49 },
            { "Necromancy", 50 }
        };

        private int? ConvertSkillName(string skillName)
        {
            if (skillNameToSkillId.TryGetValue(skillName, out var skillId))
                return skillId;

            return null;
        }
    }
}
