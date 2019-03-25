using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class Objects : IEnumerable<KeyValuePair<string, int>>
    {
        private readonly Dictionary<string, int> objects = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public void Set(string name, int value) => objects[name] = value;
        public int Get(string name) => objects[name];
        public void Clear() => objects.Clear();

        public bool TryGet(string name, out int value) => objects.TryGetValue(name, out value);
        public void Remove(string name) => objects.Remove(name);

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }
    }
}
