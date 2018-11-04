namespace InjectionScript.Parsing
{
    public struct SyntaxError
    {
        public int Line { get; }
        public int CharPos { get; }
        public string Message { get; }

        public SyntaxError(int line, int charPos, string message)
        {
            Line = line;
            CharPos = charPos;
            Message = message;
        }
    }
}
