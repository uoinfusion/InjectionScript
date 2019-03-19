using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public sealed class Paths
    {
        private readonly Func<string> scriptPath;

        public Paths(Func<string> scriptPath)
        {
            this.scriptPath = scriptPath;
        }

        public string ScriptPath => scriptPath();
    }
}
