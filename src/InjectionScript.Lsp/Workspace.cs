using InjectionScript.Runtime;
using System;
using System.Collections.Generic;

namespace InjectionScript.Lsp
{
    public class InjectionWorkspace : IInjectionWorkspace
    {
        private readonly Dictionary<string, string> documents = new Dictionary<string, string>();
        private readonly IDiagnosticReporter diagnosticReporter;

        public InjectionWorkspace(IDiagnosticReporter diagnosticReporter)
        {
            this.diagnosticReporter = diagnosticReporter;
        }

        public void UpdateDocument(Uri uri, string content)
        {
            documents[uri.AbsoluteUri] = content;

            var runtime = new InjectionRuntime();
            var messages = runtime.Load(content, uri.LocalPath);
             diagnosticReporter.Report(uri, messages);
        }

        public bool TryGetDocument(Uri uri, out string content)
            => documents.TryGetValue(uri.AbsoluteUri, out content);
    }

    public interface IInjectionWorkspace
    {
        void UpdateDocument(Uri uri, string content);

        bool TryGetDocument(Uri uri, out string content);
    }
}
