using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript
{
    public class Message
    {
        public int Line { get; }
        public int CharPos { get; }
        public string Text { get; }
        public string Code { get; }
        public MessageSeverity Severity { get; }

        public Message(int line, int charPos, MessageSeverity severity, string code, string text)
        {
            Line = line;
            CharPos = charPos;
            Text = text;
            Code = code;
            Severity = severity;
        }

        public bool IsCode(string code) 
            => Code.Equals(code, StringComparison.OrdinalIgnoreCase);
    }
}
