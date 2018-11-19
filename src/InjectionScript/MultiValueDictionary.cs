using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript
{
    public sealed class MultiValueDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

        public IEnumerable<IEnumerable<TValue>> Values => dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                dictionary.Add(key, list);
            }

            list.Add(value);
        }

        public IEnumerable<TValue> this[TKey key] => dictionary[key];
        public bool TryGet(TKey key, out IEnumerable<TValue> values)
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                values = list;
                return true;
            }

            values = Enumerable.Empty<TValue>();
            return false;
        }
    }
}
