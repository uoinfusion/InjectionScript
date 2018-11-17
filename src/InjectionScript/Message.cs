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
        public MessageSeverity Severity { get; }

        public Message(int line, int charPos, string text, MessageSeverity severity)
        {
            Line = line;
            CharPos = charPos;
            Text = text;
            Severity = severity;
        }
    }
}
