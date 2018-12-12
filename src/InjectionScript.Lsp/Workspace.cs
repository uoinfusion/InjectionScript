using InjectionScript.Runtime;
using System;
using System.Collections.Generic;

namespace InjectionScript.Lsp
{
    public class InjectionWorkspace : IInjectionWorkspace
    {
        private readonly Dictionary<string, string> documents = new Dictionary<string, string>();
        private readonly IDiagnosticReporter diagnosticReporter;
        private readonly SlidingTimer diagnosticTimer;

        private Uri lastDocumentUri;

        private object documentLock = new object();

        public InjectionWorkspace(IDiagnosticReporter diagnosticReporter)
        {
            this.diagnosticReporter = diagnosticReporter;
            diagnosticTimer = new SlidingTimer(TimeSpan.FromSeconds(1));
            diagnosticTimer.TimeoutElapsed += HandleDiagnosticUpdateTimeout;
        }

        private void HandleDiagnosticUpdateTimeout(object sender, EventArgs e)
        {
            UpdateDiagnostic();
        }

        private void UpdateDiagnostic()
        {
            string content;
            Uri uri;

            lock (documentLock)
            {
                content = documents[lastDocumentUri.AbsoluteUri];
                uri = lastDocumentUri;
            }

            var runtime = new InjectionRuntime();
            var messages = runtime.Load(content, uri.LocalPath);
            diagnosticReporter.Report(uri, messages);
        }

        public void UpdateDocument(Uri uri, string content)
        {
            lock (documentLock)
            {
                documents[uri.AbsoluteUri] = content;
                lastDocumentUri = uri;
                diagnosticTimer.Start();
            }
        }

        public bool TryGetDocument(Uri uri, out string content)
            => documents.TryGetValue(uri.AbsoluteUri, out content);
    }
}
