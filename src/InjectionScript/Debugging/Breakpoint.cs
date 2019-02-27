using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    public class Breakpoint
    {
        public string FileName { get; }
        public int Line { get; }

        public Breakpoint(string fileName, int line)
        {
            FileName = fileName;
            Line = line;
        }

        public override string ToString() => $"{FileName}, {Line}";
    }
}
