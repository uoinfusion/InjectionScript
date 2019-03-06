namespace InjectionScript.Runtime
{
    public interface IApiBridge
    {
        int FindItem { get; }
        int Self { get; }
        int LastCorpse { get; }
        int LastStatus { get; }
        int LastTarget { get; }
        int Backpack { get; }

        int Strength { get; }
        int Intelligence { get; }
        int Dexterity { get; }
        int Stamina { get; }
        int Mana { get; }
        int Weight { get; }
        int Gold { get; }

        void Wait(int ms);
        void SetFindDistance(int distance);
        void SetGrabDelay(int valueInt);

        int GetX(int id);
        int GetY(int id);
        int GetZ(int id);

        int GetDistance(int id);

        int GetHP(int id);
        int GetMaxHP(int id);
        int GetNotoriety(int id);
        string GetName(int id);
        int IsNpc(int id);

        int GetQuantity(int id);

        int Exists(int id);
        int IsOnline();
        int Dead();
        int Hidden(int id);

        void AddObject(string name);
        void Click(int id);
        void UseObject(int id);
        void Attack(int id);
        void GetStatus(int id);
        void UseType(int type, int color);
        void WaitTargetObject(int id);
        void WaitTargetObject(int id1, int id2);
        void WaitTargetTile(int type, int x, int y, int z);
        int IsTargeting();
        void SetReceivingContainer(int id);
        void UnsetReceivingContainer();
        void Grab(int amount, int id);
        void MoveItem(int id, int amount, int targetContainerId);
        void ReceiveObjectName(int id, int delay);

        void LClick(int x, int y);
        void KeyPress(int key);
        void Say(string message);

        void PlayWav(string file);

        void TextOpen();
        void TextPrint(string text);
        void TextClear();

        void ServerPrint(string message);
        void Print(string msg);
        void CharPrint(int id, int color, string msg);

        int InJournal(string pattern);
        int InJournalBetweenTimes(string pattern, int startTime, int endTime, int limit);
        void DeleteJournal();
        void DeleteJournal(string text);
        string GetJournalText(int index);
        string JournalSerial(int index);
        string JournalColor(int index);
        void SetJournalLine(int index);
        void Arm(string name);
        void SetArm(string name);
        void Equip(int layer, int id);
        void Unequip(int layer);
        int ObjAtLayer(int layer);
        void WarMode(int mode);
        int WarMode();
        void UseSkill(string skillName);
        void Cast(string spellName, int id);
        void Morph(int type);
        void Cast(string spellName);

        int FindCount();
        int FindType(int type, int color, int containerId, int range);
        int Count(int type, int color, int containerId);
        void Ignore(int id);
        void IgnoreReset();
        void Terminate(string subrutineName);
        int GetGraphics(int id);
        int GetDir(int id);
        int GetColor(int id);
        int GetLayer(int id);

        void WaitGump(int triggerId);
        void SendGumpSelect(int triggerId);

        void MakeStepByKey(int key);

        string PrivateGetTile(int x, int y, int unknown, int minTile, int maxTile);
        void Snap(string name);

        void Error(string message);

        void Exec(string subrutineName);
    }
}
