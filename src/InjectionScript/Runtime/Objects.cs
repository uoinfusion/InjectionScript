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
        private readonly object objectsLock = new object();

        public event Action<string> KeyAdded;
        public event Action<string> KeyRemoved;

        public void Set(string name, int value)
        {
            bool notify = true;

            lock (objectsLock)
            {
                if (!objects.ContainsKey(name))
                {
                    objects.Add(name, value);
                    notify = true;
                }
                else
                    objects[name] = value;
            }

            if (notify)
                KeyAdded?.Invoke(name);
        }

        public int Get(string name)
        {
            lock (objectsLock)
            {
                return objects[name];
            }
        }

        public void Clear()
        {
            lock (objectsLock)
            {
                objects.Clear();
            }
        }

        public bool TryGet(string name, out int value)
        {
            lock (objectsLock)
            {
                return objects.TryGetValue(name, out value);
            }
        }

        public void Remove(string name)
        {
            bool notify = false;
            lock (objectsLock)
            {
                if (objects.ContainsKey(name))
                {
                    notify = true;
                    objects.Remove(name);
                }
            }

            if (notify)
                KeyRemoved?.Invoke(name);
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            lock (objectsLock)
            {
                return objects.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (objectsLock)
            {
                return objects.GetEnumerator();
            }
        }
    }
}
