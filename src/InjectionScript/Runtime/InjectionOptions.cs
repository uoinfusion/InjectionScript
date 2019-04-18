using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Runtime
{
    public class InjectionOptions
    {
        public bool Light { get; set; }

        public void CopyTo(InjectionOptions other)
        {
            other.Light = Light;
        }
    }
}
