using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class InjectionException : Exception
    {
        public InjectionException(string message) : base(message)
        {
        }
    }
}
