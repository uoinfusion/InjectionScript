using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript
{
    public class Message
    {
        public int StartLine { get; }
        public int StartColumn { get; }
        public int EndLine { get; }
        public int EndColumn { get; }
        public string Text { get; }
        public string Code { get; }
        public MessageSeverity Severity { get; }

        public Message(int startLine, int startColumn, int endLine, int endColumn, MessageSeverity severity, string code, string text)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
            Text = text;
            Code = code;
            Severity = severity;
        }

        public Message(int startLine, int startColumn, MessageSeverity severity, string code, string text)
            : this(startLine, startColumn, startLine, startColumn + 1, severity, code, text)
        {
        }

        public bool IsCode(string code) 
            => Code.Equals(code, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => $"{Severity} {Code}: {StartLine}, {StartColumn} - {Text}";
    }
}
