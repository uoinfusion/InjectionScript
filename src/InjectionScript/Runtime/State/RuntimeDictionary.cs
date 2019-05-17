using System;
using System.Collections;
using System.Collections.Generic;

namespace InjectionScript.Runtime.State
{
    public abstract class RuntimeDictionary<T> : IEnumerable<KeyValuePair<string, T>>
    {
        private readonly Dictionary<string, T> items = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        private readonly object itemsLock = new object();

        public event Action<string> KeyAdded;
        public event Action<string> KeyRemoved;

        public void Set(string name, T value)
        {
            bool notify = true;

            lock (itemsLock)
            {
                if (!items.ContainsKey(name))
                {
                    items.Add(name, value);
                    notify = true;
                }
                else
                    items[name] = value;
            }

            if (notify)
                KeyAdded?.Invoke(name);
        }

        public void Clear()
        {
            lock (itemsLock)
            {
                items.Clear();
            }
        }

        public bool Exists(string name)
        {
            lock (itemsLock)
            {
                return items.ContainsKey(name);
            }
        }

        public T Get(string name)
        {
            lock (itemsLock)
            {
                return items[name];
            }
        }

        public bool TryGet(string name, out T value)
        {
            lock (itemsLock)
            {
                return items.TryGetValue(name, out value);
            }
        }

        public void Remove(string name)
        {
            bool notify = false;
            lock (itemsLock)
            {
                if (items.ContainsKey(name))
                {
                    notify = true;
                    items.Remove(name);
                }
            }

            if (notify)
                KeyRemoved?.Invoke(name);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            lock (itemsLock)
            {
                return items.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (itemsLock)
            {
                return items.GetEnumerator();
            }
        }

    }
}
