using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Parsing
{
    public sealed class ScriptFile
    {
        private readonly string[] lines;

        public injectionParser.FileContext Syntax { get; }
        public string FileName { get; }
        public string Content { get; }

        public ScriptFile(string fileName, string content, injectionParser.FileContext syntax)
        {
            Syntax = syntax;
            FileName = fileName;
            Content = content;

            lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public string GetLine(int line) => lines[line];
    }
}
