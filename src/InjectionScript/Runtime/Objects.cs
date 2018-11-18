using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class Objects
    {
        private readonly Dictionary<string, int> objects = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public void Set(string name, int value) => objects[name] = value;
        public int Get(string name) => objects[name];

        public bool TryGet(string name, out int value) => objects.TryGetValue(name, out value);
    }
}
