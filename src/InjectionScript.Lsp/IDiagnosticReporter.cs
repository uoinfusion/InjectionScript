using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Lsp
{
    public interface IDiagnosticReporter
    {
        void Report(Uri documentUri, MessageCollection messages);
    }
}
