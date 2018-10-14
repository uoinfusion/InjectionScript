using InjectionScript.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.ParseFile(@"c:\Users\jakub\sources\ultima\injscripts\jooky\Mining2016.sc");
        }
    }
}
