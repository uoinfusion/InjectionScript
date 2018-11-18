using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class InjectionApi
    {
        private readonly IApiBridge bridge;
        private readonly Objects objects = new Objects();

        public InjectionApi(IApiBridge bridge)
        {
            this.bridge = bridge;
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

        public int GetObject(string id)
        {
            if (id.Equals("finditem", StringComparison.OrdinalIgnoreCase))
                return bridge.FindItem;
            if (id.Equals("self", StringComparison.OrdinalIgnoreCase))
                return bridge.Self;
            if (id.Equals("lastcorpse", StringComparison.OrdinalIgnoreCase))
                return bridge.LastCorpse;
            if (id.Equals("lasttarget", StringComparison.OrdinalIgnoreCase))
                return bridge.LastTarget;
            if (id.Equals("laststatus", StringComparison.OrdinalIgnoreCase))
                return bridge.LastStatus;

            if (objects.TryGet(id, out var value))
                return value;

            if (NumberConversions.TryStr2Int(id, out value))
                return value;

            return 0;
        }

        public void SetObject(string name, int value)
        {
            objects.Set(name, value);
        }

        private int ConvertContainer(string id)
        {
            if (id.Equals("my", StringComparison.OrdinalIgnoreCase))
                return -1;
            else if (id.Equals("ground", StringComparison.OrdinalIgnoreCase))
                return 1;

            return GetObject(id);
        }

        public void Wait(int ms) => bridge.Wait(ms);

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

        public int IsNpc(string id) => IsNpc(GetObject(id));
        public int IsNpc(int id) => bridge.IsNpc(id);

        public int GetQuantity(string id) => GetQuantity(GetObject(id));
        public int GetQuantity(int id) => bridge.GetQuantity(id);

        public string GetSerial(string id) => NumberConversions.Int2Hex(GetObject(id));
        public int Dead() => bridge.Dead();
        public int Hidden() => Hidden("self");
        public int Hidden(string idText) => Hidden(GetObject(idText));
        public int Hidden(int id) => bridge.Hidden(id);

        public void AddObject(string name, int id) => objects.Set(name, id);
        public void AddObject(string name) => bridge.AddObject(name);

        public int Str() => bridge.Strength;
        public int Int() => bridge.Intelligence;
        public int Dex() => bridge.Dexterity;
        public int Stamina() => bridge.Stamina;
        public int Mana() => bridge.Mana;
        public int Weight() => bridge.Weight;
        public int Gold() => bridge.Gold;

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

        public void KeyPress(int key) => bridge.KeyPress(key);
        public void Say(string message) => bridge.Say(message);
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
    }
}
