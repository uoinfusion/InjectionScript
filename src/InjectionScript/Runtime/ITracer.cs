using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public interface ITracer
    {
        void Enable();
        void Disable();
        string Dump();
    }
}
