namespace InjectionScript.Runtime
{
    public interface IApiBridge
    {
        int FindItem { get; }
        int Self { get; }
        int LastCorpse { get; }
        int LastStatus { get; }
        int LastTarget { get; }
        int Strength { get; }
        int Intelligence { get; }
        int Dexterity { get; }
        int Stamina { get; }
        int Mana { get; }
        int Weight { get; }
        int Gold { get; }

        void Wait(int ms);
        void SetFindDistance(int distance);

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

        int Dead();
        int Hidden(int id);

        void AddObject(string name);
        void Click(int id);
        void UseObject(int id);
        void Attack(int id);
        void GetStatus(int id);
        void UseType(int type);
        void WaitTargetObject(int id);
        void WaitTargetObject(int id1, int id2);
        int IsTargeting();
        void SetReceivingContainer(int id);
        void Grab(int amount, int id);

        void LClick(int x, int y);
        void KeyPress(int key);
        void Say(string message);

        void PlayWav(string file);

        void TextOpen();
        void TextPrint(string text);

        void ServerPrint(string message);
        void Print(string msg);
        void ClientPrint(int id, int color, string msg);
        int InJournal(string pattern);
        void DeleteJournal();
        string GetJournalText(int index);
        string JournalSerial(int index);
        void SetJournalLine(int index);
        void Arm(string name);
        void SetArm(string name);
        void WarMode(int mode);
        int WarMode();
        void UseSkill(string skillName);
        void Cast(string spellName, int id);
        void Morph(int type);
        void Cast(string spellName);

        int FindCount();
        void FindType(int type, int color, int containerId);
        int Count(int type);
        void Ignore(int id);
        void IgnoreReset();
        void Terminate(string subrutineName);
        int GetGraphics(int id);
        int GetDir(int id);
    }
}
