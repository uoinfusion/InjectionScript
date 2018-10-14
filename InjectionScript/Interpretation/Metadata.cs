using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class Metadata
    {
        private readonly Dictionary<string, SubrutineDefinition> subrutines 
            = new Dictionary<string, SubrutineDefinition>();

        public void Add(SubrutineDefinition subrutineDef)
        {
            subrutines.Add(subrutineDef.Name, subrutineDef);
        }

        internal bool TryGet(string name, out SubrutineDefinition subrutine)
        {
            return subrutines.TryGetValue(name, out subrutine);
        }
    }
}
